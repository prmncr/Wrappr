using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Wrappr.Utilities;

namespace Wrappr.Services;

public partial class Navigation : ObservableObject {
	[ObservableProperty] public partial bool CanGoBack { get; set; }

	public static Navigation Instance { get; private set; } = null!;

	private INavigator? Navigator { get; }

	public static IEnumerable<string> Trace => BackStack.Select(it => it.LocalizedName).Reverse();

	private static readonly ObservableStack<INavigable> BackStack = new();

	private Navigation(INavigator navigator) {
		Navigator = navigator;
		BackStack.CollectionChanged += (_, _) => CanGoBack = BackStack.Count > 1;
	}

	public static void ChangePage<TNavigable>(object? dataContext = null) where TNavigable : Page, INavigable {
		if (Instance.Navigator?.ChangePage<TNavigable>(out var page) != true) return;
		page!.DataContext = dataContext;
		BackStack.Push(page);
	}

	public static void DropCurrentPageAndChange<TNavigable>(object? dataContext = null) where TNavigable : Page, INavigable {
		BackStack.Pop();
		Instance.Navigator?.Back();
		ChangePage<TNavigable>(dataContext);
	}

	public static void NewRoot<TNavigable>(object? dataContext = null) where TNavigable : Page, INavigable {
		Clear();
		ChangePage<TNavigable>(dataContext);
	}

	public static void Back() {
		BackStack.Pop();
		Instance.Navigator?.Back();
	}

	public static void BackTo(int index) {
		while (BackStack.Count - 1 != index) {
			Back();
		}
	}

	public static void Forward() {
		Instance.Navigator?.Forward();
	}

	private static void Clear() {
		BackStack.Clear();
		Instance.Navigator?.Clear();
	}

	public static void Initialize(INavigator navigator) {
		Instance = new Navigation(navigator);
	}

	public interface INavigator {
		public bool ChangePage<TNavigable>(out TNavigable? page) where TNavigable : Page, INavigable;

		public void Back();

		public void Forward();

		public void Clear();
	}
}