using Microsoft.UI.Xaml;
using Wrappr.Resources;
using Wrappr.Services;
using Wrappr.Utilities;

namespace Wrappr.Components.Pages;

public partial class WrappersListViewerPage : INavigable {
	public WrappersListViewerPage() {
		InitializeComponent();
	}

	public string NavigationTag => nameof(WrappersListViewerPage);

	public string LocalizedName => Strings.WrappersTitle;

	private void WrapperCardClicked(object sender, RoutedEventArgs e) {
		Navigation.ChangePage<WrapperSettingsPage>((sender as FrameworkElement)!.DataContext);
	}

	private void CreateWrapper(object sender, RoutedEventArgs e) {
		Navigation.ChangePage<CreateWrapperPage>();
	}
}