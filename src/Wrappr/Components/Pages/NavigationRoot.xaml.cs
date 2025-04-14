using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Wrappr.Data;
using Wrappr.Services;
using Wrappr.Utilities;

namespace Wrappr.Components.Pages;

public sealed partial class NavigationRoot : Navigation.INavigator, Snackbars.ISnackbarViewport {
	public NavigationRoot() {
		InitializeComponent();

		Navigation.Initialize(this);
		Snackbars.Initialize(this);

		RootNavigation.BackRequested += (_, _) => Navigation.Back();
	}

	public bool ChangePage<TNavigable>(out TNavigable? page) where TNavigable : Page, INavigable {
		var changed = MainViewport.Navigate(typeof(TNavigable));
		page = changed ? (TNavigable)MainViewport.Content : null;
		return changed;
	}

	public void Back() {
		if (MainViewport.CanGoBack) {
			MainViewport.GoBack();
		}
	}

	public void Forward() {
		if (MainViewport.CanGoForward) {
			MainViewport.GoForward();
		}
	}

	public void Clear() {
		MainViewport.BackStack.Clear();
		MainViewport.ForwardStack.Clear();
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
		if (args.IsSettingsSelected)
			Navigation.NewRoot<SettingsPage>();
		if (((NavigationViewItem)args.SelectedItem).Tag is not string tag)
			return;
		switch (tag) {
			case nameof(WrappersListViewerPage):
				Navigation.NewRoot<WrappersListViewerPage>();
				break;
			case nameof(WrapperGroupsListViewerPage):
				Navigation.NewRoot<WrapperGroupsListViewerPage>();
				break;
		}
	}
}