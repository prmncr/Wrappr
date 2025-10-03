using Microsoft.UI.Xaml;

namespace Wrappr.Data;

public class WrapperSettings : DependencyObject
{
	public static readonly DependencyProperty TrackedProperty = DependencyProperty.Register(
		nameof(Tracked), typeof(bool), typeof(WrapperSettings), new PropertyMetadata(false)
	);

	public bool Tracked
	{
		get => (bool)GetValue(TrackedProperty);
		set => SetValue(TrackedProperty, value);
	}

	public static readonly DependencyProperty NotifiedProperty = DependencyProperty.Register(
		nameof(Notified), typeof(bool), typeof(WrapperSettings), new PropertyMetadata(false)
	);

	public bool Notified
	{
		get => (bool)GetValue(NotifiedProperty);
		set => SetValue(NotifiedProperty, value);
	}

	public static readonly DependencyProperty PollingDelayProperty = DependencyProperty.Register(
		nameof(PollingDelay), typeof(int), typeof(WrapperSettings), new PropertyMetadata(0)
	);

	public int PollingDelay
	{
		get => (int)GetValue(PollingDelayProperty);
		set => SetValue(PollingDelayProperty, value);
	}
}