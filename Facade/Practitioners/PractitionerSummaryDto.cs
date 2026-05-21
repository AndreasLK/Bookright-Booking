using Facade.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Facade.Practitioners
{
        /// <summary>
        /// Summary representation of a practitioner for UI display and editing.
        /// </summary>
        public class PractitionerSummaryDto
        {
                /// <summary>
                /// Unique identifier.
                /// </summary>
                public Guid Id { get; set; }

                /// <summary>
                /// Display alias or working title.
                /// </summary>
                [Searchable]
                public string Alias { get; set; } = string.Empty;

                /// <summary>
                /// First name from person details.
                /// </summary>
                [Searchable]
                public string FirstName { get; set; } = string.Empty;

                /// <summary>
                /// Last name from person details.
                /// </summary>
                [Searchable]
                public string LastName { get; set; } = string.Empty;

                /// <summary>
                /// Contact email address.
                /// </summary>
                [Searchable]
                public string Email { get; set; } = string.Empty;

                /// <summary>
                /// Contact phone number.
                /// </summary>
                [Searchable]
                public string PhoneNumber { get; set; } = string.Empty;

                /// <summary>
                /// Computed full name for display.
                /// </summary>
                public string FullName => $"{this.FirstName} {this.LastName}";
        }
}
