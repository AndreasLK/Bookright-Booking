using Facade.Customers;
using Microsoft.AspNetCore.Components;

namespace UI.Client.Pages.Customers
{
        /// <summary>
        /// Page component for the receptionist to search and select customers.
        /// </summary>
        public partial class CustomerSearch : ComponentBase
        {
                /// <summary>
                /// Service used to handle routing and URI manipulation.
                /// </summary>
                [Inject]
                protected NavigationManager Navigation { get; set; } = default!;

                /// <summary>
                /// Gets or sets the collection of mocked customers to display in the search list.
                /// </summary>
                protected IEnumerable<CustomerSummaryDto> CustomerSummaries { get; set; } = Array.Empty<CustomerSummaryDto>();

                /// <summary>
                /// Initializes the component with hardcoded mock data for UI testing.
                /// </summary>
                protected override void OnInitialized()
                {
                        this.CustomerSummaries = new List<CustomerSummaryDto>
                        {
                                new CustomerSummaryDto
                                {
                                        Id = Guid.NewGuid(),
                                        LegalFirstName = "Jonathan",
                                        LegalLastName = "Doe",
                                        PreferredFirstName = "Jonny",
                                        Pronouns = "He/Him",
                                        PhoneNumber = "555-0101",
                                        Email = "jonny@example.com",
                                        LoyaltyLevel = "Gold"
                                },
                                new CustomerSummaryDto
                                {
                                        Id = Guid.NewGuid(),
                                        LegalFirstName = "Elizabeth",
                                        LegalLastName = "Windsor",
                                        PreferredFirstName = "Liz",
                                        PreferredLastName = "Mountbatten",
                                        Pronouns = "She/They",
                                        PhoneNumber = "555-0103",
                                        Email = "liz@example.com",
                                        LoyaltyLevel = "Platinum"
                                }
                        };
                }

                /// <summary>
                /// Navigates to the details page for the selected customer.
                /// </summary>
                /// <param name="clickedCustomer">The customer that was clicked.</param>
                protected void GoToCustomerDetails(CustomerSummaryDto clickedCustomer)
                {
                        this.Navigation.NavigateTo(uri: $"/customers/{clickedCustomer.Id}");
                }
        }
}
