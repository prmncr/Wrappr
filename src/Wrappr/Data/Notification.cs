namespace Wrappr.Data;

public record Notification(string Title, string? Message = null, Notification.Severity NotificationSeverity = Notification.Severity.Info)
{
	public enum Severity
	{
		/**
		 * Works only for balloons
		 */
		None,
		Info,
		Warning,
		Error,
		/**
		 * Works only for in-app
		 */
		Success
	}

	public static implicit operator Notification(Exception exception)
	{
		return new Notification(
			#if DEBUG
			exception.Message,
			exception.StackTrace ?? "",
			#else
			Strings.ErrorMessageTitle,
			exception.Message,
			#endif
			Severity.Error
		);
	}
}