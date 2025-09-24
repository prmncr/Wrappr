using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using H.NotifyIcon.Core;
using Microsoft.UI.Xaml;
using Wrappr.Components.Windows;
using Wrappr.Controls;
using Wrappr.Data;
using Wrappr.Services;

namespace Wrappr;

public partial class App : Notifications.INotifier
{
	private static IWindowAdapter _windowAdapter = null!;

	public App()
	{
		Instance = this;
		Notifications.BalloonNotifier = this;

		UnhandledException += (_, eventArgs) =>
		{
			Notifications.ShowNearestNotification(eventArgs.Exception);
		};

		InitializeComponent();
		RequestedTheme = ApplicationTheme.Dark;
	}

	public static App Instance { get; private set; } = null!;

	private TaskbarIcon TaskbarIcon { get; set; } = null!;

	[field: AllowNull, MaybeNull]
	public static ICommand OpenWindowCommand => field ??= new RelayCommand(OpenWindow);

	[field: AllowNull, MaybeNull]
	public ICommand ExitApplicationCommand => field ??= new RelayCommand(ExitApplication);

	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		InitializeTrayIcon();
		var window = new MainWindow();
		#if DEBUG
		_windowAdapter = new WinUiWindowAdapter(window);
		#else
		_windowAdapter = new PopupWindowAdapter(window);
		#endif
		Notifications.HookWindow(window);
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

	public void Show(Notification notification)
	{
		TaskbarIcon.ShowNotification(
			notification.Title,
			notification.Message ?? "",
			notification.NotificationSeverity switch
			{
				Notification.Severity.None => NotificationIcon.None,
				Notification.Severity.Info => NotificationIcon.Info,
				Notification.Severity.Warning => NotificationIcon.Warning,
				Notification.Severity.Error => NotificationIcon.Error,
				_ => NotificationIcon.None
			}
		);
	}
}