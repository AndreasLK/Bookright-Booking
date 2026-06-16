using Domain;
using Domain.Entities;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using Domain.Interfaces.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Use_Case.BestDiscount
{
        /// <summary>
        /// Orchestrates the data aggregation and calculation processes to determine 
        /// the final optimized price for a customer's booking, including surcharges and discounts.
        /// </summary>
        public class PricingService
        {
                private readonly DiscountContextFactory _discountContextFactory;
                private readonly DiscountService _discountService;
                private readonly IBookingRepository _bookingRepository;
                private readonly ITreatmentRepository _treatmentRepository;

                public PricingService(
                    DiscountContextFactory discountContextFactory,
                    DiscountService discountService,
                    IBookingRepository bookingRepository,
                    ITreatmentRepository treatmentRepository)
                {
                        ArgumentNullException.ThrowIfNull(argument: discountContextFactory, paramName: nameof(discountContextFactory));
                        ArgumentNullException.ThrowIfNull(argument: discountService, paramName: nameof(discountService));
                        ArgumentNullException.ThrowIfNull(argument: bookingRepository, paramName: nameof(bookingRepository));
                        ArgumentNullException.ThrowIfNull(argument: treatmentRepository, paramName: nameof(treatmentRepository));

                        this._discountContextFactory = discountContextFactory;
                        this._discountService = discountService;
                        this._bookingRepository = bookingRepository;
                        this._treatmentRepository = treatmentRepository;
                }

                /// <summary>
                /// Asynchronously calculates the absolute lowest final price available for a specific booking.
                /// </summary>
                /// <param name="customerId">Unique identifier of the customer.</param>
                /// <param name="bookingId">Unique identifier of the current booking.</param>
                /// <returns>Final optimized price after evaluating all eligible promotional campaigns and surcharges.</returns>
                public async Task<Money> GetFinalPriceAsync(CustomerId customerId, BookingId bookingId)
                {
                        Booking? booking = await this._bookingRepository.GetByIdAsync(id: bookingId.Value);

                        if (booking is null)
                        {
                                throw new InvalidOperationException(message: "Booking not found.");
                        }

                        Treatment? treatment = await this._treatmentRepository.GetByIdAsync(id: booking.TreatmentId.Value);

                        if (treatment is null)
                        {
                                throw new InvalidOperationException(message: "Treatment not found.");
                        }

                        Money basePrice = treatment.Price;
                        Money surchargedPrice = this.ApplySurcharges(basePrice: basePrice, timeslot: booking.Timeslot);

                        DiscountContext context = await this._discountContextFactory.CreateAsync(customerId: customerId, bookingId: bookingId);

                        // We use the 'with' expression to non-destructively mutate the init-only record
                        DiscountContext contextWithSurcharge = context with { BasePrice = surchargedPrice };

                        Money finalDiscountedPrice = this._discountService.GetBestDiscount(context: contextWithSurcharge);

                        return finalDiscountedPrice;
                }

                /// <summary>
                /// Applies weekend and evening surcharges based on domain configuration.
                /// </summary>
                private Money ApplySurcharges(Money basePrice, TimeSlot timeslot)
                {
                        bool isWeekend = Config.SURCHARGE_WEEKEND_DAYS.Contains(value: (Domain.Enums.Weekday)timeslot.StartDateTime.DayOfWeek);

                        TimeOnly startTime = TimeOnly.FromDateTime(dateTime: timeslot.StartDateTime);
                        bool isEvening = startTime >= Config.SURCHARGE_START_TIME || startTime <= Config.SURCHARGE_END_TIME;

                        if (isWeekend || isEvening)
                        {
                                decimal surchargedValue = basePrice.Value * Config.AFTERNOON_AND_WEEKEND_SURCHARGE_MULTIPLIER;
                                return new Money(value: surchargedValue, currency: basePrice.Currency);
                        }

                        return basePrice;
                }
        }
}
