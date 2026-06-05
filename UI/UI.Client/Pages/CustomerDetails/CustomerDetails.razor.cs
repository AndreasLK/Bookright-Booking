using Facade.Common.Dtos;
using Facade.Customers;
using Microsoft.AspNetCore.Components;

namespace UI.Client.Pages.CustomerDetails
{
        /// <summary>
        /// Page component for editing a specific customer's detailed profile.
        /// </summary>
        public partial class CustomerDetails : ComponentBase
        {
                /// <summary>
                /// The unique identifier of the customer, extracted from the route.
                /// </summary>
                [Parameter]
                public Guid Id { get; set; }

                [Inject]
                protected CustomerService CustomerService { get; set; } = default!;

                /// <summary>
                /// Service used to handle routing and URI manipulation.
                /// </summary>
                [Inject]
                protected NavigationManager Navigation { get; set; } = default!;

                /// <summary>
                /// The customer data model bound to the form.
                /// </summary>
                protected CustomerDetailsDto? Model { get; set; }

                /// <summary>
                /// Initializes the component and fetches the customer data.
                /// </summary>
                protected override async Task OnInitializedAsync()
                {
                        this.Model = await this.CustomerService.GetCustomerDetailsAsync(id: this.Id);

                        if (this.Model is null)
                        {
                                this.Navigation.NavigateTo(uri: "/not-found");
                        }
                }

                /// <summary>
                /// Handles the valid submission of the form.
                /// </summary>
                /// <param name="updatedModel">The modified customer data.</param>
                protected async Task SaveChangesAsync()
                {
                        if (this.Model is not null)
                        {
                                // We send the DTO back to the Facade to handle the actual Domain updates
                                await this.CustomerService.UpdateCustomerAsync(dto: this.Model);
                                this.Navigation.NavigateTo(uri: "/customers");
                        }
                }

                /// <summary>
                /// Cancels the edit process and returns to the search list.
                /// </summary>
                protected void GoBack()
                {
                        this.Navigation.NavigateTo(uri: "/customers");
                }
        }
}
