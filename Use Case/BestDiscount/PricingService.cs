using Domain;
using Domain.Entities;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Use_Case.BestDiscount
{
        /// <summary>
        /// A comprehensive breakdown of how a final price was calculated.
        /// </summary>
        public record PricingBreakdown(
            Money BasePrice,
            Money SurchargeAmount,
            string SurchargeReason,
            Money DiscountAmount,
            string DiscountReason,
            List<string> EvaluatedDiscounts,
            Money FinalPrice
        );

        /// <summary>
        /// Orchestrates the data aggregation and calculation processes to determine 
        /// the final optimized price and its itemized breakdown.
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

                public async Task<PricingBreakdown> GetFinalPriceAsync(CustomerId customerId, BookingId bookingId)
                {
                        Booking? booking = await this._bookingRepository.GetByIdAsync(id: bookingId.Value);
                        if (booking is null) throw new InvalidOperationException(message: "Booking not found.");

                        Treatment? treatment = await this._treatmentRepository.GetByIdAsync(id: booking.TreatmentId.Value);
                        if (treatment is null) throw new InvalidOperationException(message: "Treatment not found.");

                        DiscountContext context = await this._discountContextFactory.CreateAsync(customerId: customerId, bookingId: bookingId);

                        return this.CalculateBreakdown(basePrice: treatment.Price, timeslot: booking.Timeslot, context: context);
                }

                public async Task<PricingBreakdown> GetPreviewPriceAsync(CustomerId customerId, TreatmentId treatmentId, TimeSlot timeslot)
                {
                        Treatment? treatment = await this._treatmentRepository.GetByIdAsync(id: treatmentId.Value);
                        if (treatment is null) throw new InvalidOperationException(message: "Treatment not found.");

                        DiscountContext context = await this._discountContextFactory.CreatePreviewAsync(
                            customerId: customerId,
                            treatmentId: treatmentId,
                            timeslot: timeslot);

                        return this.CalculateBreakdown(basePrice: treatment.Price, timeslot: timeslot, context: context);
                }

                /// <summary>
                /// Executes the math to determine the exact monetary value of surcharges and discounts.
                /// </summary>
                private PricingBreakdown CalculateBreakdown(Money basePrice, TimeSlot timeslot, DiscountContext context)
                {
                        // 1. Calculate Surcharges
                        Money surchargedPrice = this.ApplySurcharges(basePrice: basePrice, timeslot: timeslot, out string surchargeReason);
                        Money surchargeAmount = new Money(value: surchargedPrice.Value - basePrice.Value, currency: basePrice.Currency);

                        // 2. Calculate Discounts
                        DiscountContext contextWithSurcharge = context with { BasePrice = surchargedPrice };
                        DiscountEvaluationResult discountResult = this._discountService.GetBestDiscount(context: contextWithSurcharge);
                        Money discountAmount = new Money(value: surchargedPrice.Value - discountResult.FinalPrice.Value, currency: basePrice.Currency);

                        return new PricingBreakdown(
                            BasePrice: basePrice,
                            SurchargeAmount: surchargeAmount,
                            SurchargeReason: surchargeReason,
                            DiscountAmount: discountAmount,
                            DiscountReason: discountResult.AppliedCampaignName,
                            EvaluatedDiscounts: discountResult.EvaluatedCampaignNames,
                            FinalPrice: discountResult.FinalPrice
                        );
                }

                private Money ApplySurcharges(Money basePrice, TimeSlot timeslot, out string reason)
                {
                        bool isWeekend = Config.SURCHARGE_WEEKEND_DAYS.Contains(value: (Domain.Enums.Weekday)timeslot.StartDateTime.DayOfWeek);
                        TimeOnly startTime = TimeOnly.FromDateTime(dateTime: timeslot.StartDateTime);
                        bool isEvening = startTime >= Config.SURCHARGE_START_TIME || startTime <= Config.SURCHARGE_END_TIME;

                        if (isWeekend || isEvening)
                        {
                                reason = isWeekend ? "Weekend-tillæg" : "Aften-tillæg";
                                decimal surchargedValue = basePrice.Value * Config.AFTERNOON_AND_WEEKEND_SURCHARGE_MULTIPLIER;
                                return new Money(value: surchargedValue, currency: basePrice.Currency);
                        }

                        reason = "Ingen";
                        return basePrice;
                }
        }
}
