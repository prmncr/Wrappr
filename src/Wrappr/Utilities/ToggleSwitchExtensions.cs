using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Wrappr.Utilities;

public class ToggleSwitchExtensions
{
	public static readonly DependencyProperty ToggledCommandProperty =
		DependencyProperty.Register(
			"ToggledCommand",
			typeof(ICommand),
			typeof(ToggleSwitchExtensions),
			new PropertyMetadata(null, OnToggledCommandChanged));

	public static void SetToggledCommand(UIElement element, ICommand value) => element.SetValue(ToggledCommandProperty, value);

	public static ICommand GetToggledCommand(UIElement element) => (ICommand)element.GetValue(ToggledCommandProperty);

	private static void OnToggledCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not ToggleSwitch toggleSwitch) return;
		toggleSwitch.PointerReleased -= ToggleSwitch_Toggled;

		if (e.NewValue != null)
		{
			toggleSwitch.PointerReleased += ToggleSwitch_Toggled;
		}
	}

	private static void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
	{
		if (sender is not ToggleSwitch toggleSwitch) return;
		var command = GetToggledCommand(toggleSwitch);
		if (command.CanExecute(toggleSwitch.IsOn))
		{
			command.Execute(toggleSwitch.IsOn);
		}
	}
}