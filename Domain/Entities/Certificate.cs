using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;

namespace Domain.Entities
{
        public record Certificate
        {
                /// <summary>
                /// Official title of the accreditation.
                /// </summary>
                public string Name { get; private set; }

                /// <summary>
                /// Unique certificate Authorizationnumber.
                /// </summary>
                public string CertificateId { get; private set; }

                /// <summary>
                /// Aquired professional level or role.
                /// </summary>
                public AuthorizationType AuthorizationType { get; private set; }

                /// <summary>
                /// Effective start date.
                /// </summary>
                public DateOnly ValidFrom { get; private set; }

                /// <summary>
                /// Expiration date.
                /// </summary>
                public DateOnly? ValidUntil { get; private set; }

                /// <summary>
                /// Storage path for digital credential scan.
                /// </summary>
                public string? PhotoFilePath { get; private set; }
        }
}
