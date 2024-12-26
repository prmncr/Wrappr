using Hardcodet.Wpf.TaskbarNotification;
using Wrappr.Components.MainMenu;

namespace Wrappr.Utils.UI;

public static class Balloons {
	public static void ShowBalloon(string title, string message, BalloonIcon icon) {
		MainMenuWindow.Instance.TrayIcon.ShowBalloonTip(title, message, icon);
	}
}