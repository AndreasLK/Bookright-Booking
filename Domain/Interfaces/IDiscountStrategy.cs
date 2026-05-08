using System;
using System.Collections.Generic;
using System.Text;
using Domain.Strategies;
using Domain.Value_Objects;


namespace Domain.Interfaces
{
        public interface IDiscountStrategy

        {
                public Money GetFinalPrice(Money totalPurchase, Money currentPurchasePrice);
        }
}
