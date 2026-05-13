using Domain.Interfaces;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Domain.Entities
{
        /// <summary>
        /// Defines a promotional campaign, including its active period, usage restrictions, 
        /// and the specific discount strategy it applies.
        /// </summary>
        public class Campaign
        {
                /// <summary>
                /// Unique identifier of the campaign.
                /// </summary>
                public CampaignId Id { get; private set; }

                /// <summary>
                /// Display name of the campaign.
                /// </summary>
                public string Name { get; private set; }

                /// <summary>
                /// Detailed explanation of the campaign's terms and purpose.
                /// </summary>
                public string Description { get; private set; }

                /// <summary>
                /// Timestamp indicating when the campaign becomes active and available for use.
                /// </summary>
                public DateTime StartDate { get; private set; }

                /// <summary>
                /// Timestamp indicating when the campaign expires and is no longer valid.
                /// </summary>
                public DateTime EndDate { get; private set; }

                /// <summary>
                /// Minimum required time interval between consecutive uses of this campaign by the same customer.
                /// </summary>
                public TimeSpan Cooldown { get; private set; }

                /// <summary>
                /// Algorithmic strategy used to calculate the discount for this campaign.
                /// </summary>
                public IDiscountStrategy Strategy { get; private set; }

                /// <summary>
                /// Initializes a new instance of the <see cref="Campaign"/> class.
                /// </summary>
                /// <param name="id">Unique identifier of the campaign.</param>
                /// <param name="name">Display name of the campaign.</param>
                /// <param name="description">Detailed explanation of the campaign.</param>
                /// <param name="startDate">Activation timestamp.</param>
                /// <param name="endDate">Expiration timestamp.</param>
                /// <param name="strategy">Discount calculation strategy.</param>
                /// <param name="cooldown">Required rest period between uses.</param>
                /// <exception cref="ArgumentNullException">Thrown if any required parameter is null.</exception>
                /// <exception cref="ArgumentException">Thrown if strings are blank, end date is before start date, or cooldown is negative.</exception>
                public Campaign(
                        CampaignId id,
                        string name,
                        string description,
                        DateTime startDate,
                        DateTime endDate,
                        IDiscountStrategy strategy,
                        TimeSpan cooldown)
                {

                        ArgumentNullException.ThrowIfNull(argument: id, paramName: nameof(id));
                        ArgumentNullException.ThrowIfNull(argument: name, paramName: nameof(name));
                        ArgumentNullException.ThrowIfNull(argument: description, paramName: nameof(description));
                        ArgumentNullException.ThrowIfNull(argument: strategy, paramName: nameof(strategy));


                        if (string.IsNullOrWhiteSpace(name))
                        {
                                throw new ArgumentException(
                                        message: "Campaign name cannot be blank",
                                        paramName: nameof(name));
                        }
                        if (string.IsNullOrWhiteSpace(description))
                        {
                                throw new ArgumentException(
                                        message: "Campaign description cannot be blank",
                                        paramName: nameof(description));
                        }
                        if (endDate < startDate)
                        {
                                throw new ArgumentException(
                                        message: "End date cannot be before start date",
                                        paramName: nameof(endDate));
                        }
                        if (cooldown < TimeSpan.Zero)
                        {
                                throw new ArgumentException(
                                        message: "Cooldown cannot be negative",
                                        paramName: nameof(cooldown));
                        }

                        this.Id = id;
                        this.Name = name;
                        this.Description = description;
                        this.StartDate = startDate;
                        this.EndDate = endDate;
                        this.Strategy = strategy;
                        this.Cooldown = cooldown;
                }
        }
}
