using System.Runtime.InteropServices;
using Windows.Graphics;
using H.NotifyIcon.Core;
using H.NotifyIcon.Interop;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;

namespace Wrappr.Components.Windows;

public partial class PopupWindow : IWindow
{
	private delegate nint WndProc(nint hWnd, uint msg, nint wParam, nint lParam);

	private readonly Window _window;
	private readonly AppWindow _appWindow;
	private readonly RectInt32 _windowRect;

	// prevents garbage collector to eat this bro
	// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
	private readonly WndProc _interceptor;
	private readonly nint _hWnd;
	private readonly nint _interceptedWindow;

	private const uint WindowMovedMsg = 0x0084;
	private const int HtCaption = 2;
	private const int GwlpWndProc = -4;

	public PopupWindow(Window window)
	{
		_window = window;

		_hWnd = WindowNative.GetWindowHandle(_window);
		_interceptor = Intercept;
		_interceptedWindow = SetWindowLongPtrSafe(_hWnd, GwlpWndProc, Marshal.GetFunctionPointerForDelegate(_interceptor));

		DesktopWindowsManagerMethods.SetRoundedCorners(_hWnd);
		var id = Win32Interop.GetWindowIdFromWindow(_hWnd);
		_appWindow = AppWindow.GetFromWindowId(id);
		_appWindow.IsShownInSwitchers = false;

		var size = _window.AppWindow.Size;
		var screenSize = DisplayArea.Primary.WorkArea;
		_windowRect = new RectInt32(screenSize.Width - size.Width - 10, screenSize.Height - size.Height - 10, size.Width, size.Height);

		var presenter = (OverlappedPresenter)_appWindow.Presenter;
		presenter.IsMaximizable = false;
		presenter.IsMinimizable = false;
		presenter.IsResizable = false;
		presenter.IsAlwaysOnTop = true;

		HwndUtilities.SetWindowStyleAsPopupWindow(_hWnd);

		_window.Activated += (_, args) =>
		{
			if (args.WindowActivationState != WindowActivationState.Deactivated)
			{
				WindowUtilities.ShowWindow(_hWnd);
			} else
			{
				WindowUtilities.HideWindow(_hWnd);
			}
		};
	}

	public void Show()
	{
		_appWindow.MoveAndResize(_windowRect);
		WindowUtilities.ShowWindow(_hWnd);
		WindowUtilities.SetForegroundWindow(_hWnd);
	}

	public void Close()
	{
		_window.Close();
	}

	private nint Intercept(nint hWnd, uint msg, nint wParam, nint lParam)
	{
		if (msg != WindowMovedMsg) return CallWindowProc(_interceptedWindow, hWnd, msg, wParam, lParam);
		var result = CallWindowProc(_interceptedWindow, hWnd, msg, wParam, lParam);
		return result == HtCaption ? 0 : result;
	}

	[LibraryImport("user32.dll", EntryPoint = "CallWindowProcW", SetLastError = true)]
	private static partial nint CallWindowProc(nint lpPrevWndFunc, nint hWnd, uint msg, nint wParam, nint lParam);

	[LibraryImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
	private static partial nint SetWindowLongPtr64(nint hWnd, int nIndex, nint dwNewLong);

	[LibraryImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = true)]
	private static partial int SetWindowLong32(nint hWnd, int nIndex, int dwNewLong);

	private static nint SetWindowLongPtrSafe(nint hWnd, int nIndex, nint dwNewLong)
	{
		return nint.Size == 8 ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong) : new nint(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
	}
}