namespace TreniniApp.Extensions;

public static class ElementExtensions
{
    /// <summary>
    /// Sets the specified <paramref name="property"/> to the given <paramref name="value"/> on the <typeparamref name="TLayout"/> element,
    /// and returns the element for fluent chaining.
    /// </summary>
    /// <typeparam name="TLayout">
    /// The type of the element, which must inherit from <see cref="Element"/> and implement <see cref="IPaddingElement"/>.
    /// </typeparam>
    /// <param name="paddingElement">The element on which to set the property value.</param>
    /// <param name="property">The bindable property to set.</param>
    /// <param name="value">The value to assign to the property.</param>
    /// <returns>The same <typeparamref name="TLayout"/> element, enabling method chaining.</returns>
    public static TLayout Set<TLayout>(
        this TLayout paddingElement,
        BindableProperty property,
        object value
    )
        where TLayout : Element, IPaddingElement
    {
        paddingElement.SetValue(property, value);
        return paddingElement;
    }
}
