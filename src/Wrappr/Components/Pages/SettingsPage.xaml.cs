using Microsoft.UI.Xaml.Controls;
using Wrappr.Resources;
using Wrappr.Utilities;

namespace Wrappr.Components.Pages;

public sealed partial class SettingsPage : INavigable
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
            SubFrame.Navigate(typeof(SettingsSubPage));
        } else if ((string)item.Tag == AboutTag)
        {
            SubFrame.Navigate(typeof(AboutSubPage));
        }
    }
}