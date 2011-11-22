using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;
using MvcInstaller.Settings;

namespace MvcInstaller
{
    /// <summary>
    /// The RulesValidationFactory class is used in the InstallWizard inside the 
    /// CreateMembership method.  It validates rules based on the web.config/membership
    /// section and returns the errors to the browser.
    /// </summary>
    public class RulesValidationFactory : IRulesValidationFactory
    {
        #region ctors
        private InstallerConfig _config;
        private MembershipSection _membershipSection;

        public RulesValidationFactory(InstallerConfig config, Configuration configSection)
        {
            this._config = config;
            this._membershipSection = (MembershipSection)configSection.GetSection("system.web/membership");
        }
        #endregion

        #region Public Methods
        public bool Validate()
        {
            InitRules();
            bool result = true;
            IList<string> errors = new List<string>();
            foreach (PropertyRule rule in _propertyRules)
            {
                if (!rule.Validate())
                {
                    result = false;
                    errors.Add(rule.ErrorMessage);
                }
            }

            if (errors.Count > 0)
            {
                _validationErrors = errors.Distinct().ToList();
            }

            return result;
        }

        #endregion

        #region Public Properties
        private IList<string> _validationErrors = new List<string>();
        public IList<string> ValidationErrors
        {
            get { return _validationErrors; }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Setup the rules to validate on the Membership section attributes.
        /// </summary>
        private void InitRules()
        {
            // First get the values for the rules we want to validate.
            bool requiresQandA = false;
            bool requiresUniqueEmail = false;
            int minRequiredPasswordLength = 0;
            int minRequiredNonalphanumericCharacters = 0;

            // Get the values from the web.config file.
            ProviderSettings settings = _membershipSection.Providers[_membershipSection.DefaultProvider];
            requiresQandA = Convert.ToBoolean(settings.Parameters["requiresQuestionAndAnswer"]);
            requiresUniqueEmail = Convert.ToBoolean(settings.Parameters["requiresUniqueEmail"]);
            minRequiredPasswordLength = Convert.ToInt32(settings.Parameters["minRequiredPasswordLength"]);
            minRequiredNonalphanumericCharacters = Convert.ToInt32(settings.Parameters["minRequiredNonalphanumericCharacters"]);

            List<string> emailList = new List<string>();
            foreach (Role role in _config.RoleManager.Roles)
            {
                foreach (User user in role.Users)
                {
                    // Add rules here
                    // -- Validate length of password against web.config/membership/providers/minRequiredPasswordLength
                    // -- Validate number of nonalphanumericcharacters against web.confg
                    // -- Validate that emails are unique if required
                    // -- Validate that if Q and A is required, they are supplied

                    emailList.Add(user.Email);
                    AddRule(new RequireUniqueEmail(emailList, requiresUniqueEmail));
                    AddRule(new MinRequiredPasswordLength(user.UserName, user.Password.Length, minRequiredPasswordLength));
                    AddRule(new MinRequiredNonAlphanumericCharacters(user.UserName, user.Password, minRequiredNonalphanumericCharacters));
                    AddRule(new RequiresQuestionAndAnswer(user.UserName, user.SecretQuestion, user.SecretAnswer, requiresQandA));
                }
            }
        }

        private IList<PropertyRule> _propertyRules = new List<PropertyRule>();

        /// <summary>
        /// Add a rule to the list of rules to validate.
        /// </summary>
        /// <param name="rule"></param>
        private void AddRule(PropertyRule rule)
        {
            _propertyRules.Add(rule);
        }
        #endregion
    }
}
