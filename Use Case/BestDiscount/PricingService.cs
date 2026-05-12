using Domain.Value_Objects;
using Domain.Value_Objects.Ids;


namespace Use_Case.BestDiscount
{
        public class PricingService
        {
                private DiscountContextFactory _discountContextFactory;
                private DiscountService _discountService;

                public PricingService(DiscountContextFactory discountContextFactory, DiscountService discountService)
                {
                        ArgumentNullException.ThrowIfNull(discountContextFactory);
                        ArgumentNullException.ThrowIfNull(discountService);

                        this._discountContextFactory = discountContextFactory;
                        this._discountService = discountService;
                }

                public async Task<Money> GetBestPriceAsync(CustomerId customerId, BookingId bookingId)
                {
                        DiscountContext context = await this._discountContextFactory.CreateAsync(customerId, bookingId);
                        Money discountedPrice = this._discountService.GetBestDiscount(context: context);


                        return discountedPrice;
                }
        }
}
