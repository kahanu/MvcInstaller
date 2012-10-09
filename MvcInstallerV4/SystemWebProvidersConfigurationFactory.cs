using System;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;
using MvcInstaller.Settings;

namespace MvcInstaller
{
    public class SystemWebProvidersConfigurationFactory : IConfigurationFactory
    {
        #region ctors
        private IConnectionStringComponent component;

        public SystemWebProvidersConfigurationFactory(IConnectionStringComponent component)
        {
            this.component = component;
        }

        public SystemWebProvidersConfigurationFactory()
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
            ConnectionStringsSection connectionStringsSection = configSection.ConnectionStrings;

            string connString = string.Empty;
            string providerName = string.Empty;

            if (!string.IsNullOrEmpty(config.Database.EntityFrameworkEntitiesName))
            {
                // EntityFramework connection string
                providerName = component.GetProviderName;
                connString = component.GetConnString();

                string efname = config.Database.EntityFrameworkEntitiesName;

                ConnectionStringSettings appTemplate = new ConnectionStringSettings(config.Database.EntityFrameworkEntitiesName, connString, providerName);
                //connectionStringsSection.ConnectionStrings.Clear();
                connectionStringsSection.ConnectionStrings.Add(appTemplate);

                if (!string.IsNullOrEmpty(config.Database.EntityFrameworkEntitiesName) && config.Membership.Create)
                {
                    config.Database.EntityFrameworkEntitiesName = "";
                    ConnectionStringSettings connTemplate = new ConnectionStringSettings("MembershipConnection", component.GetConnString(), "System.Data.SqlClient");
                    connectionStringsSection.ConnectionStrings.Add(connTemplate);
                    config.Database.EntityFrameworkEntitiesName = efname;
                }
            }
            else
            {
                // Standard SqlServer Connection string
                providerName = component.GetProviderName;
                connString = component.GetConnString();

                ConnectionStringSettings appTemplate = new ConnectionStringSettings(config.Database.ConnectionStringName, connString, providerName);
                //connectionStringsSection.ConnectionStrings.Clear();
                connectionStringsSection.ConnectionStrings.Add(appTemplate);
            }
        }

        /// <summary>
        /// Update the web.config membership related config sections, membership, profile, and roleManager.
        /// And also set the roleManager Enabled property to true.
        /// This is updated for the new System.Web.Providers namespace.
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
                SessionStateSection sessionState = configSection.GetSection("system.web/sessionState") as SessionStateSection;

                string connString = config.Database.ConnectionStringName;
                if (!string.IsNullOrEmpty(config.Database.EntityFrameworkEntitiesName))
                {
                    connString = "MembershipConnection";
                }

                // Update the membership section.
                bool isCustomMembershipProvider = (config.Membership.ProviderName == "DefaultMembershipProvider") ? false : true;
                membership.DefaultProvider = config.Membership.ProviderName;

                for (int i = 0; i < membership.Providers.Count; i++)
                {
                    if (membership.Providers[i].Name == "DefaultMembershipProvider")
                    {
                        membership.Providers[i].Parameters["connectionStringName"] = connString;
                        membership.Providers[i].Parameters["name"] = "DefaultMembershipProvider";
                        membership.Providers[i].Parameters["type"] = "System.Web.Providers.DefaultMembershipProvider, System.Web.Providers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
                        membership.Providers[i].Parameters["applicationName"] = config.ApplicationName;
                    }
                }

                if (isCustomMembershipProvider)
                {
                    bool exists = false;
                    for (int i = 0; i < membership.Providers.Count; i++)
                    {
                        if (membership.Providers[i].Name == config.Membership.ProviderName)
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)
                    {
                        // Create a new provider.
                        ProviderSettings p = new ProviderSettings();
                        p.Parameters["connectionStringName"] = connString;
                        p.Parameters["name"] = config.Membership.ProviderName;
                        p.Parameters["type"] = config.Membership.type;
                        p.Parameters["applicationName"] = config.ApplicationName;
                        membership.Providers.Add(p);
                    }
                }


                // Update the profile section.
                bool isCustomProfileProvider = (config.Profile.ProviderName == "DefaultProfileProvider") ? false : true;
                profile.DefaultProvider = config.Profile.ProviderName;
                for (int i = 0; i < profile.Providers.Count; i++)
                {
                    if (profile.Providers[i].Name == "DefaultProfileProvider")
                    {
                        profile.Providers[i].Parameters["connectionStringName"] = connString;
                        profile.Providers[i].Parameters["name"] = "DefaultProfileProvider";
                        profile.Providers[i].Parameters["type"] = "System.Web.Providers.DefaultProfileProvider, System.Web.Providers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
                        profile.Providers[i].Parameters["applicationName"] = config.ApplicationName;
                    }
                }

                if (isCustomProfileProvider)
                {
                    bool exists = false;
                    for (int i = 0; i < profile.Providers.Count; i++)
                    {
                        if (profile.Providers[i].Name == config.Profile.ProviderName)
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)
                    {
                        ProviderSettings p = new ProviderSettings();
                        p.Parameters["connectionStringName"] = connString;
                        p.Parameters["name"] = config.Profile.ProviderName;
                        p.Parameters["type"] = config.Profile.type;
                        p.Parameters["applicationName"] = config.ApplicationName;
                        profile.Providers.Add(p);
                    }
                }


                // Update the roleManager section.
                bool isCustomRoleProvider = (config.RoleManager.ProviderName == "DefaultRoleProvider") ? false : true;
                roleManager.DefaultProvider = config.RoleManager.ProviderName;
                for (int i = 0; i < roleManager.Providers.Count; i++)
                {
                    if (roleManager.Providers[i].Name == "DefaultRoleProvider")
                    {
                        roleManager.Providers[i].Parameters["connectionStringName"] = connString;
                        roleManager.Providers[i].Parameters["name"] = "DefaultRoleProvider";
                        roleManager.Providers[i].Parameters["type"] = "System.Web.Providers.DefaultRoleProvider, System.Web.Providers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
                        roleManager.Providers[i].Parameters["applicationName"] = config.ApplicationName;
                    }
                }

                if (isCustomRoleProvider)
                {
                    bool exists = false;
                    for (int i = 0; i < roleManager.Providers.Count; i++)
                    {
                        if (roleManager.Providers[i].Name == config.RoleManager.ProviderName)
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)
                    {
                        ProviderSettings p = new ProviderSettings();
                        p.Parameters["connectionStringName"] = connString;
                        p.Parameters["name"] = config.RoleManager.ProviderName;
                        p.Parameters["type"] = config.RoleManager.type;
                        p.Parameters["applicationName"] = config.ApplicationName;
                        roleManager.Providers.Add(p);
                    }
                }

                roleManager.Enabled = true;

                // Update new SessionState section.
                bool isCustomSessionStateProvider = (config.SessionState.ProviderName == "DefaultSessionProvider") ? false : true;
                sessionState.CustomProvider = config.SessionState.ProviderName;
                for (int i = 0; i < sessionState.Providers.Count; i++)
                {
                    if (sessionState.Providers[i].Name == "DefaultSessionProvider")
                    {
                        sessionState.Providers[i].Parameters["connectionStringName"] = connString;
                        sessionState.Providers[i].Parameters["name"] = "DefaultSessionProvider";
                        sessionState.Providers[i].Parameters["type"] = "System.Web.Providers.DefaultSessionStateProvider, System.Web.Providers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
                        sessionState.Providers[i].Parameters["applicationName"] = config.ApplicationName;
                    }
                }

                if (isCustomSessionStateProvider)
                {
                    bool exists = false;
                    for (int i = 0; i < sessionState.Providers.Count; i++)
                    {
                        if (sessionState.Providers[i].Name == config.SessionState.ProviderName)
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)
                    {
                        ProviderSettings p = new ProviderSettings();
                        p.Parameters["connectionStringName"] = connString;
                        p.Parameters["name"] = config.SessionState.ProviderName;
                        p.Parameters["type"] = config.SessionState.type;
                        p.Parameters["applicationName"] = config.ApplicationName;
                        sessionState.Providers.Add(p);
                    }
                }
            }
        }

        #endregion
    }
}
