using Facade.Customers;
using Microsoft.AspNetCore.Components;

namespace UI.Client.Pages.Customers
{
        /// <summary>
        /// Page component for the receptionist to search and select customers.
        /// </summary>
        public partial class CustomerSearch : ComponentBase
        {
                [Inject]
                protected CustomerService CustomerService { get; set; } = default!;

                [Inject]
                protected NavigationManager Navigation { get; set; } = default!;

                // We strictly hold DTOs in the UI state
                protected IEnumerable<CustomerSummaryDto> CustomerSummaries { get; set; } = Array.Empty<CustomerSummaryDto>();

                protected override async Task OnInitializedAsync()
                {
                        // The UI asks the Facade for data, and receives secure DTOs back
                        this.CustomerSummaries = await this.CustomerService.SearchCustomersAsync();
                }

                protected void GoToCustomerDetails(CustomerSummaryDto clickedCustomer)
                {
                        this.Navigation.NavigateTo(uri: $"/customers/{clickedCustomer.Id}");
                }
        }
}

