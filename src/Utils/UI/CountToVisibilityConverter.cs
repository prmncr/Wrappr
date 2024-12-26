using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Wrappr.Utils.UI;

public class CountToVisibilityConverter : IValueConverter {
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
		var invert = false;

		if (parameter != null) {
			invert = System.Convert.ToBoolean(parameter);
		}
		var isVisible = System.Convert.ToBoolean(value);
		if (invert) {
			isVisible = !isVisible;
		}

		return isVisible ? Visibility.Visible : Visibility.Collapsed;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
		return Enum.Parse<Visibility>(value?.ToString() ?? throw new InvalidOperationException(), true) == Visibility.Visible;
	}
}