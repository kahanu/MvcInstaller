using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvcInstaller.Settings;

namespace MvcInstaller
{
    public class InstallerRulesValidator : IRulesValidationFactory
    {

        private readonly InstallerConfig config;

        public InstallerRulesValidator(InstallerConfig config)
        {
            this.config = config;
        }

        #region IRulesValidationFactory Members

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

        private void InitRules()
        {
            throw new NotImplementedException();
        }

        private IList<string> _validationErrors = new List<string>();
        public IList<string> ValidationErrors
        {
            get { return _validationErrors; }
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
