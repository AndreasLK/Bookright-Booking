using System.Linq.Expressions;
using Domain.Entities.Persons;

namespace Domain.Specifications.Customers
{
        /// <summary>
        /// Finds customers by exact email match.
        /// Used for duplicate-detection during customer registration.
        /// </summary>
        public class CustomerByEmailSpecification : Specification<Customer>
        {
                private readonly string _email;

                public CustomerByEmailSpecification(string email)
                {
                        this._email = email;
                }

                public override Expression<Func<Customer, bool>> ToExpression()
                {
                        return customer => customer.Email.Value == this._email;
                }
        }
}
