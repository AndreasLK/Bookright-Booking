using Domain.Value_Objects.Ids;

namespace Domain.Entities.Persons
{
        /// <inheritdoc />
        public class Receptionist : Person
        {
                public ReceptionistId Id { get; init; }
        }
}
