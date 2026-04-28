using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;

namespace Domain.Entities
{
        public record Certificate
        {
                public string Name { get; private set; }

                public string CertificateId { get; private set; }

                public AuthorizationType AuthorizationType { get; private set; }

                public DateOnly ValidFrom { get; private set; }

                public DateOnly? ValidUntil { get; private set; }

                public string? PhotoFilePath { get; private set; }
        }
}
