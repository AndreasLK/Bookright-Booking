using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;

namespace UI.Client.Components.EnumDropdown
{
        /// <summary>
        /// A generic dropdown component that automatically populates options from an Enum type.
        /// Utilizes EnumExtensions to display human-readable names prioritizing the [Display] attribute.
        /// </summary>
        /// <typeparam name="TEnum">The struct Enum type to generate options for.</typeparam>
        public partial class EnumDropdown<TEnum> : ComponentBase where TEnum : struct, Enum
        {
                /// <summary>
                /// The current bound value of the dropdown.
                /// </summary>
                [Parameter]
                public TEnum Value { get; set; }

                /// <summary>
                /// The callback delegate invoked when the user selects a new value.
                /// </summary>
                [Parameter]
                public EventCallback<TEnum> ValueChanged { get; set; }

                /// <summary>
                /// The expression identifying the bound value for Blazor form validation purposes.
                /// </summary>
                [Parameter]
                public Expression<Func<TEnum>> ValueExpression { get; set; } = default!;

                /// <summary>
                /// The CSS class applied to the select element.
                /// </summary>
                [Parameter]
                public string CssClass { get; set; } = "form-select";

                /// <summary>
                /// Indicates whether to show a default, unselected placeholder option.
                /// </summary>
                [Parameter]
                public bool ShowDefaultOption { get; set; } = true;

                /// <summary>
                /// The text displayed for the default placeholder option.
                /// </summary>
                [Parameter]
                public string DefaultOptionText { get; set; } = "Select an option...";
        }
}
