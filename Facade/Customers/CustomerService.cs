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


        }
}
