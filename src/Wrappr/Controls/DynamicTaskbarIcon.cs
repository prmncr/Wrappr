using System.Drawing;
using Windows.Graphics;
using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using H.NotifyIcon.Core;
using H.NotifyIcon.Interop;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using WinRT.Interop;
using Wrappr.Model;
using Wrappr.Resources;
using Wrappr.Services;
using Wrappr.Utilities;
using Size = Windows.Foundation.Size;

namespace Wrappr.Controls;

public class DynamicTaskbarIcon
{
	private const string IconPath = "Assets/Images/icon64.ico";
	private readonly AppWindow _appWindow;
	private readonly Frame _frame;
	private readonly IntPtr _hWnd;
	private readonly Icon _icon = Icon.ExtractIcon(IconPath, 0)!;
	private readonly TaskbarIcon _taskbarIcon;
	private readonly Window _window;
	private MenuFlyout _flyout;
	private bool _isContextMenuVisible;

	public DynamicTaskbarIcon()
	{
		_taskbarIcon = new TaskbarIcon
		{
			Visibility = Visibility.Visible,
			ToolTipText = Strings.AppName,
			ContextMenuMode = ContextMenuMode.SecondWindow,
			LeftClickCommand = App.OpenWindowCommand,
			NoLeftClickDelay = true,
			RightClickCommand = new RelayCommand(UpdateFlyout),
			Icon = _icon
		};

		_frame = new Frame { Background = new SolidColorBrush(Colors.Transparent) };

		_window = new Window
		{
			Content = _frame,
			Title = "Tray Dialog Helper"
		};

		_taskbarIcon.ActualThemeChanged += (_, _) => _frame.RequestedTheme = _taskbarIcon.ActualTheme;

		_hWnd = WindowNative.GetWindowHandle(_window);
		DesktopWindowsManagerMethods.SetRoundedCorners(_hWnd);
		WindowUtilities.MakeTransparent(_hWnd);

		var id = Win32Interop.GetWindowIdFromWindow(_hWnd);
		_appWindow = AppWindow.GetFromWindowId(id);
		_appWindow.IsShownInSwitchers = false;
		_appWindow.SetIcon(Application.Current.RequestedTheme == ApplicationTheme.Dark ? "Assets/Images/services-white.ico" : "Assets/Images/services-black.ico");

		var presenter = (OverlappedPresenter)_appWindow.Presenter;
		presenter.IsMaximizable = false;
		presenter.IsMinimizable = false;
		presenter.IsResizable = false;
		presenter.IsAlwaysOnTop = true;
		presenter.SetBorderAndTitleBar(false, false);

		_flyout = PrepareFlyout(GetItems().ToList());

		_frame.Loaded += (_, _) =>
		{
			HwndUtilities.SetWindowStyleAsPopupWindow(_hWnd);

			_flyout.ShowAt(
				_window.Content, new FlyoutShowOptions
				{
					ShowMode = FlyoutShowMode.Transient
				}
			);
			_flyout.Hide();
		};

		_window.Activated += (sender, args) =>
		{
			if (args.WindowActivationState == WindowActivationState.Deactivated)
			{
				_isContextMenuVisible = false;
				_flyout.Hide();
				_ = WindowUtilities.HideWindow(_hWnd);
				return;
			}

			_isContextMenuVisible = true;
			_flyout.ShowAt(
				_window.Content, new FlyoutShowOptions
				{
					ShowMode = FlyoutShowMode.Transient
				}
			);
		};
	}

	public static implicit operator TaskbarIcon(DynamicTaskbarIcon dynamicTaskbarIcon)
	{
		return dynamicTaskbarIcon._taskbarIcon;
	}

	private static List<MenuFlyoutItemBase> GetTopControls()
	{
		return
		[
			new MenuFlyoutItem
			{
				Text = Strings.ContextMenuOpenApp,
				Command = App.OpenWindowCommand,
				Icon = new SymbolIcon(Symbol.Home)
			},
			new MenuFlyoutSeparator()
		];
	}

	private static List<MenuFlyoutItemBase> GetBottomControls()
	{
		return
		[
			new MenuFlyoutSeparator(),
			new MenuFlyoutItem
			{
				Text = Strings.CloseApp,
				Icon = new FontIcon { Glyph = Icons.PowerButton },
				Command = App.Instance.ExitApplicationCommand
			}
		];
	}

	private void UpdateFlyout()
	{
		var content = GetItems().ToList();
		_flyout = PrepareFlyout(content);
		ShowContextMenu(Cursor.Position, content);
	}

	private static IEnumerable<MenuFlyoutItemBase> GetWrappers()
	{
		if (Wrappers.Instance.Storage.Count == 0)
		{
			yield return new MenuFlyoutItem
			{
				IsEnabled = false,
				Text = Strings.NoWrappersPopupPlaceholder
			};
			yield break;
		}
		foreach (var it in Wrappers.Instance.Storage)
		{
			var item = new ToggleMenuFlyoutItem
			{
				Text = it.ServiceName,
				IsChecked = it.Enabled,
				IsEnabled = Elevation.IsElevated
			};
			it.ServiceToggled += newValue => item.IsChecked = newValue;
			item.Click += async (_, _) => await it.ToggleService(!item.IsChecked);
			yield return item;
		}
	}

	private static IEnumerable<MenuFlyoutItemBase> GetItems()
	{
		return GetTopControls().Concat(GetWrappers()).Concat(GetBottomControls());
	}

	private void ShowContextMenu(Point cursorPosition, IEnumerable<MenuFlyoutItemBase> content)
	{
		var maxSize = new Size(double.PositiveInfinity, double.PositiveInfinity);

		var width = 100.0;
		var height = 8.0;

		foreach (var item in content)
		{
			item.Measure(maxSize);

			if (item is MenuFlyoutItem i)
			{
				width = Math.Max(width, item.FontSize * i.Text.Length);
			}
			height += item.DesiredSize.Height;
		}

		var scale = _flyout.XamlRoot?.RasterizationScale ?? 1.0;

		var rectangle = CursorUtilities.CalculatePopupWindowPosition(
			cursorPosition.X,
			cursorPosition.Y,
			(int)Math.Round(scale * (width + 4.0)),
			(int)Math.Round(scale * (height + 4.0))
		);

		WindowUtilities.ShowWindow(_hWnd);
		_appWindow.MoveAndResize(new RectInt32(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height));
		WindowUtilities.SetForegroundWindow(_hWnd);
	}

	private MenuFlyout PrepareFlyout(IEnumerable<MenuFlyoutItemBase> content)
	{
		var flyout = new MenuFlyout
		{
			AreOpenCloseAnimationsEnabled = true,
			Placement = FlyoutPlacementMode.Full
		};

		foreach (var flyoutItemBase in content)
		{
			if (flyoutItemBase is not MenuFlyoutSeparator)
			{
				flyoutItemBase.Height = 32;
				flyoutItemBase.Padding = new Thickness(11, 0, 11, 0);
			}
			flyout.Items.Add(flyoutItemBase);

			if (flyoutItemBase is MenuFlyoutItem item)
			{
				item.Click += (_, _) =>
				{
					_isContextMenuVisible = false;
					flyout.Hide();
					_ = WindowUtilities.HideWindow(_hWnd);
				};
			}
		}

		flyout.Closed += async (_, _) =>
		{
			if (!flyout.AreOpenCloseAnimationsEnabled || !_isContextMenuVisible)
			{
				_ = WindowUtilities.HideWindow(_hWnd);
				return;
			}

			await Task.Delay(1).ConfigureAwait(true);
			flyout.ShowAt(
				_window.Content, new FlyoutShowOptions
				{
					ShowMode = FlyoutShowMode.Transient
				}
			);
		};
		return flyout;
	}
}