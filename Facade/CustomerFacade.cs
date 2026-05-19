using System;
using System.Collections.Generic;
using System.Text;
using UseCase.Customers;

namespace Facade
{
        /* Single entrypoint for UI layer to perform Customer related operations.
                 Delegates to the approciate UseCase for each operation
                 */
        public class CustomerFacade
        {
                private readonly RegisterCustomerUseCase _registerCustomer;

                public CustomerFacade(RegisterCustomerUseCase registerCustomer)
                {
                        this._registerCustomer = registerCustomer;
                }

                //Register new customer into the system

                public Task<RegisterCustomerResult> RegisterCustomerAsync(
                        RegisterCustomerCommand command)
                {
                        return this._registerCustomer.ExecuteAsync(command);
                }


        }

}
