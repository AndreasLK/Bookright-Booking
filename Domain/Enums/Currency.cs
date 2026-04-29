namespace Domain.Enums
{
        /// <summary>
        /// Specifies the supported currency types available for financial transactions, 
        /// balance calculations, and exchange rate conversions within the system.
        /// </summary>
        public enum Currency
        {
                /// <summary>
                /// Jutlandic Dollars (Jyske Dollars). A regional currency unit.
                /// </summary>
                JYD,

                /// <summary>
                /// Danish Krone. The official currency of Denmark.
                /// </summary>
                DKK,

                /// <summary>
                /// Euro. The official currency of the Eurozone.
                /// </summary>
                EUR,

                /// <summary>
                /// Bitcoin. A decentralized digital cryptocurrency.
                /// </summary>
                BTC,

                /// <summary>
                /// Trump Currency. A domain-specific currency unit used within the application logic.
                /// </summary>
                Trump

        }
}
