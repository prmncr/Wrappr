namespace Wrappr.Components.MainMenu;

public partial class MainMenuWindow {
	public MainMenuWindow() {
		Instance = this;
		InitializeComponent();
	}

	public static MainMenuWindow Instance { get; private set; } = null!;
}