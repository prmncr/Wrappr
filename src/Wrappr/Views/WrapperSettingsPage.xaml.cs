using Microsoft.UI.Xaml;
using Wrappr.Models;
using Wrappr.UI;
using Wrapper = Wrappr.ViewModels.Wrapper;

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