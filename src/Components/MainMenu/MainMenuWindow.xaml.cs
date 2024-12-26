using System.Windows;
using Wrappr.Utils;
using Application = System.Windows.Application;

namespace Wrappr.Components.MainMenu;

public partial class MainMenuWindow {
	public MainMenuWindow() {
		Instance = this;
		if (Arguments.Boot.IsSilentMode) {
			Hide();
		}
	}

	public static MainMenuWindow Instance { get; private set; } = null!;

	private void OpenMainMenu(object sender, RoutedEventArgs e) {
		Show();
	}

	private void CloseApplication(object sender, RoutedEventArgs e) {
		Application.Current.Shutdown();
	}
}