using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;
using Domain.Strategies;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;


namespace Domain.Interfaces
{
        public interface IDiscountStrategy

        {
                public Money GetFinalPrice(Money totalPurchase, Money currentPurchasePrice, TreatmentId treatmentId, Month customerBirthMonth, List<DateTime> timesUsedCampaign);
        }
}
