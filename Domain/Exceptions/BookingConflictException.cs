namespace Domain.Exceptions
{
        /// <summary>
        /// Thrown when a new booking would cause a scheduling conflict
        /// with an existing booking for a shared resource (room, clinic, or practitioner).
        /// </summary>
        public class BookingConflictException : Exception
        {
                public BookingConflictException(string message) : base(message) { }
        }
}
