using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Domain.Entities
{
        public class Campaign
        {
                public CampaignId Id { get; private set; }

                public string Name { get; private set; }

                public string Description { get; private set; }

                public DateOnly StartDate { get; private set; }

                public DateOnly EndDate { get; private set; }
                public Strategies.DiscountStrategy Strategy { get; private set; }

                public Campaign(
                        CampaignId id,
                        string name,
                        string description,
                        DateOnly startDate,
                        DateOnly endDate,
                        Strategies.DiscountStrategy strategy)
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


                        this.Id = id;
                        this.Name = name;
                        this.Description = description;
                        this.StartDate = startDate;
                        this.EndDate = endDate;
                        this.Strategy = strategy;
                }




        }
}
