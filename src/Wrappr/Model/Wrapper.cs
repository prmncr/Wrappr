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

public partial class Wrapper : ObservableObject
{
	private ServiceStatusMonitor? _serviceStatusMonitor;

	public Wrapper() { }

	public Wrapper(WrapperConfig config)
	{
		PollingDelay = config.PollingDelay;
		IsTrackingEnabled = config.Tracked;
		IsNotificationsEnabled = config.Notified;

		if (config.Name == null)
		{
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

		if (Service == null)
		{
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
		set
		{
			OnPropertyChanging();
			field = value;
			ServiceToggled?.Invoke(value);
			OnPropertyChanged();
		}
	}

	public bool IsTrackingEnabled
	{
		get;
		private set
		{
			field = value;
			UpdateWrapper();
			OnPropertyChanged();
		}
	}

	public bool IsNotificationsEnabled
	{
		get;
		private set
		{
			field = value;
			UpdateWrapper();
			OnPropertyChanged();
		}
	}

	public int PollingDelay
	{
		get;
		set
		{
			field = value;
			UpdateWrapper();
			OnPropertyChanged();
		}
	}

	public event Action<bool>? ServiceToggled;

	private void UpdateWrapper()
	{
		UpdateMonitor();
		Wrappers.Instance.Save();
	}

	[RelayCommand]
	public async Task ToggleService(bool switchedTo)
	{
		if (Service == null)
		{
			Snackbars.ShowSnackbar(new SnackbarData(Strings.ErrorMessageTitle, InfoBarSeverity.Error, Strings.ServiceNotFoundMessage));
			return;
		}
		string? message;
		_serviceStatusMonitor?.Pause();
		if (switchedTo)
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
		Enabled = Service.Status == ServiceControllerStatus.Running;
		_serviceStatusMonitor?.Resume();
		if (message != null)
		{
			Snackbars.ShowSnackbar(new SnackbarData(Strings.ErrorMessageTitle, InfoBarSeverity.Error, message));
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

	[RelayCommand]
	private void DeleteWrapper()
	{
		Wrappers.Instance.Storage.Remove(this);
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
				Balloons.ShowBalloonMessage(
					new BalloonMessageData
					{
						Title = Strings.ServiceStatusWasChangedBalloonTitle,
						Message = string.Format(Strings.ServiceStatusWasChangedBalloonText, Service?.ServiceName, newStatus),
						Icon = NotificationIcon.Info
					}
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