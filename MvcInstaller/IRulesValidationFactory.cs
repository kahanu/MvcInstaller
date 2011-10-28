using System.Collections.Generic;

namespace MvcInstaller
{
    /// <summary>
    /// Interface for the RuleValidationFactory.
    /// </summary>
    public interface IRulesValidationFactory
    {
        bool Validate();
        IList<string> ValidationErrors { get; }
    }
}
