using Microsoft.AspNetCore.Components;
using UI.Client.Extensions;

namespace UI.Client.Components.SearchableDataList
{
        /// <summary>
        /// A generic list component that provides real-time search filtering and double-click selection.
        /// </summary>
        /// <typeparam name="TItem">The type of items displayed in the list.</typeparam>
        public partial class SearchableDataList<TItem> : ComponentBase
        {
                /// <summary>
                /// The complete collection of items to be displayed and filtered.
                /// </summary>
                [Parameter]
                public IEnumerable<TItem> Items { get; set; } = Array.Empty<TItem>();

                /// <summary>
                /// The visual template used to render each individual item in the list.
                /// </summary>
                [Parameter]
                [EditorRequired]
                public RenderFragment<TItem> ItemTemplate { get; set; } = default!;

                /// <summary>
                /// The callback invoked when a user double-clicks an item.
                /// </summary>
                [Parameter]
                public EventCallback<TItem> OnItemDoubleClick { get; set; }

                /// <summary>
                /// Gets or sets the current search term entered by the user.
                /// </summary>
                protected string SearchTerm { get; set; } = string.Empty;

                /// <summary>
                /// Gets the dynamically filtered list of items based on the current search term.
                /// Utilizes the custom ApplySearch extension method.
                /// </summary>
                protected IEnumerable<TItem> FilteredItems => this.Items.ApplySearch(searchTerm: this.SearchTerm);

                /// <summary>
                /// Handles the double-click event on an item and triggers the callback.
                /// </summary>
                /// <param name="item">The item that was double-clicked.</param>
                protected async Task HandleDoubleClickAsync(TItem item)
                {
                        if (this.OnItemDoubleClick.HasDelegate)
                        {
                                await this.OnItemDoubleClick.InvokeAsync(arg: item);
                        }
                }
        }
}
