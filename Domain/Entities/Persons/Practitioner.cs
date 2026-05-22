
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

                /// <summary>
                /// Initializes a new instance of the <see cref="Practitioner"/> class.
                /// </summary>
                /// <param name="id">Unique identifier.</param>
                /// <param name="alias">Professional handle.</param>
                /// <param name="details">Personal characteristics and contact data.</param>
                public Practitioner(
                        PractitionerId id,
                        string? alias,
                        PersonDetails details) : base(details: details)
                {
                        this.Id = id;
                        this.Alias = alias;
                }

                /// <summary>
                /// Appends a professional certification.
                /// </summary>
                /// <param name="certificate">The certification record.</param>
                public void AddCertificate(Certificate certificate)
                {

                        if (this._certificates.Exists(c => c.CertificateId == certificate.CertificateId))
                        {
                                throw new InvalidOperationException(message: $"Practitioner already has Certificate {certificate.CertificateId}");
                        }
                        this._certificates.Add(certificate);
                }

                /// <summary>
                /// Revokes a professional certification by its identifier.
                /// </summary>
                /// <param name="id">The certification identifier.</param>
                public void RemoveCertificate(CertificateId id)
                {
                        Certificate? certificate = this._certificates.FirstOrDefault(cert => cert.CertificateId == id);
                        if (certificate is not null)
                        {
                                this._certificates.Remove(certificate);
                        }
                }

                /// <summary>
                /// Updates the display alias.
                /// </summary>
                /// <param name="newAlias">The newly requested string.</param>
                public void UpdateAlias(string newAlias)
                {
                        if (string.IsNullOrWhiteSpace(value: newAlias))
                        {
                                throw new ArgumentException(message: "Alias cannot be empty.", paramName: nameof(newAlias));
                        }

                        this.Alias = newAlias;
                }

                /// <summary>
                /// Updates demographic or contact fields by invoking protected base capabilities.
                /// </summary>
                /// <param name="newDetails">The updated data block.</param>
                public void UpdateDetails(PersonDetails newDetails)
                {
                        ArgumentNullException.ThrowIfNull(argument: newDetails, paramName: nameof(newDetails));

                        this.UpdatePersonDetails(details: newDetails);
                }
        }

}
