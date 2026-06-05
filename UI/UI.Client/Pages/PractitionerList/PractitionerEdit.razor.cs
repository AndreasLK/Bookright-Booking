using Microsoft.AspNetCore.Components;
using Facade.Practitioners;
using System;
using System.Threading.Tasks;

namespace UI.Client.Pages.PractitionerList
{
        /// <summary>
        /// Page component for editing a practitioner's details.
        /// </summary>
        public partial class PractitionerEdit : ComponentBase
        {
                [Inject] private PractitionerService PractitionerService { get; set; } = default!;
                [Inject] private NavigationManager NavigationManager { get; set; } = default!;

                /// <summary>
                /// The unique identifier extracted from the URL route.
                /// </summary>
                [Parameter]
                public Guid Id { get; set; }

                protected PractitionerSummaryDto? _practitioner;
                protected bool _isLoading = true;

                /// <inheritdoc/>
                protected override async Task OnInitializedAsync()
                {
                        this._practitioner = await this.PractitionerService.GetPractitionerByIdAsync(id: this.Id);
                        this._isLoading = false;
                }

                /// <summary>
                /// Triggered by the GenericEditForm when validation passes.
                /// </summary>
                /// <param name="model">The mutated record from the form.</param>
                protected async Task HandleValidSubmitAsync(PractitionerSummaryDto model)
                {
                        if (model != null)
                        {
                                await this.PractitionerService.UpdatePractitionerAsync(model: model);
                                this.NavigationManager.NavigateTo(uri: "/practitioners");
                        }
                }

                /// <summary>
                /// Triggered by the GenericEditForm cancel button.
                /// </summary>
                protected void GoBack()
                {
                        this.NavigationManager.NavigateTo(uri: "/practitioners");
                }
        }
}
