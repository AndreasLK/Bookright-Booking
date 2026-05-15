using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;
using Domain.Strategies;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;


namespace Domain.Interfaces
{
        /// <summary>
        /// Contract for evaluating and calculating discounted treatment prices based on campaign rules and customer history.
        /// </summary>
        public interface IDiscountStrategy

        {
                /// <summary>
                /// Calculates the final discounted price for a treatment.
                /// </summary>
                /// <param name="totalPurchase">Normalized total historical spend.</param>
                /// <param name="currentPurchasePrice">Normalized base price of the treatment.</param>
                /// <param name="treatmentId">Unique identifier of the treatment.</param>
                /// <param name="customerBirthMonth">Optional birth month of the customer.</param>
                /// <param name="timesUsedCampaign">Historical usage dates of this campaign.</param>
                /// <returns>Final calculated price after applying the discount strategy.</returns>
                public Money GetFinalPrice(Money totalPurchase, Money currentPurchasePrice, TreatmentId treatmentId, Month? customerBirthMonth, List<DateTime> timesUsedCampaign);
        }
}
