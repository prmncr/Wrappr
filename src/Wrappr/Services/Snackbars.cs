using Wrappr.Data;

namespace Wrappr.Services;

public static class Snackbars
{
	private static ISnackbarViewport? _viewport;

	public static void ShowSnackbar(SnackbarData message)
	{
		_viewport?.ShowInfoBarMessage(message);
	}

	public static void Initialize(ISnackbarViewport viewport)
	{
		_viewport = viewport;
	}

	public interface ISnackbarViewport
	{
		public void ShowInfoBarMessage(SnackbarData message);
	}
}