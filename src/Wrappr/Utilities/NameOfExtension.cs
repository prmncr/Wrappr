using Microsoft.UI.Xaml.Markup;

namespace Wrappr.Utilities;

[ContentProperty(Name = nameof(Value))]
public partial class NameOfExtension : MarkupExtension
{
	public Type Type { get; set; } = typeof(object);

	public string Value { get; private set; } = nameof(Object);

	protected override object ProvideValue()
	{
		if (Type == null)
			throw new ArgumentException("Syntax for x:NameOf is Type={x:Type [className]}");
		Value = Type.Name;
		return Type.Name;
	}
}