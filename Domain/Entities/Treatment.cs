
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
namespace Domain.Entities
{
        /// <summary>
        /// Specific variation of a treatment (e.g., 30 min vs 60 min) with unique pricing and duration data
        /// </summary>
        public class Treatment
        {
                /// <summary>
                /// Unique identifier for this specific variation.
                /// </summary>
                public TreatmentId Id { get; init; }

                /// <summary>
                /// Service title Excluding duration (like 'Fysioterapi').
                /// </summary>
                public string Name { get; private set; }

                /// <summary>
                /// ID of TreatmentCategory (like Physiotherapi) to ensure a certified practitioner
                /// </summary>
                public TreatmentCategoryId CategoryId { get; private set; }

                /// <summary>
                /// Price of Treatment including 'Moms' before discounts or surcharges
                /// </summary>
                public Money Price { get; private set; }

                /// <summary>
                /// Duration of Treatment
                /// </summary>
                public Duration Duration { get; private set; }

                public Treatment(
                    TreatmentId id,
                    string name,
                    TreatmentCategoryId categoryId,
                    Money price,
                    Duration duration)
                {
                        ArgumentNullException.ThrowIfNull(id, nameof(id));
                        ArgumentNullException.ThrowIfNull(name, nameof(name));

                        if (string.IsNullOrWhiteSpace(name))
                        {
                                throw new ArgumentException("Name cannot be empty or whitespace.", nameof(name));
                        }

                        ArgumentNullException.ThrowIfNull(categoryId, nameof(categoryId));
                        ArgumentNullException.ThrowIfNull(price, nameof(price));
                        ArgumentNullException.ThrowIfNull(duration, nameof(duration));

                        this.Id = id;
                        this.Name = name;
                        this.CategoryId = categoryId;
                        this.Price = price;
                        this.Duration = duration;
                }
        }
}
