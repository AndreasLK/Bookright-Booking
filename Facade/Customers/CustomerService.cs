using Domain.Interfaces.Repositories;

namespace Facade.Customers
{
        public class CustomerService
        {
                private readonly ICustomerRepository _customerRepository;

                public CustomerService(ICustomerRepository customerRepository)
                {
                        if (customerRepository is null)
                        {
                                throw new ArgumentNullException(paramName: nameof(customerRepository));
                        }
                        this._customerRepository = customerRepository;
                }

                public async Task<IReadOnlyList<CustomerSummaryDto>> SearchCustomerAsync(string searchTherm)
                {
                        if (string.IsNullOrWhiteSpace(value: searchTherm))
                        {
                                throw new ArgumentException(message: "Search term cannot be null or whitespace.", paramName: nameof(searchTherm));
                        }
                }

        }
}
