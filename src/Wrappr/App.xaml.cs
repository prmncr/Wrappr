using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using Microsoft.UI.Xaml;
using Wrappr.Components.Windows;
using Wrappr.Controls;
using Wrappr.Data;
using Wrappr.Services;

namespace Wrappr;

public partial class App : Balloons.IBalloonSender
{
	private static IWindowAdapter _windowAdapter = null!;

	public App()
	{
		Instance = this;
		Balloons.Initialize(this);

		UnhandledException += (_, eventArgs) =>
		{
			Snackbars.ShowSnackbar(eventArgs.Exception);
		};

		InitializeComponent();
		RequestedTheme = ApplicationTheme.Dark;
	}

	public static App Instance { get; private set; } = null!;

	private TaskbarIcon TaskbarIcon { get; set; } = null!;

	[field: AllowNull] [field: MaybeNull] public static ICommand OpenWindowCommand => field ??= new RelayCommand(OpenWindow);

	[field: AllowNull] [field: MaybeNull] public ICommand ExitApplicationCommand => field ??= new RelayCommand(ExitApplication);

	public void ShowBalloonMessage(BalloonMessageData messageData)
	{
		TaskbarIcon.ShowNotification(messageData.Title, messageData.Message, messageData.Icon);
	}

	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		InitializeTrayIcon();

		#if DEBUG
		_windowAdapter = new WinUiWindowAdapter(new MainWindow());
		#else
		_windowAdapter = new PopupWindowAdapter(new MainWindow());
		#endif
	}

	#if DEBUG
	public static void ChangeMainWindowFormat()
	{
		_windowAdapter.Close();
		Navigation.Clear();
		if (_windowAdapter is WinUiWindowAdapter)
		{
			_windowAdapter = new PopupWindowAdapter(new MainWindow());
		} else
		{
			_windowAdapter = new WinUiWindowAdapter(new MainWindow());
		}
		_windowAdapter.Show();
	}
	#endif

	private void InitializeTrayIcon()
	{
		TaskbarIcon = new DynamicTaskbarIcon();
		TaskbarIcon.ForceCreate();
	}

	private static void OpenWindow()
	{
		_windowAdapter.Show();
	}

	private void ExitApplication()
	{
		TaskbarIcon.Dispose();
		_windowAdapter.Close();
		Environment.Exit(0);
	}
}