using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Use_Case
{
        public static class BestDiscountCalculator
        {
                public static Money CalculateBestDiscount(
                        CustomerId customerId,
                        BookingId bookingId,
                        IBookingRepository bookingRepository,
                        ITreatmentRepository treatmentRepository,
                        Campaign[] campaigns,
                        ICurrencyConverter currencyConverter)
                {
                        ArgumentNullException.ThrowIfNull(customerId);
                        ArgumentNullException.ThrowIfNull(bookingId);
                        ArgumentNullException.ThrowIfNull(bookingRepository);
                        ArgumentNullException.ThrowIfNull(campaigns);
                        ArgumentNullException.ThrowIfNull(currencyConverter);


                        Booking? booking = bookingRepository.GetByIdAsync(bookingId.Value).Result;
                        if (booking is null)
                        {
                                throw new ArgumentException(message: $"Booking with id {bookingId} not found.");
                        }

                        List<Money> PaidBookingHistory = bookingRepository.FindAsync(booking => booking.CustomerId == customerId).Result.Select(b => b.Paid); //SOME LINQ HERE

                        foreach (Money value in PaidBookingHistory)
                        {
                                currencyConverter.Convert(de)
                        }


                        if (bookingPrice is null)
                        {
                                throw new ArgumentException(message: $"Price for treatment with id {booking.TreatmentId} not found.");
                        }

                }
        }
}
