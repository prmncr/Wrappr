using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Wrappr.Data;
using Wrappr.Services;

namespace Wrappr.Components.Pages;

public sealed partial class NavigationRootPage : Navigation.INavigator, Snackbars.ISnackbarViewport {
	public NavigationRootPage() {
		InitializeComponent();

		Navigation.Initialize(this);
		Snackbars.Initialize(this);

		RootNavigation.BackRequested += (_, _) => Navigation.Back();

		MainViewport.Navigated += (_, _) => RootNavigation.IsBackEnabled = MainViewport.CanGoBack;
	}

	public Page? CurrentPage => MainViewport.Content as Page;

	public void ChangePage<TPage>() {
		MainViewport.Navigate(typeof(TPage));
	}

	public void ChangePageAndRemovePrevious<TPage>() {
		MainViewport.Navigate(typeof(TPage));
		MainViewport.BackStack.Remove(MainViewport.BackStack.Last());
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

	private void OnPaneDisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args) {
		if (sender.PaneDisplayMode == NavigationViewPaneDisplayMode.Top) {
			VisualStateManager.GoToState(this, "Top", true);
		} else {
			VisualStateManager.GoToState(this, args.DisplayMode == NavigationViewDisplayMode.Minimal ? "Compact" : "Default", true);
		}
	}

	private void OnRootNavigationSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) {
		if (args.IsSettingsSelected) {
			Navigation.TryChangePage<SettingsPage>();
			Navigation.Clear();
			RootNavigation.IsBackEnabled = false;
		}

		if (((NavigationViewItem)args.SelectedItem).Tag is not string tag) return;

		if (tag == WrappersListViewerPage.NavigationTag) {
			Navigation.TryChangePage<WrappersListViewerPage>();
			Navigation.Clear();
		}

		RootNavigation.IsBackEnabled = false;
	}
}