using Microsoft.UI.Xaml.Controls;

namespace Wrappr.Services;

public class Navigation {
	private static Navigation _instance = null!;

	private Navigation(INavigator navigator) {
		Navigator = navigator;
	}

	private INavigator? Navigator { get; }

	public static Page? CurrentPage => _instance.Navigator?.CurrentPage;

	public static void ChangePage<TPage>() {
		_instance.Navigator?.ChangePage<TPage>();
	}

	public static void TryChangePage<TPage>() {
		_instance.Navigator?.ChangePage<TPage>();
	}

	public static void ChangePageAndRemovePrevious<TPage>() {
		_instance.Navigator?.ChangePageAndRemovePrevious<TPage>();
	}

	public static void Back() {
		_instance.Navigator?.Back();
	}

	public static void Forward() {
		_instance.Navigator?.Forward();
	}

	public static void Clear() {
		_instance.Navigator?.Clear();
	}

	public static void Initialize(INavigator navigator) {
		_instance = new Navigation(navigator);
	}

	public interface INavigator {
		public Page? CurrentPage { get; }

		public void ChangePage<TPage>();

		public void TryChangePage<TPage>() {
			if (CurrentPage is not TPage) ChangePage<TPage>();
		}

		public void ChangePageAndRemovePrevious<TPage>();

		public void Back();

		public void Forward();

		public void Clear();
	}
}