// Copyright (c) 2010, Gizmo Beach.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// - Neither  the  name  of  the  Gizmo Beach  nor   the   names  of  its
//   contributors may be used to endorse or  promote  products  derived  from  this
//   software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING,  BUT  NOT  LIMITED TO, THE IMPLIED
// WARRANTIES  OF  MERCHANTABILITY   AND  FITNESS  FOR  A  PARTICULAR  PURPOSE  ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL,  SPECIAL,  EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO,  PROCUREMENT  OF  SUBSTITUTE  GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)  HOWEVER  CAUSED AND ON
// ANY  THEORY  OF  LIABILITY,  WHETHER  IN  CONTRACT,  STRICT  LIABILITY,  OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE)  ARISING  IN  ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
using MvcInstaller.Settings;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.Configuration;
using System.Web.Security;
using System.Xml.Linq;

namespace MvcInstaller
{
    /// <summary>
    /// This is the primary class that handles installing your web application.
    /// </summary>
    public class InstallWizard
    {
        public InstallWizard()
        {

        }

        /// <summary>
        /// Run the process to install the database and users.
        /// </summary>
        /// <param name="config"></param>
        public static void Run(InstallerConfig config)
        {
            if (config == null)
                throw new NullReferenceException("config");

            try
            {
                // Update the web.config/connectionStrings section.
                UpdateWebConfig(config);

                // Create membership roles and users.
                CreateMembership(config);

                // Now execute the sql scripts.
                RunScripts(config);

                // Finally remove the AppInstalled key in the appsettings.
                RemoveAppInstalledKey();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Detects whether the application has already been installed based on the AppInstalled appsettings key.
        /// This is used in the Index action of the InstallController.
        /// </summary>
        public static bool Installed
        {
            get
            {
                bool blnOk = false;

                // This needs to be created before you execute the installer.
                // Add this to the appSettings section...
                //    <appSettings>
                //       <add key="AppInstalled" value="false" />
                //    </appSettings>
                // This will automatically be removed when the installation is successful.
                string installed = WebConfigurationManager.AppSettings["AppInstalled"];

                if (installed == null)
                {
                    // If the key doesn't exist, then we are going to assume
                    // it's already been installed.
                    blnOk = true;
                }
                else
                {
                    blnOk = Convert.ToBoolean(installed);
                }
                return blnOk;
            }
        }

        #region Private Methods


        /// <summary>
        /// Create the Membership database and set the roles and users.
        /// </summary>
        /// <param name="config"></param>
        private static void CreateMembership(InstallerConfig config)
        {
            if (config.Membership.Create)
            {
                // Let's first verify that the RoleManager is enabled.
                if (!Roles.Enabled)
                    throw new ApplicationException("The RoleManager was not Enabled. It has been updated! Click &quot;Install&quot; to continue.");

                // Add the ASPNETDB tables to the database using the SqlServices Install method.
                // This will add the ASPNETDB tables to the same database as the application.
                // NOTE: This method can ONLY be used for SQL Server.  To point to MySql, 
                // you will need to create the database scripts for MySql and add them to 
                // the RunScripts method, but then you need to have the MySql data provider
                // set in the web.config.
                if (config.Database.UseTrustedConnection)
                {
                    // For SQL Server trusted connections
                    System.Web.Management.SqlServices.Install(config.Database.DataSource.Trim(), config.Database.InitialCatalog, System.Web.Management.SqlFeatures.All);
                }
                else
                {
                    // For SQL Server
                    System.Web.Management.SqlServices.Install(config.Database.DataSource.Trim(), config.Database.UserName, config.Database.Password, config.Database.InitialCatalog, System.Web.Management.SqlFeatures.All);
                }

                // Now create the roles
                foreach (var role in config.RoleManager.Roles)
                {
                    if (!Roles.RoleExists(role.Name))
                        Roles.CreateRole(role.Name);

                    // Now create user and add to role.
                    foreach (var user in role.Users)
                    {
                        MembershipCreateStatus status = MembershipCreateStatus.UserRejected;
                        MembershipUser u = System.Web.Security.Membership.CreateUser(user.UserName.Trim(), user.Password.Trim(), user.Email.Trim(),
                            user.SecretQuestion.Trim(), user.SecretAnswer.Trim(), true, out status);

                        if (status == MembershipCreateStatus.Success)
                        {
                            Roles.AddUserToRole(user.UserName, role.Name);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Run the sql scripts in the location set in the installer.config file.
        /// </summary>
        /// <param name="config"></param>
        private static void RunScripts(InstallerConfig config)
        {
            string[] files = Directory.GetFiles(config.Path.AppPath + config.Path.RelativeSqlPath);
            foreach (string file in files)
            {
                string[] statements = GetScriptStatements(File.ReadAllText(file, new System.Text.UTF8Encoding()));
                ExecuteStatements(statements, config);
            }
        }

        /// <summary>
        /// This will execute the sql statements.
        /// </summary>
        /// <param name="tableStatements"></param>
        /// <param name="dbName"></param>
        private static void ExecuteStatements(string[] tableStatements, InstallerConfig config)
        {
            if (tableStatements.Length > 0)
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    IConnectionStringComponent component = new ConnectionStringComponent(config);
                    conn.ConnectionString = component.GetConnString();
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(string.Empty, conn))
                    {
                        foreach (string statement in tableStatements)
                        {
                            command.CommandText = statement;
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// This gets individual script statements from the sql script.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static string[] GetScriptStatements(string p)
        {
            string[] statements = p.Split(new string[] { "GO\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            return statements;
        }


        /// <summary>
        /// Write the new connection string to the web.config file and update the membership sections if necessary.
        /// </summary>
        private static void UpdateWebConfig(InstallerConfig config)
        {
            System.Configuration.Configuration configSection = WebConfigurationManager.OpenWebConfiguration("~");
            if (configSection == null)
            {
                throw new InvalidOperationException("Configuration file not available.");
            }

            IConnectionStringComponent component = new ConnectionStringComponent(config);
            IConfigurationFactory factory = new ConfigurationFactory(component);
            factory.Execute(config, configSection);

            // You can uncomment this if you want to add a LocalSqlServer connection.
            // I am connecting to the ASPNETDB tables with the same connection string since they are both in the same database.
            // If you create a separate ASPNETDB database, then you can uncomment the following and set the connection string manually.
            //ConnectionStringSettings LocalSqlServer = new ConnectionStringSettings("LocalSqlServer", GetConnString(config), "System.Data.SqlClient");
            //connectionStringsSection.ConnectionStrings.Add(LocalSqlServer);
            configSection.Save();
        }

        /// <summary>
        /// Remove the AppInstalled key from the appsettings section.
        /// </summary>
        /// <param name="config"></param>
        private static void RemoveAppInstalledKey()
        {
            System.Configuration.Configuration configSection = WebConfigurationManager.OpenWebConfiguration("~");
            if (configSection == null)
            {
                throw new InvalidOperationException("Configuration file not available.");
            }
            AppSettingsSection appSection = configSection.AppSettings;
            appSection.Settings.Remove("AppInstalled");
            configSection.Save();
        }

        #endregion
    }
}
