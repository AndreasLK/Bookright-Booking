using System.Reflection;
using Facade.Common.Attributes;

namespace UI.Client.Extensions
{
        /// <summary>
        /// Provides search filtering capabilities for collections using the [Searchable] attribute.
        /// </summary>
        public static class SearchExtensions
        {
                /// <summary>
                /// Filters a collection by matching a search string against searchable properties.
                /// </summary>
                /// <typeparam name="TItem">The type of items in the collection.</typeparam>
                /// <param name="source">The collection to filter.</param>
                /// <param name="searchTerm">The space-separated string of words to search for.</param>
                /// <returns>A filtered collection matching all search words.</returns>
                public static IEnumerable<TItem> ApplySearch<TItem>(this IEnumerable<TItem> source, string searchTerm)
                {
                        // Guard clause (Unhappy path first)
                        if (string.IsNullOrWhiteSpace(value: searchTerm))
                        {
                                return source;
                        }

                        string[] searchWords = searchTerm.Split(
                            separator: ' ',
                            options: StringSplitOptions.RemoveEmptyEntries);

                        PropertyInfo[] searchableProperties = typeof(TItem)
                            .GetProperties()
                            .Where(predicate: prop => Attribute.IsDefined(
                                element: prop,
                                attributeType: typeof(SearchableAttribute)))
                            .ToArray();

                        // Extracted logic to avoid Level-4 nesting
                        return source.Where(predicate: item => MatchesSearchTerms(
                            item: item,
                            searchWords: searchWords,
                            searchableProperties: searchableProperties));
                }

                /// <summary>
                /// Evaluates if a single item matches all the provided search words.
                /// </summary>
                private static bool MatchesSearchTerms<TItem>(TItem item, string[] searchWords, PropertyInfo[] searchableProperties)
                {
                        IEnumerable<string> itemValues = searchableProperties
                            .Select(selector: prop => prop.GetValue(obj: item)?.ToString() ?? string.Empty);

                        return searchWords.All(predicate: word =>
                            itemValues.Any(predicate: value =>
                                value.Contains(value: word, comparisonType: StringComparison.OrdinalIgnoreCase)));
                }
        }
}
