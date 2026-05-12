using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Use_Case.BestDiscount
{
        public class DiscountService
        {

                private readonly IBookingRepository _bookingRepository;
                private readonly ITreatmentRepository _treatmentRepository;

                public DiscountService(IBookingRepository bookingRepository, ITreatmentRepository treatmentRepository)
                {
                        ArgumentNullException.ThrowIfNull(bookingRepository);
                        ArgumentNullException.ThrowIfNull(treatmentRepository);

                        this._bookingRepository = bookingRepository;
                        this._treatmentRepository = treatmentRepository;
                }

                public Money CalculateBestDiscount(DiscountContext context)
                {
                        ArgumentNullException.ThrowIfNull(context);

                        context.
                }
        }
}
