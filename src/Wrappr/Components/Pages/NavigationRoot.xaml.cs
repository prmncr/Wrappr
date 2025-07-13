using Microsoft.UI.Xaml.Controls;
using Wrappr.Data;
using Wrappr.Services;

namespace Wrappr.Components.Pages;

public partial class NavigationRoot : Navigation.INavigator, Snackbars.ISnackbarViewport
{
	public NavigationRoot()
	{
		InitializeComponent();

		Navigation.Initialize(this);
		Navigation.ChangePage<WrappersListViewerPage>();
		Snackbars.Initialize(this);

		NavigationView.BackRequested += (_, _) => Navigation.Back();
	}

	public bool ChangePage(Type pageType, out Page? page)
	{
		var changed = NavigationFrame.Navigate(pageType);
		page = changed ? (Page)NavigationFrame.Content : null;
		return changed;
	}

	public void Back()
	{
		if (NavigationFrame.CanGoBack)
		{
			NavigationFrame.GoBack();
		}
	}

	public void Clear()
	{
		NavigationFrame.BackStack.Clear();
		NavigationFrame.ForwardStack.Clear();
	}

	public void ShowInfoBarMessage(SnackbarData message)
	{
		InfoBar.IsOpen = true;
		InfoBar.Severity = message.Severity;
		InfoBar.Title = message.Title;
		InfoBar.Message = message.Message;
	}
}