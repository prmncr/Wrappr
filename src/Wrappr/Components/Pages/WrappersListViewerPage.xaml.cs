using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Wrappr.Resources;
using Wrappr.Services;
using Wrappr.Utilities;

namespace Wrappr.Components.Pages;

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
        OptionsFlyout.Items.Add(new MenuFlyoutSubItem
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
        });
    }
    #endif

    private void Exit(object sender, RoutedEventArgs e)
    {
	    Application.Current.Exit();
    }
}