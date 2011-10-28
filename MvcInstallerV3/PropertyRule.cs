using System.Collections.Generic;
using MvcInstaller.Settings;

namespace MvcInstaller
{
    /// <summary>
    /// Abstract class that all validation rule inherit.
    /// </summary>
    public abstract class PropertyRule
    {
        public string ErrorMessage { get; set; }
        public abstract bool Validate();
    }
}