using System.ServiceProcess;

namespace Wrappr.Wrapping;

public class ServiceStatusMonitor(ServiceController service, Action onStatusChanged) {
	private ServiceControllerStatus _initialStatus = service.Status;
	private bool _suppressed;

	public CancellationTokenSource CancellationTokenSource { get; } = new();

	public int PollingDelay { get; set; } = 1000;

	public void Pause() {
		_suppressed = true;
	}

	public void Resume() {
		_suppressed = false;
	}

	public void ChangeInitialStatus(ServiceControllerStatus status) {
		_initialStatus = status;
	}

	public void WatchServiceStatus() {
		while (true) {
			if (CancellationTokenSource.IsCancellationRequested) return;
			if (_suppressed) continue;
			service.Refresh();
			if (_initialStatus == service.Status) {
				Thread.Sleep(PollingDelay);
				continue;
			}
			onStatusChanged.Invoke();
			_initialStatus = service.Status;
		}
	}
}