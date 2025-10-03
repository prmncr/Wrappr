using H.NotifyIcon;
using Microsoft.UI.Xaml;

namespace Wrappr.UI;

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