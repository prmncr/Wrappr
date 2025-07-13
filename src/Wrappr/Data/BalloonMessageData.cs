using H.NotifyIcon.Core;

namespace Wrappr.Data;

public record BalloonMessageData
{
	public required string Title { get; init; }
	public required string Message { get; init; }
	public required NotificationIcon Icon { get; init; }
}