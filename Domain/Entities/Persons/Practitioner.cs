using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities.Persons;
using Domain.Enums;
using Domain.Value_Objects.Ids;

namespace Domain.Entities.People
{
        /// <inheritdoc />
        public class Practitioner : Person
        {
                /// <summary>
                /// Unique practitioner identifier.
                /// </summary>
                public PractitionerId Id { get; init; }

                /// <summary>
                /// Professional certificate identifiers.
                /// </summary>
                public List<Guid> CertificateId = new List<Guid>();

                /// <summary>
                /// Professional nickname or clinical handle.
                /// </summary>
                public string? Alias { get; private set; }


        }

}
