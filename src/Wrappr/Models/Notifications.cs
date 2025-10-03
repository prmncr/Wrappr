using Microsoft.UI.Xaml;
using Wrappr.Data;

namespace Wrappr.Models;

public static class Notifications
{
	public static INotifier? InAppNotifier { get; set; }

	public static INotifier? BalloonNotifier { get; set; }

	private static INotifier? _nearest = BalloonNotifier;

	public static void HookWindow(Window window)
	{
		window.VisibilityChanged += (_, args) =>
		{
			if (args.Visible)
			{
				SetNearestToInApp();
			} else
			{
				SetNearestToBalloons();
			}
		};
	}

	public static void ShowInAppNotification(Notification notification)
	{
		InAppNotifier?.Show(notification);
	}

	public static void ShowBalloonNotification(Notification notification)
	{
		BalloonNotifier?.Show(notification);
	}

	public static void ShowNearestNotification(Notification notification)
	{
		_nearest?.Show(notification);
	}

	private static void SetNearestToInApp() => _nearest = InAppNotifier;
	private static void SetNearestToBalloons() => _nearest = BalloonNotifier;

	public interface INotifier
	{
		public void Show(Notification notification);
	}
}