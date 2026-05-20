using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Specifications;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Infrastructure.Persistence
{
        public class InMemoryTreatmentRepository : ITreatmentRepository
        {
                private readonly List<Treatment> _treatments = new();

                public InMemoryTreatmentRepository()
                {
                        this.SeedData();
                }

                // --- IRepository Methods ---
                public Task<Treatment?> GetByIdAsync(Guid id) => Task.FromResult(this._treatments.FirstOrDefault(t => t.Id.Value == id));
                public Task<IReadOnlyList<Treatment>> GetAllAsync() => Task.FromResult<IReadOnlyList<Treatment>>(_treatments.AsReadOnly());
                public Task<Treatment> AddAsync(Treatment entity) { this._treatments.Add(entity); return Task.FromResult(entity); }
                public Task UpdateAsync(Treatment entity)
                {
                        int index = this._treatments.FindIndex(t => t.Id.Value == entity.Id.Value);
                        if (index != -1) this._treatments[index] = entity;
                        return Task.CompletedTask;
                }
                public Task<bool> DeleteAsync(Guid id) => Task.FromResult(this._treatments.RemoveAll(t => t.Id.Value == id) > 0);

                public Task<IReadOnlyList<Treatment>> FindAsync(Specification<Treatment> specification)
                {
                        var query = this._treatments.AsQueryable().Where(specification.ToExpression());
                        if (specification.OrderBy != null) query = query.OrderBy(specification.OrderBy);
                        else if (specification.OrderByDescending != null) query = query.OrderByDescending(specification.OrderByDescending);
                        if (specification.Skip.HasValue) query = query.Skip(specification.Skip.Value);
                        if (specification.Take.HasValue) query = query.Take(specification.Take.Value);
                        return Task.FromResult<IReadOnlyList<Treatment>>(query.ToList().AsReadOnly());
                }

                private void SeedData()
                {
                        // Assuming you add a constructor to the Treatment class matching these parameters
                        this._treatments.Add(new Treatment(
                            id: new TreatmentId(Guid.Parse("T1111111-1111-1111-1111-111111111111")),
                            name: "Deep Tissue Massage",
                            categoryId: new TreatmentCategoryId(Guid.NewGuid()),
                            price: new Money(600, Currency.DKK),
                            duration: new Duration(value: TimeSpan.FromMinutes(60))
                        ));

                        this._treatments.Add(new Treatment(
                            id: new TreatmentId(Guid.Parse("T2222222-2222-2222-2222-222222222222")),
                            name: "Standard Physiotherapy",
                            categoryId: new TreatmentCategoryId(Guid.NewGuid()),
                            price: new Money(450, Currency.DKK),
                            duration: new Duration(value: TimeSpan.FromMinutes(45))
                        ));
                }
        }
}
