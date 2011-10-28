using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Configuration;
using MvcInstaller.Settings;

namespace MvcInstaller
{
    public class RequiresQuestionAndAnswer : PropertyRule
    {
        private readonly string question;

        private readonly string answer;

        private readonly bool required;

        private readonly string userName;

        public RequiresQuestionAndAnswer(string userName, string question, string answer, bool required)
        {
            this.userName = userName;
            this.required = required;
            this.answer = answer;
            this.question = question;
            ErrorMessage = string.Format("The Question and Answer is required for '{0}'.", userName);
        }

        public override bool Validate()
        {
            bool result = true;
            if (required)
            {
                if (string.IsNullOrEmpty(question) || string.IsNullOrEmpty(answer))
                {
                    result = false;
                }
            }

            return result;
        }
    }
}