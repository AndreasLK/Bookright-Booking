using Microsoft.AspNetCore.Components;
using Facade.Practitioners;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UI.Client.Pages.PractitionerList
{
        /// <summary>
        /// Page component for searching and listing practitioners.
        /// </summary>
        public partial class PractitionerSearch : ComponentBase
        {
                [Inject] private PractitionerService PractitionerService { get; set; } = default!;
                [Inject] private NavigationManager NavigationManager { get; set; } = default!;

                protected IEnumerable<PractitionerSummaryDto>? _practitioners;
                protected bool _isLoading = true;

                /// <inheritdoc/>
                protected override async Task OnInitializedAsync()
                {
                        this._practitioners = await this.PractitionerService.GetPractitionersAsync();
                        this._isLoading = false;
                }

                /// <summary>
                /// Navigates to the edit page for the selected record.
                /// </summary>
                /// <param name="practitioner">The user-selected object.</param>
                protected void HandlePractitionerSelected(PractitionerSummaryDto practitioner)
                {
                        if (practitioner == null)
                        {
                                return;
                        }

                        this.NavigationManager.NavigateTo(uri: $"/practitioners/{practitioner.Id}");
                }
        }
}
