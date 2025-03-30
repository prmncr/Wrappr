using Microsoft.UI.Xaml;
using Wrappr.Services;

namespace Wrappr.Components.Pages;

public sealed partial class WrapperSettingsPage {
	public WrapperSettingsPage() {
		InitializeComponent();
	}

	private void AfterWrapperDeleted(object sender, RoutedEventArgs e) {
		Navigation.Back();
	}
}