using Facade.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Facade.Clinics
{
        /// <summary>
        /// Data Transfer Object representing a clinic for UI selection and filtering.
        /// </summary>
        public record ClinicDto
        {
                /// <summary>
                /// Unique identifier for the clinic.
                /// </summary>
                public Guid Id { get; init; }

                /// <summary>
                /// The display name of the clinic.
                /// </summary>
                [Searchable]
                public string Name { get; init; } = string.Empty;
        }
}
