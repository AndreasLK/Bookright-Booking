using Domain.Entities.Persons;
using System;
using System.Collections.Generic;
using System.Text;

namespace Facade.Common.Extensions
{
        public static class PersonExtensions
        {
                /// <summary>
                /// Formats a Person's name for UI display, prioritizing preferred names.
                /// </summary>
                public static string ToDisplayFullName(this Person person)
                {
                        if (person == null) return "Unknown Person";

                        string first = string.IsNullOrWhiteSpace(person.PreferredFirstName)
                            ? person.LegalFirstName
                            : person.PreferredFirstName;

                        string last = string.IsNullOrWhiteSpace(person.PreferredLastName)
                            ? person.LegalLastName
                            : person.PreferredLastName;

                        return $"{first} {last}".Trim();
                }
        }
}
