using Domain.Entities.Persons;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Domain.Specifications.Practitioners
{
        /// <summary>
        /// Specification to search practitioners by their legal name, preferred name, or alias.
        /// </summary>
        public class PractitionerSearchSpecification : Specification<Practitioner>
        {
                private readonly string _searchTerm;

                public PractitionerSearchSpecification(string searchTerm)
                {
                        this._searchTerm = searchTerm.ToLower();
                }

                public override Expression<Func<Practitioner, bool>> ToExpression()
                {
                        return p =>
                            p.LegalFirstName.ToLower().Contains(this._searchTerm) ||
                            p.LegalLastName.ToLower().Contains(this._searchTerm) ||
                            (p.PreferredFirstName != null && p.PreferredFirstName.ToLower().Contains(this._searchTerm)) ||
                            (p.PreferredLastName != null && p.PreferredLastName.ToLower().Contains(this._searchTerm)) ||
                            (p.Alias != null && p.Alias.ToLower().Contains(this._searchTerm));
                }
        }
}
