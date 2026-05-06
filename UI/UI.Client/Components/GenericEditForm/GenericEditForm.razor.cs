using Microsoft.AspNetCore.Components;

namespace UI.Client.Components.GenericEditForm
{
        /// <summary>
        /// A generic form component that wraps an EditForm with standardized styling and validation.
        /// </summary>
        /// <typeparam name="TItem">The type of the model being edited.</typeparam>
        public partial class GenericEditForm<TItem> : ComponentBase
        {
                /// <summary>
                /// The title displayed in the card header.
                /// </summary>
                [Parameter]
                public string Title { get; set; } = "Edit Record";

                /// <summary>
                /// The model instance being edited.
                /// </summary>
                [Parameter]
                [EditorRequired]
                public TItem Model { get; set; } = default!;

                /// <summary>
                /// The render fragment containing the form fields to display.
                /// </summary>
                [Parameter]
                [EditorRequired]
                public RenderFragment<TItem> FormFields { get; set; } = default!;

                /// <summary>
                /// Callback invoked when the form is valid and submitted.
                /// </summary>
                [Parameter]
                public EventCallback<TItem> OnSave { get; set; }

                /// <summary>
                /// Callback invoked when the cancel button is clicked.
                /// </summary>
                [Parameter]
                public EventCallback OnCancel { get; set; }

                /// <summary>
                /// Handles the valid submit event and triggers the OnSave callback.
                /// </summary>
                protected async Task HandleValidSubmitAsync()
                {
                        await this.OnSave.InvokeAsync(arg: this.Model);
                }

                /// <summary>
                /// Handles the cancel event and triggers the OnCancel callback.
                /// </summary>
                protected async Task HandleCancelAsync()
                {
                        await this.OnCancel.InvokeAsync();
                }
        }
}
