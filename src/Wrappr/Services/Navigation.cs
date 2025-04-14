using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Wrappr.Utilities;

namespace Wrappr.Services;

public partial class Navigation : ObservableObject {
	[ObservableProperty] public partial bool CanGoBack { get; set; }

	public static Navigation Instance { get; private set; } = null!;

	private INavigator? Navigator { get; }

	public static IEnumerable<NavigationNode> Trace => BackStack.Reverse();

	private static readonly ObservableStack<NavigationNode> BackStack = new();
	private static Page? CurrentPage => Instance.Navigator?.CurrentPage;

	private Navigation(INavigator navigator) {
		Navigator = navigator;
		BackStack.CollectionChanged += (_, _) => CanGoBack = BackStack.Count > 1;
	}

	public static void ChangePage<TNavigable>(object? dataContext = null) where TNavigable : INavigable {
		Instance.Navigator?.ChangePage<TNavigable>();
		CurrentPage!.DataContext = dataContext;
		BackStack.Push(new NavigationNode(TNavigable.TypeNavigationTag, TNavigable.NodeName(dataContext), dataContext));
	}

	public static void DropCurrentPageAndChange<TNavigable>(object? dataContext = null) where TNavigable : INavigable {
		BackStack.Pop();
		Instance.Navigator?.Back();
		ChangePage<TNavigable>(dataContext);
	}

	public static void NewRoot<TNavigable>(object? dataContext = null) where TNavigable : INavigable {
		Clear();
		ChangePage<TNavigable>(dataContext);
	}

	public static void Back() {
		BackStack.Pop();
		Instance.Navigator?.Back();
	}

	public static void BackTo(NavigationNode node) {
		while (Instance.Navigator?.CurrentPage is INavigable navigable && navigable.NavigationTag != node.Tag) {
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
		public Page? CurrentPage { get; }

		public void ChangePage<TPage>();

		public void Back();

		public void Forward();

		public void Clear();
	}
}