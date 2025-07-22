using H.NotifyIcon;
using Microsoft.UI.Xaml;

namespace Wrappr.Components.Windows;

public class WinUiWindowAdapter(Window window) : IWindowAdapter
{
	public void Show()
	{
		window.Show();
	}

	public void Close()
	{
		window.Close();
	}
}