using Microsoft.UI.Xaml;

namespace Wrappr.Services;

public class ApplicationWindowProvider
{
	private static ApplicationWindowProvider _instance = null!;

	private readonly Window _window;

	private ApplicationWindowProvider(Window window)
	{
		_window = window;
	}

	public static Window Window => _instance._window;

	public static void Initialize(Window window)
	{
		_instance = new ApplicationWindowProvider(window);
	}
}