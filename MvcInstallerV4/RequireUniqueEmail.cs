using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Configuration;
using MvcInstaller.Settings;

namespace MvcInstaller
{
    /// <summary>
    /// Validate that the email addresses in the installer.config file
    /// are unique.
    /// </summary>
    public class RequireUniqueEmail : PropertyRule
    {
        private readonly bool requiredToBeUnique;

        private readonly List<string> emailList;

        public RequireUniqueEmail(List<string> emailList, bool requiredToBeUnique)
        {
            this.emailList = emailList;
            this.requiredToBeUnique = requiredToBeUnique;
            ErrorMessage = "There are duplicate email addresses when they are supposed to be unique.";
        }

        public override bool Validate()
        {
            bool result = true;

            if (requiredToBeUnique)
            {
                foreach (string item in emailList)
                {
                    List<string> results = emailList.FindAll(
                        delegate(string email)
                        {
                            return (item == email);
                        });

                    if (results.Count > 1)
                    {
                        // validation violation
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }
    }
}