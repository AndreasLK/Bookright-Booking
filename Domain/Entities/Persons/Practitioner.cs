
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Domain.Entities.Persons
{
        /// <inheritdoc />
        public class Practitioner : Person
        {
                /// <summary>
                /// Unique practitioner identifier.
                /// </summary>
                public PractitionerId Id { get; init; }

                private readonly List<Certificate> _certificates = new List<Certificate>();
                /// <summary>
                /// Professional certificate identifiers.
                /// </summary>
                public IReadOnlyCollection<Certificate> Certificates => this._certificates.AsReadOnly();

                /// <summary>
                /// Professional nickname or clinical handle.
                /// </summary>
                public string? Alias { get; private set; }

                public Practitioner(
                        PractitionerId id,
                        string? alias,
                        PersonDetails details) : base(details: details)
                {
                        this.Id = id;
                        this.Alias = alias;
                }

                /// <summary>
                /// Add a new certificate to the Practitioner
                /// </summary>
                public void AddCertificate(Certificate certificate)
                {

                        if (this._certificates.Exists(c => c.CertificateId == certificate.CertificateId))
                        {
                                throw new InvalidOperationException(message: $"Practitioner already has Certificate {certificate.CertificateId}");
                        }
                        this._certificates.Add(certificate);
                }

                /// <summary>
                /// Remove certificate from the Practitioner
                /// </summary>
                public void RemoveCertificate(CertificateId id)
                {
                        Certificate? certificate = this._certificates.FirstOrDefault(cert => cert.CertificateId == id);
                        if (certificate is not null)
                        {
                                this._certificates.Remove(certificate);
                        }
                }
        }

}
