using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcInstaller
{
    public class RequiresApplicationName : PropertyRule
    {

        private readonly string applicationName;

        public RequiresApplicationName(string applicationName)
        {
            this.applicationName = applicationName;
            ErrorMessage = "The ApplicationName cannot be empty.  It must contain at least a slash '/'.";
        }

        public override bool Validate()
        {
            bool result = true;
            if (string.IsNullOrEmpty(applicationName))
            {
                result = false;
            }

            return result;
        }
    }
}
