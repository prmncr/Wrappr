using Wrappr.Data;

namespace Wrappr.Services;

public class Balloons {
	private static Balloons _instance = null!;

	private readonly IBalloonSender _sender;

	private Balloons(IBalloonSender sender) {
		_sender = sender;
	}

	public static void ShowBalloonMessage(BalloonMessageData messageData) {
		_instance._sender.ShowBalloonMessage(messageData);
	}

	public static void Initialize(IBalloonSender sender) {
		_instance = new Balloons(sender);
	}

	public interface IBalloonSender {
		public void ShowBalloonMessage(BalloonMessageData messageData);
	}
}