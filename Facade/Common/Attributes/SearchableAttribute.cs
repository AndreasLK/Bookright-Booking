namespace Facade.Common.Attributes
{
    /// <summary>
    /// Marks a property as searchable to be picked up by the Facade's filtering logic.
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Property)]
    public class SearchableAttribute : Attribute
    {
    }

}
