using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Wrappr.Data;
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

	private async void ServiceStatusButtonClick(object sender, RoutedEventArgs e)
	{
		try
		{
			var button = (sender as ToggleButton)!;
			var wrapper = (DataContext as Wrapper)!;
			await wrapper.ToggleService(!button.IsChecked ?? true);
			button.IsChecked = wrapper.Enabled;
			SetToggleServiceButtonText(wrapper.Enabled);
		} catch (Exception ex)
		{
			Snackbars.ShowSnackbar(new SnackbarData(Strings.ErrorMessageTitle, InfoBarSeverity.Error, ex.Message));
		}
	}

	private void SetToggleServiceButtonText(bool isEnabled)
	{
		ToggleServiceButtonText.Text = isEnabled ? Strings.ServiceToggleButtonEnabled : Strings.ServiceToggleButtonDisabled;
	}
}