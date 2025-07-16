using Microsoft.UI.Xaml.Controls;
using Wrappr.Utilities;

namespace Wrappr.Services;

public static class Navigation
{
	public static string PreviousLocalizedTitle { get; private set; }

	private static INavigator? _navigator;

	public static IEnumerable<string> Trace => BackStack.Select(it => it.LocalizedName).Reverse();

	private static readonly Stack<INavigable> BackStack = new();

	public static void ChangePage<TNavigable>(object? dataContext = null) where TNavigable : Page, INavigable
	{
		ChangePage(typeof(TNavigable), dataContext);
	}

	public static void ChangePage(Type pageType, object? dataContext = null)
	{
		if (BackStack.TryPeek(out var last))
		{
			PreviousLocalizedTitle = last.LocalizedName;
		}
		if (_navigator?.ChangePage(pageType, out var page) != true) return;
		page!.DataContext = dataContext;
		BackStack.Push((INavigable)page);
	}

	public static void DropCurrentPageAndChange<TNavigable>(object? dataContext = null) where TNavigable : Page, INavigable
	{
		BackStack.Pop();
		_navigator?.Back();
		ChangePage<TNavigable>(dataContext);
	}

	public static void Back()
	{
		BackStack.Pop();
		_navigator?.Back();
        if (BackStack.TryPeek(out var last))
        {
            PreviousLocalizedTitle = last.LocalizedName;
        }
    }

	public static void BackTo(int index)
	{
		while (BackStack.Count - 1 != index)
		{
			Back();
		}
	}

	public static void Initialize(INavigator navigator)
	{
		_navigator = navigator;
	}

    public static void Clear()
    {
		BackStack.Clear();
		_navigator?.Clear();
    }

    public interface INavigator
	{
		public bool ChangePage(Type pageType, out Page? page);

		public void Back();

		public void Clear();
	}
}