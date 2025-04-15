using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Wrappr.Data;
using Wrappr.Services;
using Wrappr.Utilities;

namespace Wrappr.Components.Pages;

public sealed partial class NavigationRoot : Navigation.INavigator, Snackbars.ISnackbarViewport {
	private readonly Dictionary<string, Type> _roots = new INavigable.RootInfoBuilder()
		.Add<WrappersListViewerPage>()
		.Add<WrapperGroupsListViewerPage>()
		.Add<SettingsPage>();

	public NavigationRoot() {
		InitializeComponent();

		Navigation.Initialize(this);
		Snackbars.Initialize(this);

		NavigationView.BackRequested += (_, _) => Navigation.Back();
	}

	public bool ChangePage(Type pageType, out Page? page) {
		var changed = NavigationFrame.Navigate(pageType);
		page = changed ? (Page)NavigationFrame.Content : null;
		return changed;
	}

	public void Back() {
		if (NavigationFrame.CanGoBack) {
			NavigationFrame.GoBack();
		}
	}

	public void Clear() {
		NavigationFrame.BackStack.Clear();
		NavigationFrame.ForwardStack.Clear();
	}

	public void ShowInfoBarMessage(SnackbarData message) {
		InfoBar.IsOpen = true;
		InfoBar.Severity = message.Severity;
		InfoBar.Title = message.Title;
		InfoBar.Message = message.Message;
	}

	private void OnLoaded(object sender, RoutedEventArgs e) {
		var window = ApplicationWindowProvider.Window;
		window.ExtendsContentIntoTitleBar = true;
		window.SetTitleBar(AppTitleBar);
		window.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
		RightPaddingColumn.Width = new GridLength(window.AppWindow.TitleBar.RightInset / AppTitleBar.XamlRoot.RasterizationScale);
	}

	private void OnRootNavigationSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) {
		if (args.IsSettingsSelected) {
			Navigation.NewRoot<SettingsPage>();
			return;
		}
		if (((NavigationViewItem)args.SelectedItem).Tag is not string tag)
			return;
		Navigation.NewRoot(_roots[tag]);
	}
}