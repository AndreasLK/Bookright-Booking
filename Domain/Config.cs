using Domain.Enums;
using Domain.Value_Objects;

namespace Domain
{
        public static class Config
        {

                //General:

                public static readonly TimeSpan MAX_BOOKING_DAYS_IN_FUTURE = TimeSpan.FromDays(90);
                public const int PRACTITIONER_MAX_LOCATION_SWITCHES_DAILY = 1;
                public static readonly Currency DEFAULT_CURRENCY = Currency.DKK;

                //Discounts:

                public static readonly TimeSpan LOYALTY_CHECK_LOOKBACK_PERIOD = TimeSpan.FromDays(365);


                //Bronze Discount:
                public static readonly Money BRONZE_LOYALTY_MINIMUM_AMOUNT_PURCHASED = new Money(value: 3000m, currency: Enums.Currency.DKK);
                public const decimal BRONZE_LOYALTY_DISCOUNT_MULTIPLIER = 0.95m;

                //Silver Discount:
                public static readonly Money SILVER_LOYALTY_MINIMUM_AMOUNT_PURCHASED = new Money(value: 10001m, currency: Enums.Currency.DKK);
                public const decimal SILVER_LOYALTY_DISCOUNT_MULTIPLIER = 0.90m;

                //Gold Discount:
                public static readonly Money GOLD_LOYALTY_MINIMUM_AMOUNT_PURCHASED = new Money(value: 25001m, currency: Enums.Currency.DKK);
                public const decimal GOLD_LOYALTY_DISCOUNT_MULTIPLIER = 0.85m;

                //Birthmonth Discount:
                public const decimal BIRTHMONTH_DISCOUNT_MULTIPLIER = 0.75m;
                public const int BIRTHMONTH_DISCOUNT_USAGE_LIMIT = 1;
                public static readonly TimeSpan BIRTHMONTH_DISCOUNT_COOLDOWN_PERIOD = TimeSpan.FromDays(335); //About 11 months, to prevent abuse while still allowing customers to use the discount in their birthmonth every year


                //Campaigns:
                public const int DEFAULT_CAMPAIGN_USAGE_LIMIT = 1;
                public static readonly TimeSpan DEFAULT_CAMPAIGN_COOLDOWN_PERIOD = TimeSpan.FromDays(31);



                //Afternoon and Weekend Surcharge:
                public static readonly TimeOnly SURCHARGE_START_TIME = new TimeOnly(hour: 17, minute: 0);
                public static readonly TimeOnly SURCHARGE_END_TIME = new TimeOnly(hour: 5, minute: 0);
                public static readonly Weekday[] SURCHARGE_WEEKEND_DAYS = {
                        Weekday.Saturday,
                        Weekday.Sunday,
                        Weekday.Holiday
                };
                public const decimal AFTERNOON_AND_WEEKEND_SURCHARGE_MULTIPLIER = 1.15m;


                //Sessions:
                public const int GROUP_SESSION_MAX_PARTICIPANTS = 6;


                //The Boring Stuff:

                public const string EMAIL_VERIFICATION_CHARACHTER = "@";

                public const int DIGITS_BEFORE_DOT_IN_MONEY = 18;
                public const int DIGITS_AFTER_DOT_IN_MONEY = 2;

        }
}
