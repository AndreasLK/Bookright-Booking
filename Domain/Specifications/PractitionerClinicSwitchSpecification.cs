using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Domain.Entities;

namespace Domain.Specifications
{
        
        /// Finds bookings that prove a practitioner is already assigned to a
        /// different clinic on the requested day.
        /// Used to enforce the rule that practitioners cannot switch clinics
        /// within the same working day.
       
        public class PractitionerClinicSwitchSpecification : Specification<Booking>
        {
                private readonly Guid _practitionerId;
                private readonly Guid _intendedClinicId;
                private readonly DateTime _dayStart;
                private readonly DateTime _dayEnd;

                public PractitionerClinicSwitchSpecification(
                        Guid practitionerId,
                        Guid intendedClinicId,
                        DateOnly date)
                {
                        this._practitionerId = practitionerId;
                        this._intendedClinicId = this._intendedClinicId;

                        // Defins "the day" as midnight to midnight.
                        this._dayStart = date.ToDateTime(TimeOnly.MinValue);
                        this._dayEnd = date.ToDateTime(TimeOnly.MaxValue);
                }

                public override Expression<Func<Booking, bool>> ToExpression()
                {
                        return b =>
                                // same practitioner
                                b.PractitionerId.Value == this._practitionerId
                                &&
                                //Already booked at differient clinic
                                b.ClinicId.Value == this._intendedClinicId
                                &&
                                //On the same day
                                b.Timeslot.StartDateTime >= this._dayStart &&
                                b.Timeslot.StartDateTime <= this._dayEnd;
                }


        }
}
