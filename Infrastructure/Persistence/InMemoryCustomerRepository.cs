using Domain.Entities.Persons;
using Domain.Interfaces.Repositories;

namespace Infrastructure.Persistence
{
        public class InMemoryCustomerRepository : InMemoryRepository<Customer>, ICustomerRepository
        {
                public override Task<Customer> AddAsync(Customer entity)
                {
                        this.Store[entity.Id.Value] = entity;
                        return Task.FromResult(entity);
                }

                public override Task UpdateAsync(Customer entity)
                {
                        this.Store[entity.Id.Value] = entity;
                        return Task.CompletedTask;
                }
        }
}
