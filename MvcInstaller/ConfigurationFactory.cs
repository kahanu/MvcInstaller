using MvcInstaller.Settings;
using System.Configuration;
using System.Web.Configuration;

namespace MvcInstaller
{
    /// <summary>
    /// You can manipulate any configuration section in this class.
    /// </summary>
    public class ConfigurationFactory : IConfigurationFactory
    {
        #region ctors
        private IConnectionStringComponent component;

        public ConfigurationFactory(IConnectionStringComponent component)
        {
            this.component = component;
        }

        public ConfigurationFactory()
        {

        }
        #endregion

        #region IConfigurationFactory Members

        public void Execute(InstallerConfig config, Configuration configSection)
        {
            UpdateConnectionString(config, configSection);

            UpdateMembershipConnectionStrings(configSection, config);
        }


        #endregion

        #region Private Methods


        /// <summary>
        /// Update the web.config connection strings. If the EntityFramework is being used, two connection strings
        /// will be created. One for the EntityFramework, and another for the Membership system.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="configSection"></param>
        private void UpdateConnectionString(InstallerConfig config, Configuration configSection)
        {
            string connString = component.GetConnString();
            string providerName = "System.Data.SqlClient";
            if (!string.IsNullOrEmpty(config.Database.EntityFrameworkEntitiesName))
            {
                providerName = "System.Data.EntityClient";
                connString = component.BuildEntityFrameworkConnectionString();
            }

            ConnectionStringsSection connectionStringsSection = configSection.ConnectionStrings;
            ConnectionStringSettings appTemplate = new ConnectionStringSettings(config.Database.ConnectionStringName, connString, providerName);
            connectionStringsSection.ConnectionStrings.Clear();
            connectionStringsSection.ConnectionStrings.Add(appTemplate);

            if (!string.IsNullOrEmpty(config.Database.EntityFrameworkEntitiesName) && config.Membership.Create)
            {
                ConnectionStringSettings connTemplate = new ConnectionStringSettings("MembershipConnection", component.GetConnString(), "System.Data.SqlClient");
                connectionStringsSection.ConnectionStrings.Add(connTemplate);
            }
        }

        /// <summary>
        /// Update the web.config membership related config sections, membership, profile, and roleManager.
        /// And also set the roleManager Enabled property to true.
        /// </summary>
        /// <param name="configSection"></param>
        /// <param name="config"></param>
        private void UpdateMembershipConnectionStrings(Configuration configSection, InstallerConfig config)
        {
            if (config.Membership.Create)
            {
                MembershipSection membership = configSection.GetSection("system.web/membership") as MembershipSection;
                ProfileSection profile = configSection.GetSection("system.web/profile") as ProfileSection;
                RoleManagerSection roleManager = configSection.GetSection("system.web/roleManager") as RoleManagerSection;

                string connString = config.Database.ConnectionStringName;
                if (!string.IsNullOrEmpty(config.Database.EntityFrameworkEntitiesName))
                {
                    connString = "MembershipConnection";
                }

                // Update the membership section.
                membership.DefaultProvider = config.Membership.ProviderName;
                for (int i = 0; i < membership.Providers.Count; i++)
                {
                    membership.Providers[i].Parameters["connectionStringName"] = connString;
                    membership.Providers[i].Parameters["name"] = config.Membership.ProviderName;
                    membership.Providers[i].Parameters["applicationName"] = config.ApplicationName;
                }

                // Update the profile section.
                for (int i = 0; i < profile.Providers.Count; i++)
                {
                    profile.Providers[i].Parameters["connectionStringName"] = connString;
                    profile.Providers[i].Parameters["name"] = config.Profile.ProviderName;
                    profile.Providers[i].Parameters["applicationName"] = config.ApplicationName;
                }

                // Update the roleManager section.
                for (int i = 0; i < roleManager.Providers.Count; i++)
                {
                    if (roleManager.Providers[i].Type == "System.Web.Security.SqlRoleProvider")
                    {
                        roleManager.Providers[i].Parameters["connectionStringName"] = connString;
                        roleManager.Providers[i].Parameters["name"] = config.RoleManager.ProviderName;
                    }
                    roleManager.Providers[i].Parameters["applicationName"] = config.ApplicationName;
                }

                roleManager.Enabled = true;
            }
        } 

        #endregion

    }
}
