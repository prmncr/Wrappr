using Microsoft.UI.Xaml;
using Wrappr.Model;
using Wrappr.Services;
using Wrappr.Utilities;

namespace Wrappr.Components.Pages;

public sealed partial class WrapperSettingsPage : INavigable {
	public WrapperSettingsPage() {
		InitializeComponent();
	}

	private void AfterWrapperDeleted(object sender, RoutedEventArgs e) {
		Navigation.Back();
	}

	public string NavigationTag => nameof(WrapperSettingsPage);
	public static string TypeNavigationTag => nameof(WrapperSettingsPage);
	public string LocalizedName => (DataContext as Wrapper)!.ServiceName;
}