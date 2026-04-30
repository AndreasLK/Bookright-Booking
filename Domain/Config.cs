namespace Domain
{
        public static class Config
        {

                //General:
                public static readonly TimeSpan LOYALTY_CHECK_LOOKBACK_PERIOD = TimeSpan.FromDays(365);
                public static readonly TimeSpan MAX_BOOKING_DAYS_IN_FUTURE = TimeSpan.FromDays(90);

                //Discounts:
                //Bronze Discount:
                public const decimal BRONZE_LOYALTY_MINIMUM_AMOUNT_PURCHASED = 3000m;
                public const decimal BRONZE_LOYALTY_MAXIMUM_AMOUNT_PURCHASED = 10000m;
                public const decimal BRONZE_LOYALTY_DISCOUNT_MULTIPLIER = 0.95m;

                //Silver Discount:
                public const decimal SILVER_LOYALTY_MINIMUM_AMOUNT_PURCHASED = 10001m;
                public const decimal SILVER_LOYALTY_MAXIMUM_AMOUNT_PURCHASED = 25000m;
                public const decimal SILVER_LOYALTY_DISCOUNT_MULTIPLIER = 0.90m;

                //Gold Discount:
                public const decimal GOLD_LOYALTY_MINIMUM_AMOUNT_PURCHASED = 25001m;
                public const decimal GOLD_LOYALTY_MAXIMUM_AMOUNT_PURCHASED = decimal.MaxValue;
                public const decimal GOLD_LOYALTY_DISCOUNT_MULTIPLIER = 0.85m;

                //Birthmonth Discount:
                public const decimal BIRTHMONTH_DISCOUNT_MULTIPLIER = 0.75m;
                public const int BIRTHMONTH_DISCOUNT_USAGE_LIMIT_PER_BIRTHMONTH = 1;

                public const int PRACTITIONER_MAX_LOCATION_SWITCHES_DAILY = 1;

                //Afternoon and Weekend Surcharge:
                public static readonly TimeOnly SURCHARGE_START_TIME = new TimeOnly(hour: 17, minute: 0);
                public static readonly DayOfWeek[] SURCHARGE_WEEKEND_DAYS = {
                        DayOfWeek.Saturday,
                        DayOfWeek.Sunday
                };
                public const decimal AFTERNOON_AND_WEEKEND_SURCHARGE_MULTIPLIER = 1.15m;


                //Sessions:
                public const int GROUP_SESSION_MAX_PARTICIPANTS = 6;


                //The Boring Stuff:

                public const string EMAIL_VERIFICATION_CHARACHTER = "@";



        }
}
