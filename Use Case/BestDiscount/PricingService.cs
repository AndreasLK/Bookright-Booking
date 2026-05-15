using Domain.Value_Objects;
using Domain.Value_Objects.Ids;


namespace Use_Case.BestDiscount
{
        /// <summary>
        /// Orchestrates the data aggregation and calculation processes to determine 
        /// the final optimized price for a customer's booking.
        /// </summary>
        public class PricingService
        {
                private DiscountContextFactory _discountContextFactory;
                private DiscountService _discountService;

                /// <summary>
                /// Initializes a new instance of the <see cref="PricingService"/> class.
                /// </summary>
                /// <param name="discountContextFactory">Factory to assemble the required contextual data.</param>
                /// <param name="discountService">Service to evaluate strategies and determine the lowest price.</param>
                /// <exception cref="ArgumentNullException">Thrown if any dependency is null.</exception>
                public PricingService(DiscountContextFactory discountContextFactory, DiscountService discountService)
                {
                        ArgumentNullException.ThrowIfNull(discountContextFactory);
                        ArgumentNullException.ThrowIfNull(discountService);

                        this._discountContextFactory = discountContextFactory;
                        this._discountService = discountService;
                }

                /// <summary>
                /// Asynchronously calculates the absolute lowest final price available for a specific booking.
                /// </summary>
                /// <param name="customerId">Unique identifier of the customer.</param>
                /// <param name="bookingId">Unique identifier of the current booking.</param>
                /// <returns>Final optimized price after evaluating all eligible promotional campaigns.</returns>
                public async Task<Money> GetBestPriceAsync(CustomerId customerId, BookingId bookingId)
                {
                        DiscountContext context = await this._discountContextFactory.CreateAsync(customerId, bookingId);
                        Money discountedPrice = this._discountService.GetBestDiscount(context: context);


                        return discountedPrice;
                }
        }
}
