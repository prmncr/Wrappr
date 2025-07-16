using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Wrappr.Model;
using Wrappr.Services;
using Wrappr.Utilities;

namespace Wrappr.Components.Pages;

public sealed partial class WrapperSettingsPage : INavigable
{
	public WrapperSettingsPage()
	{
		InitializeComponent();
        Loaded += (_, _) =>
        {
            var wrapper = (DataContext as Wrapper)!;
            ToggleServiceButtonText.Text = wrapper.Enabled ? "Enabled" : "Disabled";
        };
    }

	private void AfterWrapperDeleted(object sender, RoutedEventArgs e)
	{
		Navigation.Back();
	}

	public string LocalizedName => (DataContext as Wrapper)!.ServiceName;

    private async void ServiceStatusButtonClick(object sender, RoutedEventArgs e)
    {
        var button = (sender as ToggleButton)!;
        var wrapper = (DataContext as Wrapper)!;
        await wrapper.ToggleService(!button.IsChecked ?? true);
        button.IsChecked = wrapper.Enabled;
        ToggleServiceButtonText.Text = wrapper.Enabled ? "Enabled" : "Disabled";
    }
}