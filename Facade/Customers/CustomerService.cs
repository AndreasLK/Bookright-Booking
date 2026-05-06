using Domain.Interfaces.Repositories;

namespace Facade.Customers
{
        /// <summary>
        /// Orchestrates customer-related operations between the UI layer and the Domain layer.
        /// Acts as a strict boundary, ensuring the presentation layer only interacts with DTOs 
        /// and never directly accesses Domain entities or Infrastructure.
        /// </summary>
        public class CustomerService
        {
                private readonly ICustomerRepository _customerRepository;

                /// <summary>
                /// Initializes a new instance of the <see cref="CustomerService"/> class.
                /// </summary>
                /// <param name="customerRepository">The data access implementation for customer entities.</param>
                /// <exception cref="ArgumentNullException">Thrown when the injected repository is null.</exception>
                public CustomerService(ICustomerRepository customerRepository)
                {
                        if (customerRepository is null)
                        {
                                throw new ArgumentNullException(paramName: nameof(customerRepository));
                        }
                        this._customerRepository = customerRepository;
                }


        }
}
