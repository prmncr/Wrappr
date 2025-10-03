using Windows.Graphics;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Wrappr.Data;
using Wrappr.Model;
using Wrappr.Interop;
using WinRT;
using Microsoft.UI.Composition.SystemBackdrops;
using WindowActivatedEventArgs = Microsoft.UI.Xaml.WindowActivatedEventArgs;

namespace Wrappr.Views;

public partial class MainWindow : Navigation.INavigator, Notifications.INotifier
{
	private WindowsSystemDispatcherQueueHelper? _wsdqHelper;
	private DesktopAcrylicController? _acrylicController;
	private SystemBackdropConfiguration? _configurationSource;

	public MainWindow()
	{
		InitializeComponent();
		var size = new SizeInt32(400, 550);
		AppWindow.Resize(size);
		AppWindow.SetIcon("Assets/Images/logo32.ico");
		Navigation.Initialize(this);
		Notifications.InAppNotifier = this;
		SetAcrylicBackdrop();
	}

	private void FrameLoaded(object sender, RoutedEventArgs e)
	{
		Navigation.ChangePage<WrappersListViewerPage>();
	}

	public bool ChangePage(Type pageType, out Page? page)
	{
		var changed = MainFrame.Navigate(pageType);
		page = changed ? (Page)MainFrame.Content : null;
		return changed;
	}

	public void Back()
	{
		if (MainFrame.CanGoBack)
		{
			MainFrame.GoBack();
		}
	}

	public void Clear()
	{
		MainFrame.BackStack.Clear();
		MainFrame.ForwardStack.Clear();
	}

	public void Show(Notification notification)
	{
		DispatcherQueue.EnqueueAsync(() =>
			{
				InfoBar.IsOpen = true;
				InfoBar.Severity = notification.NotificationSeverity switch
				{
					Notification.Severity.Info => InfoBarSeverity.Informational,
					Notification.Severity.Warning => InfoBarSeverity.Warning,
					Notification.Severity.Error => InfoBarSeverity.Error,
					Notification.Severity.Success => InfoBarSeverity.Success,
					_ => InfoBarSeverity.Informational
				};
				InfoBar.Title = notification.Title;
				InfoBar.Message = notification.Message;
			}
		);
	}

	private void SetAcrylicBackdrop()
	{
		if (!DesktopAcrylicController.IsSupported()) return;

		_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
		_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

		_configurationSource = new SystemBackdropConfiguration();
		Activated += OnWindowActivated;
		Closed += OnWindowClosed;

		_configurationSource.IsInputActive = true;
		_configurationSource.Theme = SystemBackdropTheme.Dark;
		_acrylicController = new DesktopAcrylicController
		{
			Kind = DesktopAcrylicKind.Thin
		};
		_acrylicController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
		_acrylicController.SetSystemBackdropConfiguration(_configurationSource);
	}

	private void OnWindowActivated(object sender, WindowActivatedEventArgs args)
	{
		if (_configurationSource == null) return;
		_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
	}

	private void OnWindowClosed(object sender, WindowEventArgs args)
	{
		if (_acrylicController != null)
		{
			_acrylicController.Dispose();
			_acrylicController = null;
		}
		Activated -= OnWindowActivated;
		_configurationSource = null;
	}
}