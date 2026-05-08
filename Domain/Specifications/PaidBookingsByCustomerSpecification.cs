using Domain.Entities;
using Domain.Value_Objects.Ids;

namespace Domain.Specifications
{
        public class PaidBookingsByCustomerSpecification
        {
                private readonly CustomerId _customerId;

                public PaidBookingsByCustomerSpecification(CustomerId customerId)
                {
                        ArgumentNullException.ThrowIfNull(argument: customerId, nameof(customerId));
                        this._customerId = customerId;
                }




        }
}
