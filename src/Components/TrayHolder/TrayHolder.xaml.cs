using System.Windows;
using Wrappr.Components.MainMenu;
using Wrappr.Utils;

namespace Wrappr.Components.TrayHolder;

public partial class TrayHolder {
	public static TrayHolder Instance { get; private set; } = null!;

	public TrayHolder() {
		Instance = this;
		InitializeComponent();
		if (!Arguments.Boot.IsSilentMode) {
			new MainMenuWindow().Show();
		}
	}

	private void OpenMainMenu(object sender, RoutedEventArgs e) {
		new MainMenuWindow().Show();
	}

	private void CloseApplication(object sender, RoutedEventArgs e) {
		Application.Current.Shutdown();
	}
}