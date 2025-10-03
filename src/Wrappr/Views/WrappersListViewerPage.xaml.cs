using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Wrappr.Resources;
using Wrappr.Model;
using Wrappr.UI;

namespace Wrappr.Views;

public partial class WrappersListViewerPage : INavigable
{
	public WrappersListViewerPage()
	{
		InitializeComponent();
		#if DEBUG
		CreateDebugMenu();
		#endif
	}

	public string LocalizedName => Strings.WrappersTitle;

	private void WrapperCardClicked(object sender, RoutedEventArgs e)
	{
		Navigation.ChangePage<WrapperSettingsPage>((sender as FrameworkElement)!.DataContext);
	}

	private void CreateWrapper(object sender, RoutedEventArgs e)
	{
		Navigation.ChangePage<CreateWrapperPage>();
	}

	private void SettingsButtonClicked(object sender, RoutedEventArgs e)
	{
		Navigation.ChangePage<SettingsPage>();
	}

	#if DEBUG
	private void CreateDebugMenu()
	{
		OptionsFlyout.Items.Add(
			new MenuFlyoutSubItem
			{
				BorderBrush = null,
				Background = null,
				Text = "Debug features",
				Icon = new FontIcon
				{
					Glyph = Icons.Bug,
					FontSize = 14
				},
				Items =
				{
					new MenuFlyoutItem
					{
						Text = "Switch window mode",
						Command = new RelayCommand(App.ChangeMainWindowFormat)
					}
				}
			}
		);
	}
	#endif

	private void Exit(object sender, RoutedEventArgs e)
	{
		Application.Current.Exit();
	}

	private async void NotElevatedButtonClick(object sender, RoutedEventArgs e)
	{
		try
		{
			await new ContentDialog
			{
				XamlRoot = XamlRoot,
				Title = Strings.ReadOnlyModeTitle,
				Content = Strings.ReadOnlyModeDescription,
				PrimaryButtonText = Strings.ReloadAppButtonText,
				PrimaryButtonCommand = Elevation.ElevateCommand,
				DefaultButton = ContentDialogButton.Primary,
				CloseButtonText = Strings.Cancel
			}.ShowAsync();
		} catch (Exception exception)
		{
			Notifications.ShowNearestNotification(exception);
		}
	}
}