using Wrappr.Resources;
using Wrappr.Utilities;

namespace Wrappr.Components.Pages;

public sealed partial class WrapperGroupsListViewerPage : INavigable
{
    public WrapperGroupsListViewerPage()
    {
        InitializeComponent();
    }

    public static string TypeNavigationTag => nameof(WrapperGroupsListViewerPage);

    public string NavigationTag => nameof(WrapperGroupsListViewerPage);
    public static string NodeName(object? parameter) => Strings.WrapperGroupsPageTitle;
}