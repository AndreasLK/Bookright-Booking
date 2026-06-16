using System;
using Domain.Enums;
using Domain.Value_Objects.Ids;

namespace Domain.Entities
{
        /// <summary>
        /// Represents a category of treatments and its associated authorization requirements.
        /// </summary>
        public class TreatmentCategory
        {
                /// <summary>
                /// Unique identifier for the category.
                /// </summary>
                public TreatmentCategoryId Id { get; private set; }

                /// <summary>
                /// Name of the category (e.g., Fysioterapi).
                /// </summary>
                public string Name { get; private set; }

                /// <summary>
                /// Required authorization level for this category.
                /// </summary>
                public AuthorizationType Authorization { get; private set; }

                /// <summary>
                /// Initializes a new instance of the <see cref="TreatmentCategory"/> class.
                /// </summary>
                public TreatmentCategory(TreatmentCategoryId id, string name, AuthorizationType authorization)
                {
                        ArgumentNullException.ThrowIfNull(argument: id, paramName: nameof(id));
                        ArgumentNullException.ThrowIfNull(argument: name, paramName: nameof(name));

                        if (string.IsNullOrWhiteSpace(value: name))
                        {
                                throw new ArgumentException(message: "Name cannot be empty or whitespace.", paramName: nameof(name));
                        }

                        this.Id = id;
                        this.Name = name;
                        this.Authorization = authorization;
                }
        }
}
