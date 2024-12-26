using System.Windows;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Wrappr.Components.About;
using Wrappr.Wrapping;

namespace Wrappr.Components.MainMenu;

public partial class MainMenu : ReactiveObject {
	[ReactiveCommand]
	private void CreateWrapper() {
		Wrappers.Storage.Add(new Wrapper(new WrapperConfig()));
	}

	[ReactiveCommand]
	private void CloseApplication() {
		Application.Current.Shutdown();
	}

	[ReactiveCommand]
	private void CloseWindow(Window window) {
		window.Hide();
	}

	[ReactiveCommand]
	private void MinimizeWindow(Window window) {
		window.WindowState = WindowState.Minimized;
	}

	[ReactiveCommand]
	private void OpenAbout() {
		new AboutWindow().Show();
	}
}