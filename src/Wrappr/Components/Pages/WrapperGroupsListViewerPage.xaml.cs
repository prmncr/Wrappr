using Microsoft.UI.Xaml;
using Wrappr.Resources;
using Wrappr.Services;
using Wrappr.Utilities;

namespace Wrappr.Components.Pages;

public sealed partial class WrapperGroupsListViewerPage : INavigable
{
	public WrapperGroupsListViewerPage()
	{
		InitializeComponent();
	}

	public string LocalizedName => Strings.WrapperGroupsPageTitle;
}