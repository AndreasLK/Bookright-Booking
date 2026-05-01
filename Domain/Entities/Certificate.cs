using Domain.Enums;
using Domain.Value_Objects.Ids;

namespace Domain.Entities
{
        /// <summary>
        /// Represents a professional accreditation or license.
        /// </summary>
        public record Certificate
        {
                public string Name { get; init; }
                public CertificateId CertificateId { get; init; }
                public AuthorizationType AuthorizationType { get; init; }
                public DateOnly ValidFrom { get; init; }
                public DateOnly? ValidUntil { get; init; }
                public string? PhotoFilePath { get; init; }

                /// <param name="name">Official title of the accreditation.</param>
                /// <param name="certificateId">Unique certificate authorization number.</param>
                /// <param name="authorizationType">Acquired professional level or role.</param>
                /// <param name="validFrom">Effective start date.</param>
                /// <param name="validUntil">Optional expiration date.</param>
                /// <param name="photoFilePath">Storage path for digital credential scan.</param>
                public Certificate(
                    string name,
                    CertificateId certificateId,
                    AuthorizationType authorizationType,
                    DateOnly validFrom,
                    DateOnly? validUntil = null,
                    string? photoFilePath = null)
                {
                        // --- Guard Clauses ---
                        if (string.IsNullOrWhiteSpace(name))
                                throw new ArgumentException("Name is required.", nameof(name));

                        if (validUntil.HasValue && validUntil < validFrom)
                                throw new ArgumentOutOfRangeException(nameof(validUntil), "Expiry date cannot be before start date.");

                        // --- Assignments ---
                        this.Name = name;
                        this.CertificateId = certificateId;
                        this.AuthorizationType = authorizationType;
                        this.ValidFrom = validFrom;
                        this.ValidUntil = validUntil;
                        this.PhotoFilePath = photoFilePath;
                }
        }
}
