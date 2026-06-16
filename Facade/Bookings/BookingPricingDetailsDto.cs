using System.Collections.Generic;

namespace Facade.Bookings
{
        /// <summary>
        /// A pure data transfer object representing the itemized receipt of a booking.
        /// </summary>
        public record BookingPricingDetailsDto(
            decimal BasePrice,
            decimal SurchargeAmount,
            string SurchargeReason,
            decimal DiscountAmount,
            string DiscountReason,
            List<string> EvaluatedDiscounts,
            decimal FinalPrice
        );
}
