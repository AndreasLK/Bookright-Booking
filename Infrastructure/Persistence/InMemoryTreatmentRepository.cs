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


                public Task<Treatment?> GetByIdAsync(Guid id) => Task.FromResult(this._treatments.FirstOrDefault(t => t.Id.Value == id));
                public Task<IReadOnlyList<Treatment>> GetAllAsync() => Task.FromResult<IReadOnlyList<Treatment>>(this._treatments.AsReadOnly());
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
                        // Fysioterapi — three durations
                        this._treatments.Add(new Treatment(
                            id: new TreatmentId(Guid.Parse("F1111111-1111-1111-1111-111111111111")),
                            name: "Fysioterapi 30 min",
                            categoryId: new TreatmentCategoryId(Guid.Parse("A1111111-1111-1111-1111-111111111111")),
                            price: new Money(395, Currency.DKK),
                            duration: new Duration(value: TimeSpan.FromMinutes(30))
                        ));

                        this._treatments.Add(new Treatment(
                            id: new TreatmentId(Guid.Parse("F1111111-1111-1111-1111-111111111112")),
                            name: "Fysioterapi 45 min",
                            categoryId: new TreatmentCategoryId(Guid.Parse("A1111111-1111-1111-1111-111111111111")),
                            price: new Money(589, Currency.DKK),
                            duration: new Duration(value: TimeSpan.FromMinutes(45))
                        ));

                        this._treatments.Add(new Treatment(
                            id: new TreatmentId(Guid.Parse("F1111111-1111-1111-1111-111111111113")),
                            name: "Fysioterapi 60 min",
                            categoryId: new TreatmentCategoryId(Guid.Parse("A1111111-1111-1111-1111-111111111111")),
                            price: new Money(745, Currency.DKK),
                            duration: new Duration(value: TimeSpan.FromMinutes(60))
                        ));

                        // Sportsmassage — two durations
                        this._treatments.Add(new Treatment(
                            id: new TreatmentId(Guid.Parse("F2222222-2222-2222-2222-222222222222")),
                            name: "Sportsmassage 30 min",
                            categoryId: new TreatmentCategoryId(Guid.Parse("A2222222-2222-2222-2222-222222222222")),
                            price: new Money(350, Currency.DKK),
                            duration: new Duration(value: TimeSpan.FromMinutes(30))
                        ));

                        this._treatments.Add(new Treatment(
                            id: new TreatmentId(Guid.Parse("F2222222-2222-2222-2222-222222222223")),
                            name: "Sportsmassage 60 min",
                            categoryId: new TreatmentCategoryId(Guid.Parse("A2222222-2222-2222-2222-222222222222")),
                            price: new Money(699, Currency.DKK),
                            duration: new Duration(value: TimeSpan.FromMinutes(60))
                        ));

                        // Akupunktur — one duration
                        this._treatments.Add(new Treatment(
                            id: new TreatmentId(Guid.Parse("F3333333-3333-3333-3333-333333333333")),
                            name: "Akupunktur 45 min",
                            categoryId: new TreatmentCategoryId(Guid.Parse("A3333333-3333-3333-3333-333333333333")),
                            price: new Money(550, Currency.DKK),
                            duration: new Duration(value: TimeSpan.FromMinutes(45))
                        ));

                        // Kostvejledning — two variations
                        this._treatments.Add(new Treatment(
                            id: new TreatmentId(Guid.Parse("F4444444-4444-4444-4444-444444444444")),
                            name: "Kostvejledning - Førstegangskonsultation 60 min",
                            categoryId: new TreatmentCategoryId(Guid.Parse("A4444444-4444-4444-4444-444444444444")),
                            price: new Money(799, Currency.DKK),
                            duration: new Duration(value: TimeSpan.FromMinutes(60))
                        ));

                        this._treatments.Add(new Treatment(
                            id: new TreatmentId(Guid.Parse("F4444444-4444-4444-4444-444444444445")),
                            name: "Kostvejledning - Opfølgning 30 min",
                            categoryId: new TreatmentCategoryId(Guid.Parse("A4444444-4444-4444-4444-444444444444")),
                            price: new Money(450, Currency.DKK),
                            duration: new Duration(value: TimeSpan.FromMinutes(30))
                        ));

                        // Holdtræning/genoptræning — max 6 deltagere, pris pr. deltager
                        this._treatments.Add(new Treatment(
                            id: new TreatmentId(Guid.Parse("F5555555-5555-5555-5555-555555555555")),
                            name: "Holdtræning/Genoptræning 60 min",
                            categoryId: new TreatmentCategoryId(Guid.Parse("A5555555-5555-5555-5555-555555555555")),
                            price: new Money(150, Currency.DKK),
                            duration: new Duration(value: TimeSpan.FromMinutes(60))
                        ));
                }
        }
}
