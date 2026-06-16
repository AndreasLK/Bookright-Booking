using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Facade.Common.Attributes;

namespace UI.Client.Components.SearchableDataList
{
        public partial class SearchableDataList<TItem> : ComponentBase
        {
                [Parameter] public IEnumerable<TItem> Items { get; set; } = Array.Empty<TItem>();
                [Parameter] public RenderFragment<TItem> ItemTemplate { get; set; } = default!;

                [Parameter] public EventCallback<TItem> OnItemClick { get; set; }
                [Parameter] public EventCallback<TItem> OnItemDoubleClick { get; set; }

                public string SearchTerm { get; set; } = string.Empty;

                protected IEnumerable<TItem> FilteredItems
                {
                        get
                        {
                                if (string.IsNullOrWhiteSpace(value: this.SearchTerm))
                                {
                                        return this.Items;
                                }

                                var properties = typeof(TItem).GetProperties()
                                    .Where(predicate: prop => Attribute.IsDefined(element: prop, attributeType: typeof(SearchableAttribute)))
                                    .ToArray();

                                return this.Items.Where(predicate: item => this.EvaluatesToTrueForSearchTerm(item: item, properties: properties));
                        }
                }

                private bool EvaluatesToTrueForSearchTerm(TItem item, PropertyInfo[] properties)
                {
                        if (item is null)
                        {
                                return false;
                        }

                        if (properties is null || properties.Length == 0)
                        {
                                string? stringValue = item.ToString();
                                return stringValue is not null && stringValue.Contains(value: this.SearchTerm, comparisonType: StringComparison.OrdinalIgnoreCase);
                        }

                        foreach (PropertyInfo prop in properties)
                        {
                                object? val = prop.GetValue(obj: item);

                                if (val is null)
                                {
                                        continue;
                                }

                                if (val.ToString()!.Contains(value: this.SearchTerm, comparisonType: StringComparison.OrdinalIgnoreCase))
                                {
                                        return true;
                                }
                        }

                        return false;
                }

                protected async Task HandleItemClickAsync(TItem item)
                {
                        if (this.OnItemClick.HasDelegate)
                        {
                                await this.OnItemClick.InvokeAsync(arg: item);
                        }
                }

                protected async Task HandleItemDoubleClickAsync(TItem item)
                {
                        if (this.OnItemDoubleClick.HasDelegate)
                        {
                                await this.OnItemDoubleClick.InvokeAsync(arg: item);
                        }
                }
        }
}
