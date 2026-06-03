using Microsoft.AspNetCore.Components;

namespace UI.Client.Components.PopUps
{
        /// <summary>
        /// A generic popup component for rapidly searching and selecting an entity in dark mode.
        /// </summary>
        /// <typeparam name="TItem">The type of the entity being searched.</typeparam>
        public partial class QuickSearchPopUp<TItem> : ComponentBase
        {
                public const int MIN_SEARCH_LENGTH = 2;

                [Parameter] public string Title { get; set; } = "Søg";
                [Parameter] public RenderFragment<TItem>? ItemTemplate { get; set; }

                [Parameter] public Func<string, Task<IEnumerable<TItem>>> SearchProvider { get; set; } = default!;
                [Parameter] public EventCallback<TItem> OnItemSelected { get; set; }

                public bool IsOpen { get; private set; }
                public string SearchTerm { get; private set; } = string.Empty;
                public bool IsSearching { get; private set; }
                public IEnumerable<TItem>? Results { get; private set; }

                /// <summary>
                /// Opens the popup and resets internal state.
                /// </summary>
                public void Open()
                {
                        this.IsOpen = true;
                        this.SearchTerm = string.Empty;
                        this.Results = null;
                        this.StateHasChanged();
                }

                /// <summary>
                /// Closes the popup without selection.
                /// </summary>
                public async Task CloseAsync()
                {
                        this.IsOpen = false;
                        await this.InvokeAsync(workItem: this.StateHasChanged);
                }

                /// <summary>
                /// Executes the search using the provided provider delegate.
                /// </summary>
                private async Task ExecuteSearchAsync()
                {
                        if (string.IsNullOrWhiteSpace(value: this.SearchTerm) || this.SearchTerm.Length < MIN_SEARCH_LENGTH)
                        {
                                this.Results = null;
                                return;
                        }

                        this.IsSearching = true;
                        await this.InvokeAsync(workItem: this.StateHasChanged);

                        this.Results = await this.SearchProvider(arg: this.SearchTerm);

                        this.IsSearching = false;
                        await this.InvokeAsync(workItem: this.StateHasChanged);
                }

                /// <summary>
                /// Invokes the selection callback and closes the popup.
                /// </summary>
                /// <param name="item">The selected item.</param>
                private async Task SelectAndCloseAsync(TItem item)
                {
                        if (item == null)
                        {
                                return;
                        }

                        await this.OnItemSelected.InvokeAsync(arg: item);
                        await this.CloseAsync();
                }
        }
}
