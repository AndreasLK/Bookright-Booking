using Domain.Entities.People;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces.Repositories
{
        /// <summary>
        /// Specific data access contract for Customer entities.
        /// Handles specialized queries not covered by the generic repository.
        /// </summary>
        public interface ICustomerRepository : IRepository<Customer>
        {
        }
}
