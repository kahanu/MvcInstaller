using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcInstaller
{
    /// <summary>
    /// Validate the length of the password against the
    /// value set in the web.config/system.web/membership section.
    /// </summary>
    public class MinRequiredPasswordLength : PropertyRule
    {
        private readonly int valueToValidate;

        private readonly int requiredValue;

        public MinRequiredPasswordLength(string userName, int valueToValidate, int requiredValue)
        {
            this.requiredValue = requiredValue;
            this.valueToValidate = valueToValidate;
            ErrorMessage = string.Format("The password for '{0}' is less than the required {1} character length.", userName, requiredValue);
        }

        public override bool Validate()
        {
            bool result = true;

            if (valueToValidate < requiredValue)
            {
                result = false;
            }

            return result;
        }
    }
}
