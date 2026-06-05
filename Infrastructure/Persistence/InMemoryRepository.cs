using System.Collections.Concurrent;
using Domain.Interfaces.Repositories;
using Domain.Specifications;

namespace Infrastructure.Persistence
{
        /// <summary>
        /// Generic in-memory implementation of IRepository&lt;T&gt;.
        /// Used for development and testing without a real database.
        /// Concrete subclasses must implement how to extract the entity's ID.
        /// </summary>
        public abstract class InMemoryRepository<T> : IRepository<T> where T : class
        {
                protected readonly ConcurrentDictionary<Guid, T> Store = new();

                public Task<T?> GetByIdAsync(Guid id)
                {
                        this.Store.TryGetValue(id, out var entity);
                        return Task.FromResult(entity);
                }

                public Task<IReadOnlyList<T>> GetAllAsync()
                {
                        IReadOnlyList<T> all = this.Store.Values.ToList();
                        return Task.FromResult(all);
                }

                public Task<IReadOnlyList<T>> FindAsync(Specification<T> specification)
                {
                        // Compile the expression tree into an executable function
                        var predicate = specification.ToExpression().Compile();

                        IEnumerable<T> query = this.Store.Values.Where(predicate);

                        // Honor the specification's sorting (if any)
                        if (specification.OrderBy is not null)
                        {
                                query = query.OrderBy(specification.OrderBy.Compile());
                        }
                        else if (specification.OrderByDescending is not null)
                        {
                                query = query.OrderByDescending(specification.OrderByDescending.Compile());
                        }

                        // Honor the specification's Take limit (if any)
                        if (specification.Take.HasValue)
                        {
                                query = query.Take(specification.Take.Value);
                        }

                        IReadOnlyList<T> result = query.ToList();
                        return Task.FromResult(result);
                }

                // Abstract because we don't know how to get the ID out of T at this level
                public abstract Task<T> AddAsync(T entity);

                public abstract Task UpdateAsync(T entity);

                public Task<bool> DeleteAsync(Guid id)
                {
                        var removed = this.Store.TryRemove(id, out _);
                        return Task.FromResult(removed);
                }
        }
}
