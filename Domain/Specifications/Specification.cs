using System.Linq.Expressions;

namespace Domain.Specifications
{
        /// <summary>
        /// Base contract for query criteria and database-level sorting.
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        public abstract class Specification<T>
        {
                /// <summary>
                /// Filter criteria expression.
                /// </summary>
                /// <returns>A piece of C# code that acts as a filter like:( x => x.Id == 5).</returns>
                public abstract Expression<Func<T, bool>> ToExpression();


                /// <summary>
                /// Ascending sort criteria.
                /// </summary>
                public Expression<Func<T, object>>? OrderBy { get; private set; }


                /// <summary>
                /// Descending sort criteria.
                /// </summary>
                public Expression<Func<T, object>>? OrderByDescending { get; private set; }


                /// <summary>
                /// Sets primary ascending sort.
                /// </summary>
                protected void ApplyOrderBy(
                    Expression<Func<T, object>> orderByExpression
                    ) => this.OrderBy = orderByExpression;


                /// <summary>
                /// Sets primary descending sort.
                /// </summary>
                protected void ApplyOrderByDescending(
                    Expression<Func<T, object>> orderByDescendingExpression
                    ) => this.OrderByDescending = orderByDescendingExpression;


                /// <summary>
                /// Maximum records to retrieve.
                /// </summary>
                public int? Take { get; private set; }


                /// <summary>
                /// Restricts result set to specified count.
                /// </summary>
                protected void ApplyTake(int count) => this.Take = count;
        }
}
