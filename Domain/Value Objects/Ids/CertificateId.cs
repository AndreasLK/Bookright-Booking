namespace Domain.Value_Objects.Ids
{
        public record CertificateId(string Value) : StronglyTypedId<string>(Value);
}
