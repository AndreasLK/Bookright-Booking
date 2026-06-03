using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Domain.Entities;
using Domain.Value_Objects;

namespace Domain.Specifications
{
        public class BookingOverlapSpecification : Specification<Booking>
        {
                private readonly Guid _roomId;
                private readonly Guid _clinicId;
                private readonly Guid _practitionerId;
                private readonly Guid _customerId;
                private readonly DateTime _start;
                private readonly DateTime _end;


                public BookingOverlapSpecification(
                        Guid roomId,
                        Guid clinicId,
                        Guid practitionerId,
                        Guid customerId,
                        TimeSlot timeslot)

                {
                        this._roomId = roomId;
                        this._clinicId = clinicId;
                        this._practitionerId = practitionerId;
                        this._customerId = customerId;
                        this._start = timeslot.StartDateTime;
                        this._end = timeslot.EndDateTime;

                }


                public override Expression<Func<Booking, bool>> ToExpression()
                {
                        TimeSlot søgtPeriode = new TimeSlot(this._start, this._end);

                        return b =>
                                b.Timeslot.Overlaps(søgtPeriode)
                                &&
                                (
                                        b.RoomId.Value == this._roomId ||
                                        b.ClinicId.Value == this._clinicId ||
                                        b.PractitionerId.Value == this._practitionerId ||
                                        b.CustomerId.Value == this._customerId
                                );
                }

        }
}
