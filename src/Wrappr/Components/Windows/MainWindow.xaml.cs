using Windows.Graphics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Wrappr.Components.Pages;
using Wrappr.Data;
using Wrappr.Services;
using Wrappr.Utilities;
using WinRT;
using Microsoft.UI.Composition.SystemBackdrops;

namespace Wrappr.Components.Windows;

public partial class MainWindow : Navigation.INavigator, Snackbars.ISnackbarViewport
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
		Snackbars.Initialize(this);
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

	public void ShowInfoBarMessage(SnackbarData message)
	{
		InfoBar.IsOpen = true;
		InfoBar.Severity = message.Severity;
		InfoBar.Title = message.Title;
		InfoBar.Message = message.Message;
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