using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcInstaller;
using MvcInstaller.Settings;

namespace RuleValidationFactory.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class FactoryTests
    {
        private IRulesValidationFactory _factory;
        private Configuration _configSection;
        private InstallerConfig _config;

        public FactoryTests()
        {
            // You need to remember to remove the ".config" in the filename for it to work correctly.
            // The ConfigurationManager tacks on a trailing ".config" to the filename.
            // C:\VSProjects\MvcInstaller\RuleValidationFactory.Tests\bin\Debug\RuleValidationFactory.Tests.dll.config.config
            // So if the actual filename is "RuleValidationFactory.Tests.dll.config", 
            // then you have to enter, RuleValidationFactory.Tests.dll for it to work.
            _configSection = ConfigurationManager.OpenExeConfiguration(AppDomain.CurrentDomain.BaseDirectory + @"\RuleValidationFactory.Tests.dll");

            _config = Serializer<InstallerConfig>.Deserialize(AppDomain.CurrentDomain.BaseDirectory + @"\installer.config");

            _factory = new RulesValidationFactory(_config, _configSection);
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext) 
        //{

        //}
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        #region Configuration tests

        [TestMethod]
        public void get_default_provider_from_membership_section()
        {
            // Arrange
            MembershipSection membershipSection = _configSection.GetSection("system.web/membership") as MembershipSection;

            // Act
            string defaultProvider = membershipSection.DefaultProvider;

            // Assert
            Assert.AreEqual("AspNetSqlMembershipProvider", defaultProvider);
        }

        [TestMethod]
        public void path()
        {
            Console.WriteLine(_configSection.FilePath);
        }

        [TestMethod]
        public void get_the_minRequiredNonalphanumericCharacters_from_the_membership_section()
        {
            // Arrange
            MembershipSection membershipSection = _configSection.GetSection("system.web/membership") as MembershipSection;

            // Act
            string defaultProvider = membershipSection.DefaultProvider;
            ProviderSettings settings = membershipSection.Providers[defaultProvider];
            int actual = Convert.ToInt32(settings.Parameters["minRequiredNonalphanumericCharacters"]);

            // Assert
            Assert.AreEqual(3, actual);
        }

        #endregion

        #region MinRequiredPasswordLength Tests

        [TestMethod]
        public void min_required_password_length_should_return_false_for_invalid_length()
        {
            // Arrange
            string password = "bubble";
            string username = "joeuser";
            int requiredPasswordLength = 7;

            // Act
            var rule = new MinRequiredPasswordLength(username, password.Length, requiredPasswordLength);
            bool actual = rule.Validate();

            // Assert
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void min_required_password_length_should_return_true_for_valid_length()
        {
            // Arrange
            string password = "bubbles";
            string username = "joeuser";
            int requiredPasswordLength = 7;

            // Act
            var rule = new MinRequiredPasswordLength(username, password.Length, requiredPasswordLength);
            bool actual = rule.Validate();

            // Assert
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void min_required_password_length_message_succeeds()
        {
            // Arrange
            string password = "bubble";
            string username = "joeuser";
            int requiredPasswordLength = 7;

            // Act
            var rule = new MinRequiredPasswordLength(username, password.Length, requiredPasswordLength);
            bool actual = rule.Validate();

            // Assert
            Assert.AreEqual(string.Format("The password for 'joeuser' is less than the required 7 character length.", requiredPasswordLength), rule.ErrorMessage);
        }

        [TestMethod]
        public void min_required_password_length_succeeds_even_with_zero_min_length_required()
        {
            // Arrange
            string password = string.Empty;
            string username = "joeuser";
            int requiredPasswordLength = 0;

            // Act
            var rule = new MinRequiredPasswordLength(username, password.Length, requiredPasswordLength);
            bool actual = rule.Validate();

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void min_required_password_length_succeeds_with_zero_min_password_length()
        {
            // Arrange
            string password = "bosco";
            string username = "joeuser";
            int requiredPasswordLength = 0;

            // Act
            var rule = new MinRequiredPasswordLength(username, password.Length, requiredPasswordLength);
            bool actual = rule.Validate();

            // Assert
            Assert.IsTrue(actual);
        }

        #endregion

        #region RequiresQuestionAndAnswer Tests

        [TestMethod]
        public void requires_q_and_a_succeeds_with_all_valid_values()
        {
            // Arrange
            string question = "Favorite color";
            string answer = "Mauve";
            string username = "joeuser";
            bool required = true;

            // Act
            var rule = new RequiresQuestionAndAnswer(username, question, answer, required);
            bool actual = rule.Validate();

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void requires_q_and_a_returns_false_with_no_answer()
        {
            // Arrange
            bool required = true;
            string question = "Favorite color";
            string answer = string.Empty;
            string username = "joeuser";

            // Act
            var rule = new RequiresQuestionAndAnswer(username, question, answer, required);
            bool actual = rule.Validate();

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void requires_q_and_a_returns_false_with_no_quesion()
        {
            // Arrange
            bool required = true;
            string question = string.Empty;
            string answer = "my answer";
            string username = "joeuser";

            // Act
            var rule = new RequiresQuestionAndAnswer(username, question, answer, required);
            bool actual = rule.Validate();

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void requires_q_and_a_returns_true_if_not_required()
        {
            // Arrange
            string question = "my question";
            string answer = "my answer";
            string username = "joeuser";
            bool required = false;

            // Act
            var rule = new RequiresQuestionAndAnswer(username, question, answer, required);
            bool actual = rule.Validate();

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void requires_q_and_a_returns_true_if_not_required_and_q_and_a_are_empty()
        {
            // Arrange
            string question = string.Empty;
            string answer = string.Empty;
            string username = "joeuser";
            bool required = false;

            // Act
            var rule = new RequiresQuestionAndAnswer(username, question, answer, required);
            bool actual = rule.Validate();

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void requires_q_and_a_returns_proper_error_message_regardless_of_the_required_value()
        {
            // Arrange
            string question = string.Empty;
            string answer = string.Empty;
            string username = "joeuser";
            bool required = false;

            // Act
            var rule = new RequiresQuestionAndAnswer(username, question, answer, required);
            string actual = rule.ErrorMessage;

            // Assert
            Assert.AreEqual("The Question and Answer is required for 'joeuser'.", actual);
        }
        #endregion

        #region MinRequiredNonAlphanumericCharacters Tests

        [TestMethod]
        public void min_required_non_alpha_chars_returns_proper_error_message()
        {
            // Arrange
            string password = "mypasword";
            string username = "myusername";
            int numOfRequiredNonAlphaChars = 1;

            // Act
            var rule = new MinRequiredNonAlphanumericCharacters(username, password, numOfRequiredNonAlphaChars);
            string actual = rule.ErrorMessage;

            // Assert
            Assert.AreEqual("The password for 'myusername' does not contain the minimum number of required alphanumeric characters.", actual);
        }

        [TestMethod]
        public void min_required_non_alpha_chars_returns_true_when_zero_required_non_alpha_chars_are_required()
        {
            // Arrange
            string password = "mypasword";
            string username = "myusername";
            int numOfRequiredNonAlphaChars = 0;

            // Act
            var rule = new MinRequiredNonAlphanumericCharacters(username, password, numOfRequiredNonAlphaChars);
            bool actual = rule.Validate();

            // Assert  
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void min_required_non_alpha_chars_returns_false_when_non_zero_amount_of_required_non_alpha_chars_are_required_and_none_are_found()
        {
            // Arrange
            string password = "mypasword";
            string username = "myusername";
            int numOfRequiredNonAlphaChars = 1;

            // Act
            var rule = new MinRequiredNonAlphanumericCharacters(username, password, numOfRequiredNonAlphaChars);
            bool actual = rule.Validate();

            // Assert  
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void min_required_non_alpha_chars_returns_true_when_non_zero_amount_of_required_non_alpha_chars_are_required_and_some_are_found()
        {
            // Arrange
            string password = "my#pasword";
            string username = "myusername";
            int numOfRequiredNonAlphaChars = 1;

            // Act
            var rule = new MinRequiredNonAlphanumericCharacters(username, password, numOfRequiredNonAlphaChars);
            bool actual = rule.Validate();

            // Assert  
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void min_required_non_alpha_chars_returns_false_when_3_required_non_alpha_chars_are_required_and_2_are_found()
        {
            // Arrange
            string password = "my#pasword!";
            string username = "myusername";
            int numOfRequiredNonAlphaChars = 3;

            // Act
            var rule = new MinRequiredNonAlphanumericCharacters(username, password, numOfRequiredNonAlphaChars);
            bool actual = rule.Validate();

            // Assert  
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void min_required_non_alpha_chars_returns_true_when_3_required_non_alpha_chars_are_required_and_3_are_found()
        {
            // Arrange
            string password = "mypasw()rd!";
            string username = "myusername";
            int numOfRequiredNonAlphaChars = 3;

            // Act
            var rule = new MinRequiredNonAlphanumericCharacters(username, password, numOfRequiredNonAlphaChars);
            bool actual = rule.Validate();

            // Assert  
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void min_required_non_alpha_chars_returns_true_when_2_required_non_alpha_chars_are_required_and_3_are_found()
        {
            // Arrange
            string password = "mypasw()rd!";
            string username = "myusername";
            int numOfRequiredNonAlphaChars = 2;

            // Act
            var rule = new MinRequiredNonAlphanumericCharacters(username, password, numOfRequiredNonAlphaChars);
            bool actual = rule.Validate();

            // Assert  
            Assert.IsTrue(actual);
        }

        #endregion

        #region RequireUniqueEmail Tests

        [TestMethod]
        public void required_unique_email_returns_proper_error_message()
        {
            // Arrange

            // Act
            var rule = new RequireUniqueEmail(null, true);
            string actual = rule.ErrorMessage;

            // Assert
            Assert.AreEqual("There are duplicate email addresses when they are supposed to be unique.", actual);
        }

        [TestMethod]
        public void required_unique_email_returns_true_with_one_unique_emails()
        {
            // Arrange
            List<string> emailList = new List<string>();
            emailList.Add("tom@email.com");
            bool requiredToBeUnique = true;

            // Act
            var rule = new RequireUniqueEmail(emailList, requiredToBeUnique);
            bool actual = rule.Validate();

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void required_unique_email_returns_true_with_two_unique_emails()
        {
            // Arrange
            List<string> emailList = new List<string>();
            emailList.Add("tom@email.com");
            emailList.Add("evan@email.com");
            bool requiredToBeUnique = true;

            // Act
            var rule = new RequireUniqueEmail(emailList, requiredToBeUnique);
            bool actual = rule.Validate();

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void required_unique_email_returns_false_with_duplicate_emails_found()
        {
            // Arrange
            List<string> emailList = new List<string>();
            emailList.Add("tom@email.com");
            emailList.Add("evan@email.com");
            emailList.Add("tom@email.com");
            bool requiredToBeUnique = true;

            // Act
            var rule = new RequireUniqueEmail(emailList, requiredToBeUnique);
            bool actual = rule.Validate();

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void required_unique_email_returns_true_with_no_duplicate_emails_found()
        {
            // Arrange
            List<string> emailList = new List<string>();
            emailList.Add("tom@email.com");
            emailList.Add("evan@email.com");
            emailList.Add("tom1@email.com");
            emailList.Add("jane@email.com");
            emailList.Add("bruce@email.com");
            bool requiredToBeUnique = true;

            // Act
            var rule = new RequireUniqueEmail(emailList, requiredToBeUnique);
            bool actual = rule.Validate();

            // Assert
            Assert.IsTrue(actual);
        }


        [TestMethod]
        public void required_unique_email_returns_true_with_duplicate_emails_found_but_is_not_required()
        {
            // Arrange
            List<string> emailList = new List<string>();
            emailList.Add("tom@email.com");
            emailList.Add("evan@email.com");
            emailList.Add("tom1@email.com");
            emailList.Add("jane@email.com");
            emailList.Add("bruce@email.com");
            emailList.Add("evan@email.com");
            bool requiredToBeUnique = false;

            // Act
            var rule = new RequireUniqueEmail(emailList, requiredToBeUnique);
            bool actual = rule.Validate();

            // Assert
            Assert.IsTrue(actual);
        }


        #endregion

        #region Rules Factory tests

        [TestMethod]
        public void rules_factory_validate_returns_false_for_various_rules_violations()
        {
            // Arrange

            // Act
            var actual = _factory.Validate();

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void rules_factory_returns_collection_of_error_messages_for_violations()
        {
            // Act
            _factory.Validate();
            int count = _factory.ValidationErrors.Count;

            // Assert
            Assert.AreEqual(6, count);
        }

        [TestMethod]
        public void rules_factory_returns_error_messages_for_violations()
        {
            // Act
            StringBuilder sb = new StringBuilder();
            if (!_factory.Validate())
            {

                foreach (string error in _factory.ValidationErrors)
                {
                    sb.AppendFormat("{0}<br />", error);
                }
            }

            string actual = sb.ToString();
            string expected = "There are duplicate email addresses when they are supposed to be unique.<br />The password for 'admin' does not contain the minimum number of required alphanumeric characters.<br />The password for 'bizuser' is less than the required 7 character length.<br />The password for 'bizuser' does not contain the minimum number of required alphanumeric characters.<br />The password for 'joemanager' does not contain the minimum number of required alphanumeric characters.<br />The Question and Answer is required for 'phil'.<br />";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
