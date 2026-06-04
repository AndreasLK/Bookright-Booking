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
                private readonly TimeSlot _timeslot;


                public BookingOverlapSpecification(
                        Guid roomId,
                        Guid clinicId,
                        Guid practitionerId,
                        TimeSlot timeslot)

                {
                        this._roomId = roomId;
                        this._clinicId = clinicId;
                        this._practitionerId = practitionerId;
                        this._timeslot = timeslot;

                }


                public override Expression<Func<Booking, bool>> ToExpression()
                {

                        return b =>
                                b.Timeslot.Overlaps(this._timeslot)
                                &&
                                (
                                        b.RoomId.Value == this._roomId ||
                                        b.ClinicId.Value == this._clinicId ||
                                        b.PractitionerId.Value == this._practitionerId
                                );
                }

        }
}
