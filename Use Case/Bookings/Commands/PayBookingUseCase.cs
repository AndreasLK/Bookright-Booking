using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using System;
using System.Threading.Tasks;
using Use_Case.BestDiscount;

namespace Use_Case.Bookings.Commands
{
        public class PayBookingUseCase
        {
                private readonly IBookingRepository _bookingRepository;
                private readonly PricingService _pricingService;

                public PayBookingUseCase(IBookingRepository bookingRepository, PricingService pricingService)
                {
                        ArgumentNullException.ThrowIfNull(argument: bookingRepository, paramName: nameof(bookingRepository));
                        ArgumentNullException.ThrowIfNull(argument: pricingService, paramName: nameof(pricingService));

                        this._bookingRepository = bookingRepository;
                        this._pricingService = pricingService;
                }

                public async Task ExecuteAsync(Guid bookingIdRaw, Currency currency)
                {
                        BookingId bookingId = new BookingId(Value: bookingIdRaw);
                        Booking? booking = await this._bookingRepository.GetByIdAsync(id: bookingId.Value);

                        if (booking is null) throw new InvalidOperationException(message: "Booking could not be found.");
                        if (booking.Paid is not null) throw new InvalidOperationException(message: "Booking is already paid.");

                        if (currency == Currency.JYD)
                        {
                                await this._bookingRepository.DeleteAsync(id: bookingId.Value);
                                return;
                        }

                        PricingBreakdown breakdown = await this._pricingService.GetFinalPriceAsync(
                            customerId: booking.CustomerId,
                            bookingId: booking.Id
                        );

                        booking.RegisterPayment(payment: breakdown.FinalPrice);
                        await this._bookingRepository.UpdateAsync(entity: booking);
                }
        }
}
