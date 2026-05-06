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
                        // Simulate fetching the specific customer from the Facade layer
                        await Task.Delay(millisecondsDelay: 300);

                        this.Model = new CustomerDetailsDto
                        {
                                Id = this.Id,
                                LegalFirstName = "Jonathan",
                                LegalLastName = "Doe",
                                PreferredFirstName = "Jonny",
                                Pronouns = "He/Him",
                                Gender = GenderDto.Man,
                                Email = "jonny@example.com",
                                PhoneNumber = "555-0101",
                                DateOfBirth = new DateOnly(year: 1990, month: 5, day: 14),
                                LoyaltyLevel = "Gold", // This comes from the backend calculation
                                SygsikringDanmarkMember = true,
                                ImportantNote = "Allergic to certain massage oils."
                        };
                }

                /// <summary>
                /// Handles the valid submission of the form.
                /// </summary>
                /// <param name="updatedModel">The modified customer data.</param>
                protected async Task SaveChangesAsync(CustomerDetailsDto updatedModel)
                {
                        // Here you would call your Facade command:
                        // await this.CustomerFacade.UpdateCustomerAsync(updatedModel);

                        Console.WriteLine(value: $"Saved changes for {updatedModel.GreetingName}");
                        this.Navigation.NavigateTo(uri: "/test-customers");
                }

                /// <summary>
                /// Cancels the edit process and returns to the search list.
                /// </summary>
                protected void GoBack()
                {
                        this.Navigation.NavigateTo(uri: "/test-customers");
                }
        }
}
