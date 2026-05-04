using Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces.Repositories
{
        /// <summary>
        /// A complete Data Access Contract. 
        /// Handles both reading (Queries) and writing (Commands) for any Domain Entity.
        /// </summary>
        /// <typeparam name="T">The type of Entity, must inherit from <see cref="Entity"/>.</typeparam>
        public interface IRepository<T>
        {
                //Read Operations


                /// <summary>
                /// Retrieves a single record by its unique database identifier.
                /// </summary>
                /// <param name="id">The Guid of the entity to find.</param>
                /// <returns>The matching entity if found; otherwise, null.</returns>
                Task<T?> GetByIdAsync(Guid id);

                /// <summary>
                /// Fetches every record of this type from the database. 
                /// </summary>
                /// <remarks>
                /// Caution: Use this only for small tables (like a list of clinics). 
                /// For large datasets (like thousands of bookings), use <see cref="FindAsync"/> instead.
                /// </remarks>
                /// <returns>A Read-only list of all entities.</returns>
                Task<IReadOnlyList<T>> GetAllAsync();


                /// <summary>
                /// Executes a 'Smart Search' using the Specification Pattern.
                /// This translates the C# search rule directly into SQL, ensuring that 
                /// the database only returns the rows we actually need.
                /// </summary>
                /// <param name="specification">The business rule defining the search criteria.</param>
                /// <returns>A filtered Read-only list of entities matching the rule.</returns>
                Task<IReadOnlyList<T>> FindAsync(Specification<T> specification);


                //Write Operations


                /// <summary>Persists a new entity to the data store.</summary>
                /// <returns>The created entity with its assigned ID.</returns>
                Task<T> AddAsync(T entity);


                /// <summary>Updates an existing entity's data.</summary>
                Task UpdateAsync(T entity);


                ///<summary>
                /// Safely removes a record by its unique ID. 
                /// This prevents accidental bulk deletions.
                /// </summary>
                /// <param name="id">The unique identifier of the record to delete.</param>
                /// <returns>True if the record was found and removed; otherwise, false.</returns>
                Task<bool> DeleteAsync(Guid id);
        }
}
