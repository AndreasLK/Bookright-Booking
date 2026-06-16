using Domain.Entities;
using Domain.Value_Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Use_Case.BestDiscount
{
        /// <summary>
        /// Represents the outcome of the discount evaluation process.
        /// </summary>
        public record DiscountEvaluationResult(
            Money FinalPrice,
            string AppliedCampaignName,
            List<string> EvaluatedCampaignNames
        );

        /// <summary>
        /// Evaluates all eligible campaigns and determines the absolute best discount for the customer.
        /// </summary>
        public class DiscountService
        {
                /// <summary>
                /// Iterates through active campaigns, applies their specific strategies, and selects the one resulting in the lowest price.
                /// </summary>
                public DiscountEvaluationResult GetBestDiscount(DiscountContext context)
                {
                        ArgumentNullException.ThrowIfNull(argument: context, paramName: nameof(context));

                        Money bestPrice = context.BasePrice;
                        string bestCampaignName = "Ingen";
                        List<string> evaluatedCampaigns = new List<string>();

                        if (context.ActiveCampaigns is null || !context.ActiveCampaigns.Any())
                        {
                                return new DiscountEvaluationResult(
                                    FinalPrice: bestPrice,
                                    AppliedCampaignName: bestCampaignName,
                                    EvaluatedCampaignNames: evaluatedCampaigns
                                );
                        }

                        foreach (Campaign campaign in context.ActiveCampaigns)
                        {
                                bool isEligible = true;

                                // Enforce campaign cooldown periods
                                if (context.TimeUsedEligbleCampaigns.TryGetValue(key: campaign.Id, value: out List<DateTime>? usageDates))
                                {
                                        if (usageDates is not null && usageDates.Any())
                                        {
                                                DateTime lastUsed = usageDates.Max();
                                                if ((DateTime.Now - lastUsed) < campaign.Cooldown)
                                                {
                                                        isEligible = false;
                                                }
                                        }
                                }

                                if (isEligible)
                                {
                                        evaluatedCampaigns.Add(item: campaign.Name);

                                        Money calculatedPrice = campaign.Strategy.GetFinalPrice(
                                            totalPurchase: context.TotalHistoricalSpend,
                                            currentPurchasePrice: context.BasePrice,
                                            treatmentId: context.TreatmentId,
                                            customerBirthMonth: context.CustomerBirthMonth,
                                            timesUsedCampaign: usageDates ?? new List<DateTime>()
                                        );

                                        if (calculatedPrice.Value < bestPrice.Value)
                                        {
                                                bestPrice = calculatedPrice;
                                                bestCampaignName = campaign.Name;
                                        }
                                }
                        }

                        return new DiscountEvaluationResult(
                            FinalPrice: bestPrice,
                            AppliedCampaignName: bestCampaignName,
                            EvaluatedCampaignNames: evaluatedCampaigns
                        );
                }
        }
}
