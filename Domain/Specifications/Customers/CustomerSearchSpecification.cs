using Domain.Entities.Persons;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Domain.Specifications.Customers
{
        /// <summary>
        /// Specification to search customers by their legal name, preferred name, or email.
        /// </summary>
        public class CustomerSearchSpecification : Specification<Customer>
        {
                private readonly string _searchTerm;

                public CustomerSearchSpecification(string searchTerm)
                {
                        this._searchTerm = searchTerm.ToLower();
                }

                public override Expression<Func<Customer, bool>> ToExpression()
                {
                        return c =>
                            c.LegalFirstName.ToLower().Contains(this._searchTerm) ||
                            c.LegalLastName.ToLower().Contains(this._searchTerm) ||
                            (c.PreferredFirstName != null && c.PreferredFirstName.ToLower().Contains(this._searchTerm)) ||
                            (c.PreferredLastName != null && c.PreferredLastName.ToLower().Contains(this._searchTerm)) ||
                            c.Email.Value.ToLower().Contains(this._searchTerm);
                }
        }
}
