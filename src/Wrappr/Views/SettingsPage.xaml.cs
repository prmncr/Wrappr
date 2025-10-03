using Microsoft.UI.Xaml.Controls;
using Wrappr.Resources;
using Wrappr.UI;

namespace Wrappr.Views;

public partial class SettingsPage : INavigable
{
	public static string SettingsTag => "settings";
	public static string AboutTag => "about";

	public SettingsPage()
	{
		InitializeComponent();
	}

	public string LocalizedName => Strings.SettingsTitle;

	private void SubPageSelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
	{
		var item = sender.SelectedItem;
		if ((string)item.Tag == SettingsTag)
		{
			SubFrame.Navigate(typeof(PreferencesSubPage));
		} else if ((string)item.Tag == AboutTag)
		{
			SubFrame.Navigate(typeof(AboutSubPage));
		}
	}
}