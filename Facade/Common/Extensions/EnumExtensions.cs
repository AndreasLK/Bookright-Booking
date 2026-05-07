using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
namespace Facade.Common.Extensions
{
        /// <summary>
        /// Provides extension methods for Enum types to improve UI presentation.
        /// </summary>
        public static class EnumExtensions
        {
                private const string CAMEL_CASE_SPLIT_PATTERN = "([a-z])([A-Z])";
                private const string CAMEL_CASE_SPLIT_REPLACEMENT = "$1 $2";

                /// <summary>
                /// Gets the human-readable display name of an enum value. 
                /// Prioritizes the [Display] attribute, falling back to spacing out PascalCase names.
                /// </summary>
                /// <param name="value">The enum value to format.</param>
                /// <returns>A formatted string ready for UI display.</returns>
                public static string GetDisplayName(this Enum value)
                {
                        string stringValue = value.ToString();

                        MemberInfo? memberInfo = value.GetType()
                                                      .GetMember(name: stringValue)
                                                      .FirstOrDefault();

                        DisplayAttribute? displayAttribute = memberInfo?.GetCustomAttribute<DisplayAttribute>();

                        // Happy Path (or Guard Clause depending on how you view it)
                        if (displayAttribute != null && !string.IsNullOrWhiteSpace(value: displayAttribute.Name))
                        {
                                return displayAttribute.Name;
                        }

                        // Fallback (No magic strings, using named arguments)
                        return Regex.Replace(
                            input: stringValue,
                            pattern: CAMEL_CASE_SPLIT_PATTERN,
                            replacement: CAMEL_CASE_SPLIT_REPLACEMENT);
                }
        }
}
