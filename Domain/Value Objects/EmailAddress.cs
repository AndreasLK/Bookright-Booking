namespace Domain.Value_Objects
{
        public record EmailAddress
        {
                public string Email;
                EmailAddress(string Email)
                {
                        if (!Email.Contains(Config.EMAIL_VERIFICATION_CHARACHTER))
                        {
                                throw new ArgumentException(
                                        message: $"Email must contain {Config.EMAIL_VERIFICATION_CHARACHTER}",
                                        paramName: nameof(Email));
                        }
                        this.Email = Email;
                }
        }
}
