using Hardcodet.Wpf.TaskbarNotification;
using Wrappr.Components.TrayHolder;

namespace Wrappr.Utils.UI;

public static class Balloons {
	public static void ShowBalloon(string title, string message, BalloonIcon icon) {
		TrayHolder.Instance.TrayIcon.ShowBalloonTip(title, message, icon);
	}
}