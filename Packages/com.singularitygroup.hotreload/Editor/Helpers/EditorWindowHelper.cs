using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {
    internal static class EditorWindowHelper {
        private static readonly Regex ValidEmailRegex = new Regex(@"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
            + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
            + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$", RegexOptions.IgnoreCase);

        public static bool IsValidEmailAddress(string email) {
            return ValidEmailRegex.IsMatch(email);
        }

        public static bool IsHumanControllingUs() {
            if (Application.isBatchMode) {
                return false;
            }
            
            var isCI = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI"));
            return !isCI;
        }
    }
}
