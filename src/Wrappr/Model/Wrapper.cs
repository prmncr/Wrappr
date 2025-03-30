using System.ServiceProcess;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon.Core;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Wrappr.Data;
using Wrappr.Resources;
using Wrappr.Services;
using Wrappr.Utilities;

namespace Wrappr.Model;

public partial class Wrapper : ObservableObject {
	private bool _isNotificationsEnabled;

	private bool _isTrackingEnabled;

	private int _pollingDelay;

	private ServiceStatusMonitor? _serviceStatusMonitor;

	public Wrapper() { }

	public Wrapper(WrapperConfig config) {
		_pollingDelay = config.PollingDelay;
		_isTrackingEnabled = config.Tracked;
		_isNotificationsEnabled = config.Notified;

		if (config.Name == null) {
			IsInitialized = false;
			ServiceName = Strings.EmptyWrapperDisplayName;
			DisplayName = Strings.EmptyWrapperDisplayName;
			return;
		}

		Service = Services.GetAll().FirstOrDefault(service => config.Name == service!.ServiceName, null);
		Enabled = Service?.Status == ServiceControllerStatus.Running;
		ServiceName = Service?.ServiceName ?? Strings.EmptyWrapperServiceName;
		DisplayName = Service?.DisplayName ?? Strings.EmptyWrapperDisplayName;
		IsInitialized = true;

		if (Service == null) {
			return;
		}
		RecreateMonitor();
	}

	private ServiceController? Service { get; }

	[ObservableProperty] public partial string ServiceName { get; set; } = Strings.EmptyWrapperServiceName;

	[ObservableProperty] public partial string DisplayName { get; set; } = Strings.EmptyWrapperDisplayName;

	[ObservableProperty] public partial bool IsInitialized { get; set; }

	[ObservableProperty] public partial bool IsWaitingForStatusChange { get; set; }

	public bool Enabled
	{
		get;
		set {
			OnPropertyChanging();
			field = value;
			ServiceToggled?.Invoke(value);
			OnPropertyChanged();
		}
	}

	public bool IsTrackingEnabled
	{
		get => _isTrackingEnabled;
		private set {
			_isTrackingEnabled = value;
			UpdateWrapper();
			OnPropertyChanged();
		}
	}

	public bool IsNotificationsEnabled
	{
		get => _isNotificationsEnabled;
		private set {
			_isNotificationsEnabled = value;
			UpdateWrapper();
			OnPropertyChanged();
		}
	}

	public int PollingDelay
	{
		get => _pollingDelay;
		set {
			_pollingDelay = value;
			UpdateWrapper();
			OnPropertyChanged();
		}
	}

	public bool CanToggle => IsInitialized && Elevation.IsElevated;

	public event Action<bool>? ServiceToggled;

	private void UpdateWrapper() {
		UpdateMonitor();
		Wrappers.Instance.Save();
	}

	[RelayCommand]
	public async Task ToggleService(bool switchedTo) {
		if (Service == null) {
			Snackbars.ShowSnackbar(new SnackbarData(Strings.ErrorMessageTitle, Strings.ServiceNotFoundMessage, InfoBarSeverity.Error));
			return;
		}
		string? message;
		_serviceStatusMonitor?.Pause();
		if (switchedTo) {
			message = await Disable();
			await Task.Run(() => Service.WaitForStatus(ServiceControllerStatus.Stopped));
			_serviceStatusMonitor?.ChangeInitialStatus(ServiceControllerStatus.Stopped);
		} else {
			message = await Enable();
			await Task.Run(() => Service.WaitForStatus(ServiceControllerStatus.Running));
			_serviceStatusMonitor?.ChangeInitialStatus(ServiceControllerStatus.Running);
		}

		Service.Refresh();
		Enabled = Service.Status == ServiceControllerStatus.Running;
		_serviceStatusMonitor?.Resume();
		if (message != null) {
			Snackbars.ShowSnackbar(new SnackbarData(Strings.ErrorMessageTitle, message, InfoBarSeverity.Error));
		}
	}

	[RelayCommand]
	private void ToggleNotifications() {
		IsNotificationsEnabled = !IsNotificationsEnabled;
	}

	[RelayCommand]
	private void ToggleTracking() {
		IsTrackingEnabled = !IsTrackingEnabled;
	}

	[RelayCommand]
	private void DeleteWrapper() {
		Wrappers.Instance.Storage.Remove(this);
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
		var dispatcher = DispatcherQueue.GetForCurrentThread();
		_serviceStatusMonitor = new ServiceStatusMonitor(
			Service, newStatus => {
				if (IsNotificationsEnabled) {
					Balloons.ShowBalloonMessage(
						new BalloonMessageData {
							Title = Strings.ServiceStatusWasChangedBalloonTitle,
							Message = string.Format(Strings.ServiceStatusWasChangedBalloonText, Service?.ServiceName, newStatus),
							Icon = NotificationIcon.Info
						}
					);
				}
				Service!.Refresh();
				dispatcher.TryEnqueue(
					DispatcherQueuePriority.High, () => {
						Enabled = Service.Status == ServiceControllerStatus.Running;
					}
				);
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