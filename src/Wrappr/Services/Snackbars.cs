using Wrappr.Data;

namespace Wrappr.Services;

public class Snackbars {
	private static Snackbars _instance = null!;

	private readonly ISnackbarViewport _viewport;

	private Snackbars(ISnackbarViewport viewport) {
		_viewport = viewport;
	}

	public static void ShowSnackbar(SnackbarData message) {
		_instance._viewport.ShowInfoBarMessage(message);
	}

	public static void Initialize(ISnackbarViewport viewport) {
		_instance = new Snackbars(viewport);
	}

	public interface ISnackbarViewport {
		public void ShowInfoBarMessage(SnackbarData message);
	}
}