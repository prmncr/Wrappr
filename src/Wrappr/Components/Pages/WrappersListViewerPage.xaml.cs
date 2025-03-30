using Microsoft.UI.Xaml;
using Wrappr.Services;
using Wrappr.Utilities;

namespace Wrappr.Components.Pages;

public partial class WrappersListViewerPage : INavigable {
	public WrappersListViewerPage() {
		InitializeComponent();
	}

	public static string NavigationTag => nameof(WrappersListViewerPage);

	private void WrapperCardClicked(object sender, RoutedEventArgs e) {
		Navigation.ChangePage<WrapperSettingsPage>();
		Navigation.CurrentPage!.DataContext = (sender as FrameworkElement)!.DataContext;
	}

	private void CreateWrapper(object sender, RoutedEventArgs e) {
		Navigation.ChangePage<CreateWrapperPage>();
	}
}