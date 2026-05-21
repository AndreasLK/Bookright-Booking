namespace Facade.Common.Dtos
{
        /// <summary>
        /// Lightweight representation of a practitioner for selection lists.
        /// </summary>
        /// 
        public class PractitionerLookupDto
        {
                /// <summary>
                /// Unique identifier.
                /// </summary>
                public Guid Id { get; set; }

                /// <summary>
                /// Formatted name for display purposes.
                /// </summary>
                public string DisplayName { get; set; }

        }
}
