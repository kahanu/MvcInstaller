using System.Collections.Generic;
using System.Configuration;
using System.Web.Configuration;
using MvcInstaller.Settings;

namespace MvcInstaller
{
    /// <summary>
    /// Validate the number of non alphanumeric characters to the 
    /// value set in the web.confg/system.web/membership section.
    /// </summary>
    public class MinRequiredNonAlphanumericCharacters : PropertyRule
    {
        private readonly string passwordToValidate;
        private readonly int numberOfRequiredCharacters;
        private readonly string userName;

        public MinRequiredNonAlphanumericCharacters(string userName, string passwordToValidate, int numberOfRequiredCharacters)
        {
            this.userName = userName;
            this.numberOfRequiredCharacters = numberOfRequiredCharacters;
            this.passwordToValidate = passwordToValidate;
            ErrorMessage = string.Format("The password for '{0}' does not contain the minimum number of required alphanumeric characters.", userName);
        }

        public override bool Validate()
        {
            bool result = true;

            if (numberOfRequiredCharacters > 0)
            {
                string strChars = "!@#$%^&*()_<>{}";
                char[] chars = strChars.ToCharArray();

                int[] intResult = FindAny(chars, passwordToValidate, 0);

                if (intResult.Length < numberOfRequiredCharacters)
                {
                    result = false;
                }
            }

            return result;
        }

        private int[] FindAny(char[] charsToSearch, string stringToMatch, int startPos)
        {
            int foundPos = -1;
            int count = 0;
            List<int> foundItems = new List<int>();

            do
            {
                foundPos = stringToMatch.IndexOfAny(charsToSearch, startPos);
                if (foundPos > -1)
                {
                    startPos = foundPos + 1;
                    count++;
                    foundItems.Add(foundPos);
                }
            } while (foundPos > -1 && startPos < stringToMatch.Length);

            return foundItems.ToArray();
        }
    }
}