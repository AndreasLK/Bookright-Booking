namespace Domain.Exceptions
{
        /// <summary>
        /// Thrown when a practitioner is booked at a clinic they are not
        /// assigned to on that working day.
        /// </summary>
        public class PractitionerClinicConflictException : Exception
        {
                public PractitionerClinicConflictException(string message) : base(message) { }
        }
}
