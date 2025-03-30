using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using Microsoft.UI.Xaml;
using Wrappr.Components.Windows;
using Wrappr.Controls;
using Wrappr.Data;
using Wrappr.Model;
using Wrappr.Services;
using Wrappr.Utilities;

namespace Wrappr;

public partial class App : Balloons.IBalloonSender {
	private static Window? _window;

	public App() {
		Environment.SetEnvironmentVariable("MICROSOFT_WINDOWSAPPRUNTIME_BASE_DIRECTORY", AppContext.BaseDirectory);
		Instance = this;
		Balloons.Initialize(this);

		UnhandledException += (_, eventArgs) => {
			Snackbars.ShowSnackbar(new SnackbarData(eventArgs.Exception));
		};

		InitializeComponent();
		Wrappers.Initialize();
	}

	public static App Instance { get; private set; } = null!;

	private TaskbarIcon TaskbarIcon { get; set; } = null!;

	[field: AllowNull] [field: MaybeNull] public static ICommand ToggleWindowCommand => field ??= new RelayCommand(ToggleWindow);

	[field: AllowNull] [field: MaybeNull] public ICommand ExitApplicationCommand => field ??= new RelayCommand(ExitApplication);

	public void ShowBalloonMessage(BalloonMessageData messageData) {
		TaskbarIcon.ShowNotification(messageData.Title, messageData.Message, messageData.Icon);
	}

	protected override void OnLaunched(LaunchActivatedEventArgs args) {
		Arguments.Initialize(Environment.GetCommandLineArgs());
		InitializeTrayIcon();
		if (Arguments.Boot.IsSilentMode) return;
		_window = new MainWindow();
		_window.Activate();
		_window.Closed += (_, _) => _window = null;
	}

	private void InitializeTrayIcon() {
		TaskbarIcon = new DynamicTaskbarIcon();
		TaskbarIcon.ForceCreate();
	}

	private static void ToggleWindow() {
		if (_window == null) {
			_window = new MainWindow();
			_window.Show();
			return;
		}
		_window.Close();
		_window = null;
	}

	private void ExitApplication() {
		TaskbarIcon.Dispose();
		_window?.Close();

		if (_window == null) {
			Environment.Exit(0);
		}
	}
}