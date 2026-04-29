
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
namespace Domain.Entities
{
        /// <summary>
        /// Specific variation of a treatment (e.g., 30 min vs 60 min) with unique pricing and duration data
        /// </summary>
        public abstract class Treatment
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

        }
}
