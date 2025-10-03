using Microsoft.UI.Xaml;
using Wrappr.Model;
using Wrappr.UI;

namespace Wrappr.Views;

public partial class WrapperSettingsPage : INavigable
{
	public WrapperSettingsPage()
	{
		InitializeComponent();
	}

	private void AfterWrapperDeleted(object sender, RoutedEventArgs e)
	{
		Navigation.Back();
	}

	public string LocalizedName => (DataContext as Wrapper)!.ServiceName;
}