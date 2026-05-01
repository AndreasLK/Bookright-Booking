using System.ComponentModel.DataAnnotations;

namespace Domain.Value_Objects
{
        public record PhoneNumber
        {
                public string Value { get; init; }

                public PhoneNumber(string value)
                {
                        if (value is null)
                        {
                                throw new ArgumentNullException(paramName: nameof(value));
                        }
                        if (string.IsNullOrWhiteSpace(value: value))
                        {
                                throw new ArgumentException(
                                        message: "Phone number cannot be empty or consist only of white space.",
                                        paramName: nameof(value));
                        }

                        this.Value = value;
                }
        }
}
