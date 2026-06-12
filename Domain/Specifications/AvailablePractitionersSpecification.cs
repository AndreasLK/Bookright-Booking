using Domain.Entities.Persons;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Domain.Specifications
{
        /// <summary>
        /// Specification to retrieve practitioners who do not have a conflicting booking during the requested timeslot.
        /// </summary>
        public class AvailablePractitionersSpecification : Specification<Practitioner>
        {
                private readonly IReadOnlyList<Guid> _bookedPractitionerIds;

                public AvailablePractitionersSpecification(IEnumerable<Guid> bookedPractitionerIds)
                {
                        this._bookedPractitionerIds = bookedPractitionerIds?.ToList() ?? new List<Guid>();
                }

                public override Expression<Func<Practitioner, bool>> ToExpression()
                {
                        // If the practitioner's ID is NOT in the booked list, they are available
                        return practitioner => !this._bookedPractitionerIds.Contains(practitioner.Id.Value);
                }
        }
}
