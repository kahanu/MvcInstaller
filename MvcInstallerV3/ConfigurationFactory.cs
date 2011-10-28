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

            UpdateMembershipProviders(configSection, config);
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
        private void UpdateMembershipProviders(Configuration configSection, InstallerConfig config)
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
                bool isCustomMembershipProvider = (config.Membership.ProviderName == "AspNetSqlMembershipProvider") ? false : true;
                membership.DefaultProvider = config.Membership.ProviderName;

                for (int i = 0; i < membership.Providers.Count; i++)
                {
                    if (membership.Providers[i].Name == "AspNetSqlMembershipProvider")
                    {
                        membership.Providers[i].Parameters["connectionStringName"] = connString;
                        membership.Providers[i].Parameters["name"] = "AspNetSqlMembershipProvider";
                        membership.Providers[i].Parameters["type"] = "System.Web.Security.SqlMembershipProvider";
                        membership.Providers[i].Parameters["applicationName"] = config.ApplicationName;
                    }
                }

                if (isCustomMembershipProvider)
                {
                    // Create a new provider.
                    ProviderSettings p = new ProviderSettings();
                    p.Parameters["connectionStringName"] = connString;
                    p.Parameters["name"] = config.Membership.ProviderName;
                    p.Parameters["type"] = config.Membership.type;
                    p.Parameters["applicationName"] = config.ApplicationName;
                    membership.Providers.Add(p);
                }


                // Update the profile section.
                bool isCustomProfileProvider = (config.Profile.ProviderName == "AspNetSqlProfileProvider") ? false : true;
                profile.DefaultProvider = config.Profile.ProviderName;
                for (int i = 0; i < profile.Providers.Count; i++)
                {
                    if (profile.Providers[i].Name == "AspNetSqlProfileProvider")
                    {
                        profile.Providers[i].Parameters["connectionStringName"] = connString;
                        profile.Providers[i].Parameters["name"] = "AspNetSqlProfileProvider";
                        profile.Providers[i].Parameters["type"] = "System.Web.Profile.SqlProfileProvider";
                        profile.Providers[i].Parameters["applicationName"] = config.ApplicationName;
                    }
                }

                if (isCustomProfileProvider)
                {
                    ProviderSettings p = new ProviderSettings();
                    p.Parameters["connectionStringName"] = connString;
                    p.Parameters["name"] = config.Profile.ProviderName;
                    p.Parameters["type"] = config.Profile.type;
                    p.Parameters["applicationName"] = config.ApplicationName;
                    profile.Providers.Add(p);
                }


                // Update the roleManager section.
                bool isCustomRoleProvider = (config.RoleManager.ProviderName == "AspNetSqlRoleProvider") ? false : true;
                roleManager.DefaultProvider = config.RoleManager.ProviderName;
                for (int i = 0; i < roleManager.Providers.Count; i++)
                {
                    if (roleManager.Providers[i].Name == "AspNetSqlRoleProvider")
                    {
                        roleManager.Providers[i].Parameters["connectionStringName"] = connString;
                        roleManager.Providers[i].Parameters["name"] = "AspNetSqlRoleProvider";
                        roleManager.Providers[i].Parameters["type"] = "System.Web.Security.SqlRoleProvider";
                        roleManager.Providers[i].Parameters["applicationName"] = config.ApplicationName;
                    }
                }

                if (isCustomRoleProvider)
                {
                    ProviderSettings p = new ProviderSettings();
                    p.Parameters["connectionStringName"] = connString;
                    p.Parameters["name"] = config.RoleManager.ProviderName;
                    p.Parameters["type"] = config.RoleManager.type;
                    p.Parameters["applicationName"] = config.ApplicationName;
                    roleManager.Providers.Add(p);
                }

                roleManager.Enabled = true;
            }
        }

        #endregion

    }
}
