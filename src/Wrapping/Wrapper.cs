using System.ServiceProcess;
using Hardcodet.Wpf.TaskbarNotification;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Wrappr.Localization;
using Wrappr.Utils.UI;

namespace Wrappr.Wrapping;

public partial class Wrapper : ReactiveObject {
	private IObservable<bool> _canToggle;

	[Reactive(SetModifier = AccessModifier.Private)]
	private string _displayName = Strings.EmptyWrapperDisplayName;

	[Reactive(SetModifier = AccessModifier.Private)]
	private bool _enabled;

	[Reactive(SetModifier = AccessModifier.Private)]
	private bool _isInitialized;

	[Reactive] private bool _isNotificationsEnabled;

	[Reactive] private bool _isTrackingEnabled;

	[Reactive(SetModifier = AccessModifier.Private)]
	private bool _isWaitingForStatusChange;

	[Reactive] private int _pollingDelay;

	[Reactive] private ServiceController? _service;

	[Reactive(SetModifier = AccessModifier.Private)]
	private string _serviceName = Strings.EmptyWrapperServiceName;

	private ServiceStatusMonitor? _serviceStatusMonitor;

	public Wrapper(WrapperConfig config) {
		PollingDelay = config.PollingDelay;
		IsTrackingEnabled = config.Tracked;
		IsNotificationsEnabled = config.Notified;
		_canToggle = this.WhenAnyValue(it => it.IsInitialized);

		if (config.Name == null) {
			IsInitialized = false;
			ServiceName = Strings.EmptyWrapperDisplayName;
			DisplayName = Strings.EmptyWrapperDisplayName;
			return;
		}

		Service = Services.ListAll().FirstOrDefault(service => config.Name == service!.ServiceName, null);
		UpdateServiceInfo();

		if (Service == null) {
			return;
		}
		RecreateMonitor();
	}

	[ReactiveCommand]
	private void UpdateWrapper(string serviceName) {
		if (Service == null) {
			Messages.ShowMessage(Strings.ErrorMessageTitle, Strings.ServiceNotFoundMessage);
		}
		if (serviceName == ServiceName) {
			UpdateMonitor();
		} else {
			RecreateMonitor();
		}
		UpdateServiceInfo();
		Wrappers.Save();
	}

	[ReactiveCommand(CanExecute = nameof(_canToggle))]
	private async Task ToggleService() {
		if (Service == null) {
			Messages.ShowMessage(Strings.ErrorMessageTitle, Strings.ServiceNotFoundMessage);
			return;
		}
		string? message;
		_serviceStatusMonitor!.Pause();
		if (Enabled) {
			message = await Disable();
			await Task.Run(() => Service.WaitForStatus(ServiceControllerStatus.Stopped));
			_serviceStatusMonitor.ChangeInitialStatus(ServiceControllerStatus.Stopped);
		} else {
			message = await Enable();
			await Task.Run(() => Service.WaitForStatus(ServiceControllerStatus.Running));
			_serviceStatusMonitor.ChangeInitialStatus(ServiceControllerStatus.Running);
		}
		_serviceStatusMonitor.Resume();
		UpdateServiceInfo();
		if (message != null) {
			Messages.ShowMessage(Strings.ErrorMessageTitle, message);
		}
	}

	[ReactiveCommand]
	private void DeleteWrapper() {
		Wrappers.Storage.Remove(this);
	}

	private void UpdateServiceInfo(string? fallbackServiceName = null) {
		if (Service == null) {
			IsInitialized = false;
			ServiceName = fallbackServiceName!;
			DisplayName = Strings.WrapperForNotFoundService;
			Enabled = false;
			return;
		}
		Service.Refresh();
		Enabled = Service.Status == ServiceControllerStatus.Running;
		ServiceName = Service.ServiceName;
		DisplayName = Service.DisplayName;
		IsInitialized = true;
	}

	private async Task<string?> Disable() {
		try {
			Service!.Stop();
			IsWaitingForStatusChange = true;
			await Task.Run(() => Service?.WaitForStatus(ServiceControllerStatus.Stopped));
			IsWaitingForStatusChange = false;
			return null;
		} catch (Exception e) {
			return e.Message;
		}
	}

	private async Task<string?> Enable() {
		try {
			Service!.Start();
			IsWaitingForStatusChange = true;
			await Task.Run(() => Service?.WaitForStatus(ServiceControllerStatus.Running));
			IsWaitingForStatusChange = false;
			return null;
		} catch (Exception e) {
			return e.Message;
		}
	}

	private void RecreateMonitor() {
		if (Service == null) return;
		if (!IsTrackingEnabled) return;
		_serviceStatusMonitor = new ServiceStatusMonitor(
			Service, () => {
				if (IsNotificationsEnabled) {
					Balloons.ShowBalloon(
						Strings.ServiceStatusWasChangedBalloonTitle,
						string.Format(Strings.ServiceStatusWasChangedBalloonText, Service?.ServiceName, Service?.Status),
						BalloonIcon.Info
					);
				}
				UpdateServiceInfo();
			}
		) {
			PollingDelay = PollingDelay
		};
		_ = Task.Run(_serviceStatusMonitor!.WatchServiceStatus, _serviceStatusMonitor.CancellationTokenSource.Token);
	}

	private void UpdateMonitor() {
		if (!IsTrackingEnabled) {
			_serviceStatusMonitor?.CancellationTokenSource.Cancel();
			_serviceStatusMonitor = null;
			return;
		}
		if (_serviceStatusMonitor != null) {
			_serviceStatusMonitor.PollingDelay = PollingDelay;
		} else {
			RecreateMonitor();
		}
	}
}