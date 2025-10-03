using System.ServiceProcess;
using Wrappr.Data;
using Wrappr.Resources;
using Wrappr.Services;

namespace Wrappr.Utilities;

public class ServiceStatusMonitor(
	ServiceController service,
	bool notify,
	int pollingDelay,
	Action callback
) {
	private ServiceControllerStatus _initialStatus = service.Status;
	private bool _suppressed;

	public CancellationTokenSource CancellationTokenSource { get; } = new();

	public int PollingDelay { get; set; } = pollingDelay;

	public bool Notify { get; set; } = notify;

	public void Pause()
	{
		_suppressed = true;
	}

	public void Resume()
	{
		_suppressed = false;
	}

	public void ChangeInitialStatus(ServiceControllerStatus status)
	{
		_initialStatus = status;
	}

	public void WatchServiceStatus()
	{
		while (true)
		{
			if (CancellationTokenSource.IsCancellationRequested) return;
			if (_suppressed) continue;
			service.Refresh();
			if (_initialStatus == service.Status)
			{
				Thread.Sleep(PollingDelay);
				continue;
			}

			if (Notify)
			{
				Notifications.ShowBalloonNotification(
					new Notification(
						Strings.ServiceStatusWasChangedBalloonTitle,
						string.Format(Strings.ServiceStatusWasChangedBalloonText, service.ServiceName, service.Status)
					)
				);
			}
			callback?.Invoke();
			_initialStatus = service.Status;
		}
	}
}