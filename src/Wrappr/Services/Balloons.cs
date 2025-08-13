using Wrappr.Data;

namespace Wrappr.Services;

public static class Balloons
{
	private static IBalloonSender? _sender;

	public static void ShowBalloonMessage(BalloonMessageData messageData)
	{
		_sender?.ShowBalloonMessage(messageData);
	}

	public static void Initialize(IBalloonSender sender)
	{
		_sender = sender;
	}

	public interface IBalloonSender
	{
		public void ShowBalloonMessage(BalloonMessageData messageData);
	}
}