using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using NUnit.Framework;

using MvcInstaller;
using MvcInstaller.Settings;

namespace MvcInstaller.UnitTests
{
    [TestFixture]
    public class XmlSerializerTestFixture
    {

        [Test]
        public void Serialize_xml_config_settings_from_class()
        {
            InstallerConfig config = new InstallerConfig();

            //User user = new User();
            //user.UserName = "superadmin";
            //user.Password = "!234567";

            //Role role = new Role();
            //role.Name = "SuperAdministrator";
            //role.AddUser(user);
            //config.AddRole(role);


            //role = new Role();
            //role.Name = "Administrator";

            //user = new User();
            //user.UserName = "admin";
            //user.Password = "234566y";

            //role.AddUser(user);

            //user = new User();
            //user.UserName = "byron";
            //user.Password = "93o404u";

            //role.AddUser(user);
            //config.AddRole(role);

            Database db = new Database();
            db.ConnectionStringName = "MyCoolConnection";
            db.DataSource = "localhost";
            db.InitialCatalog = "MyCoolDb";
            db.UserName = "myUserName";
            db.Password = "myPassword";
            config.Database = db;

            Serializer<InstallerConfig>.Serialize(config, "installer.config");

        }

        [Test]
        public void Read_serialized_installer_config()
        {
            InstallerConfig config = Serializer<InstallerConfig>.Deserialize(AppDomain.CurrentDomain.BaseDirectory + @"\installer.config");

            Assert.IsNotNull(config);

            string result = config.RoleManager.Roles[1].Name;
            Assert.AreEqual("Administrator", result);
        }

        [Test]
        public void Read_collection_of_roles()
        {
            InstallerConfig config = Serializer<InstallerConfig>.Deserialize(AppDomain.CurrentDomain.BaseDirectory + @"\installer.config");
            List<Role> roles = config.RoleManager.Roles;

            Assert.AreEqual(2, roles.Count);
        }

        [Test]
        public void Read_collection_of_users_for_role()
        {
            InstallerConfig config = Serializer<InstallerConfig>.Deserialize(AppDomain.CurrentDomain.BaseDirectory + @"\installer.config");
            List<Role> roles = config.RoleManager.Roles;
            foreach (var role in roles)
            {
                Console.WriteLine("Role: {0}", role.Name);
                foreach (var user in role.Users)
                {
                    Console.WriteLine("   User: {0}:{1}", user.UserName, user.Password);
                }
            }
        }


        [Test]
        public void Read_database_connection_string_values()
        {
            InstallerConfig config = Serializer<InstallerConfig>.Deserialize(AppDomain.CurrentDomain.BaseDirectory + @"\installer.config");
            Database db = config.Database;

            Assert.AreEqual("MyDbConnection", db.ConnectionStringName);
            Assert.AreEqual("localhost\\myinstance", db.DataSource);
            Assert.AreEqual("MyCoolDb", db.InitialCatalog);
            Assert.AreEqual("myUserName", db.UserName);
            Assert.AreEqual("myPassword", db.Password);
        }

        [Test]
        public void We_are_creating_the_membership_users_and_roles_since_create_is_true()
        {
            InstallerConfig config = Serializer<InstallerConfig>.Deserialize(AppDomain.CurrentDomain.BaseDirectory + @"\installer.config");
            Membership membership = config.Membership;

            Assert.IsTrue(membership.Create);
        }

        [Test]
        public void Get_the_membership_provider_name()
        {
            InstallerConfig config = Serializer<InstallerConfig>.Deserialize(AppDomain.CurrentDomain.BaseDirectory + @"\installer.config");
            Membership membership = config.Membership;

            Assert.AreEqual("MyAspNetMembershipProvider", membership.ProviderName);
        }
    }
}
