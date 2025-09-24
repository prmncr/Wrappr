using System.ServiceProcess;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Wrappr.Data;
using Wrappr.Resources;
using Wrappr.Services;
using Wrappr.Utilities;

namespace Wrappr.Model;

public partial class Wrapper : ObservableObject
{
	private ServiceStatusMonitor? _serviceStatusMonitor;
	// private fields is used in constructor to avoid saving of empty wrappers
	// ReSharper disable ReplaceWithFieldKeyword
	private int _pollingDelay;
	private bool _isNotificationsEnabled;
	private bool _isTrackingEnabled;
	private bool _enabled;
	// ReSharper restore ReplaceWithFieldKeyword

	private ServiceController? Service { get; }

	[ObservableProperty] public partial string ServiceName { get; private set; } = Strings.EmptyWrapperServiceName;

	[ObservableProperty] public partial string DisplayName { get; private set; } = Strings.EmptyWrapperDisplayName;

	[ObservableProperty] public partial bool IsInitialized { get; private set; }

	[ObservableProperty] public partial bool IsWaitingForStatusChange { get; private set; }

	public bool Enabled
	{
		get => _enabled;
		set
		{
			if (value == _enabled) return;
			OnPropertyChanging();
			Task.Run(() => ToggleService(value));
			_enabled = value;
			ServiceToggled?.Invoke(value);
			OnPropertyChanged();
		}
	}

	public bool IsTrackingEnabled
	{
		get => _isTrackingEnabled;
		private set
		{
			_isTrackingEnabled = value;
			Task.Run(UpdateWrapper);
			OnPropertyChanged();
		}
	}

	public bool IsNotificationsEnabled
	{
		get => _isNotificationsEnabled;
		private set
		{
			_isNotificationsEnabled = value;
			Task.Run(UpdateWrapper);
			OnPropertyChanged();
		}
	}

	public int PollingDelay
	{
		get => _pollingDelay;
		set
		{
			_pollingDelay = value;
			Task.Run(UpdateWrapper);
			OnPropertyChanged();
		}
	}


	public event Action<bool>? ServiceToggled;

	public Wrapper() { }

	public Wrapper(WrapperConfig config)
	{
		_pollingDelay = config.PollingDelay;
		_isTrackingEnabled = config.Tracked;
		_isNotificationsEnabled = config.Notified;

		if (config.Name == null)
		{
			IsInitialized = false;
			ServiceName = Strings.EmptyWrapperDisplayName;
			DisplayName = Strings.EmptyWrapperDisplayName;
			return;
		}

		Service = Services.GetAll().FirstOrDefault(service => config.Name == service!.ServiceName, null);
		_enabled = Service?.Status == ServiceControllerStatus.Running;
		ServiceName = Service?.ServiceName ?? Strings.EmptyWrapperServiceName;
		DisplayName = Service?.DisplayName ?? Strings.EmptyWrapperDisplayName;
		IsInitialized = true;

		if (Service == null)
		{
			return;
		}
		RecreateMonitor();
	}

	private async Task UpdateWrapper()
	{
		UpdateMonitor();
		await WrappersStorage.Save();
	}

	private async Task ToggleService(bool switchedTo)
	{
		if (Service == null)
		{
			Notifications.ShowNearestNotification(new Notification(Strings.ErrorMessageTitle, Strings.ServiceNotFoundMessage, Notification.Severity.Error));
			return;
		}
		string? message;
		_serviceStatusMonitor?.Pause();
		if (!switchedTo)
		{
			message = await Disable();
			await Task.Run(() => Service.WaitForStatus(ServiceControllerStatus.Stopped));
			_serviceStatusMonitor?.ChangeInitialStatus(ServiceControllerStatus.Stopped);
		} else
		{
			message = await Enable();
			await Task.Run(() => Service.WaitForStatus(ServiceControllerStatus.Running));
			_serviceStatusMonitor?.ChangeInitialStatus(ServiceControllerStatus.Running);
		}

		Service.Refresh();
		_serviceStatusMonitor?.Resume();
		if (message != null)
		{
			Notifications.ShowNearestNotification(new Notification(Strings.ErrorMessageTitle, message, Notification.Severity.Error));
		}
	}

	[RelayCommand]
	private void ToggleNotifications()
	{
		IsNotificationsEnabled = !IsNotificationsEnabled;
	}

	[RelayCommand]
	private void ToggleTracking()
	{
		IsTrackingEnabled = !IsTrackingEnabled;
	}

	private async Task<string?> Disable()
	{
		try
		{
			Service!.Stop();
			IsWaitingForStatusChange = true;
			await Task.Run(() => Service?.WaitForStatus(ServiceControllerStatus.Stopped));
			IsWaitingForStatusChange = false;
			return null;
		} catch (Exception e)
		{
			return e.Message;
		}
	}

	private async Task<string?> Enable()
	{
		try
		{
			Service!.Start();
			IsWaitingForStatusChange = true;
			await Task.Run(() => Service?.WaitForStatus(ServiceControllerStatus.Running));
			IsWaitingForStatusChange = false;
			return null;
		} catch (Exception e)
		{
			return e.Message;
		}
	}

	private void RecreateMonitor()
	{
		if (Service == null) return;
		if (!IsTrackingEnabled) return;
		var dispatcher = DispatcherQueue.GetForCurrentThread();

		_serviceStatusMonitor = new ServiceStatusMonitor(Service, StatusChangedCallback) { PollingDelay = PollingDelay };
		_ = Task.Run(_serviceStatusMonitor!.WatchServiceStatus, _serviceStatusMonitor.CancellationTokenSource.Token);
		return;

		void StatusChangedCallback(ServiceControllerStatus newStatus)
		{
			if (IsNotificationsEnabled)
			{
				Notifications.ShowNearestNotification(
					new Notification(
						Strings.ServiceStatusWasChangedBalloonTitle,
						string.Format(Strings.ServiceStatusWasChangedBalloonText, Service?.ServiceName, newStatus)
					)
				);
			}
			Service!.Refresh();
			dispatcher.TryEnqueue(
				DispatcherQueuePriority.High, () =>
				{
					Enabled = Service.Status == ServiceControllerStatus.Running;
				}
			);
		}
	}

	private void UpdateMonitor()
	{
		if (!IsTrackingEnabled)
		{
			_serviceStatusMonitor?.CancellationTokenSource.Cancel();
			_serviceStatusMonitor = null;
			return;
		}
		if (_serviceStatusMonitor != null)
		{
			_serviceStatusMonitor.PollingDelay = PollingDelay;
		} else
		{
			RecreateMonitor();
		}
	}
}