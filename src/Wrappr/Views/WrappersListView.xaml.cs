using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Wrappr.Models;
using Wrappr.Resources;
using Wrappr.UI;

namespace Wrappr.Views;

public partial class WrappersListView : INavigable
{
	public WrappersListView()
	{
		InitializeComponent();
		#if DEBUG
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
		#endif
	}

	public string LocalizedName => Strings.WrappersTitle;

	private async void OpenElevationDialog(object? sender, object? args)
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
		} catch (Exception e)
		{
			Notifications.ShowNearestNotification(e);
		}
	}
}