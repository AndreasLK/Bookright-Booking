namespace Facade.Common.Validation
{
    public static class ValidationRunner
    {
        /// <summary>
        /// Runs a validation block. Only catches errors the user can fix.
        /// </summary>
        public static async Task<ValidationResult> ExecuteAsync(Func<Task> action)
        {
            try
            {
                await action.Invoke();

                return new ValidationResult(IsValid: true);
            }
            catch (ArgumentException ex)
            {
                return new ValidationResult(IsValid: false, ErrorMessage: ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return new ValidationResult(IsValid: false, ErrorMessage: ex.Message);
            }
        }
    }
}
