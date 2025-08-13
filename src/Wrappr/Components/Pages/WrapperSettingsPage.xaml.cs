using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Wrappr.Model;
using Wrappr.Resources;
using Wrappr.Services;
using Wrappr.Utilities;

namespace Wrappr.Components.Pages;

public partial class WrapperSettingsPage : INavigable
{
	public WrapperSettingsPage()
	{
		InitializeComponent();
		Loaded += (_, _) =>
		{
			var wrapper = (DataContext as Wrapper)!;
			SetToggleServiceButtonText(wrapper.Enabled);
		};
	}

	private void AfterWrapperDeleted(object sender, RoutedEventArgs e)
	{
		Navigation.Back();
	}

	public string LocalizedName => (DataContext as Wrapper)!.ServiceName;

	private void ServiceStatusButtonClick(object sender, RoutedEventArgs e)
	{
		var button = (sender as ToggleButton)!;
		var wrapper = (DataContext as Wrapper)!;
		wrapper.Enabled = button.IsChecked ?? false;
		button.IsChecked = wrapper.Enabled;
		SetToggleServiceButtonText(wrapper.Enabled);
	}

	private void SetToggleServiceButtonText(bool isEnabled)
	{
		ToggleServiceButtonText.Text = isEnabled ? Strings.ServiceToggleButtonEnabled : Strings.ServiceToggleButtonDisabled;
	}
}