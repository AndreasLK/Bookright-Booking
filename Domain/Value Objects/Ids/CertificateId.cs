namespace Domain.Value_Objects.Ids
{
        public record CertificateId
        {
                public string Value { get; init; }

                public CertificateId(string value)
                {
                        if (string.IsNullOrWhiteSpace(value))
                        {
                                throw new ArgumentException(
                                        message: "ID must not be empty",
                                        paramName: nameof(value));
                        }
                        this.Value = value;

                }
        }
}
