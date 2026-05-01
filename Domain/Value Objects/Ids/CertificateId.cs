namespace Domain.Value_Objects.Ids
{
        /// <summary>Unique identifier for a Certificate.</summary>
        public record CertificateId(string Value) : StronglyTypedId<string>(Value);
}
