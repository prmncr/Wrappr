using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace Wrappr.Controls;

public partial class Toggle
{
	public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
		nameof(Value), typeof(bool), typeof(Toggle), new PropertyMetadata(false)
	);

	public static readonly DependencyProperty OnCheckedCommandProperty = DependencyProperty.Register(
		nameof(OnCheckedCommand), typeof(ICommand), typeof(Toggle), new PropertyMetadata(null)
	);

	public Toggle()
	{
		InitializeComponent();
	}

	public bool Value
	{
		get => (bool)GetValue(ValueProperty);
		set => SetValue(ValueProperty, value);
	}

	public ICommand? OnCheckedCommand
	{
		get => (ICommand)GetValue(OnCheckedCommandProperty);
		set => SetValue(OnCheckedCommandProperty, value);
	}

	private void Toggled(object sender, RoutedEventArgs e)
	{
		OnCheckedCommand?.Execute(OutputToggleSwitch.IsOn);
	}

	private void Ghost_OnPointerEntered(object sender, PointerRoutedEventArgs e)
	{
		VisualStateManager.GoToState(OutputToggleSwitch, "PointerOver", true);
	}

	private void Ghost_OnPointerExited(object sender, PointerRoutedEventArgs e)
	{
		VisualStateManager.GoToState(OutputToggleSwitch, "Normal", true);
	}

	private void Ghost_OnPointerPressed(object sender, PointerRoutedEventArgs e)
	{
		VisualStateManager.GoToState(OutputToggleSwitch, "Pressed", true);
	}

	private void Ghost_OnPointerReleased(object sender, PointerRoutedEventArgs e)
	{
		VisualStateManager.GoToState(OutputToggleSwitch, "Normal", true);
	}
}