using System.ServiceProcess;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Wrappr.Data;
using Wrappr.Resources;
using Wrappr.Model;
using Wrappr.Interop;

namespace Wrappr.Model;

public partial class Wrapper : ObservableObject
{
	public WrapperSettings Settings { get; private set; }

	public string ServiceName { get; } = Strings.EmptyWrapperServiceName;

	public string DisplayName { get; } = Strings.EmptyWrapperDisplayName;

	[ObservableProperty]
	public partial bool IsWaitingForStatusChange { get; private set; }

	[ObservableProperty]
	public partial bool Enabled { get; set; }

	public event Action<bool>? ServiceToggled;

	public Wrapper()
	{
		Settings = new WrapperSettings();
	}

	public Wrapper(WrapperConfig config)
	{
		Settings = config;

		if (config.Name == null)
		{
			ServiceName = Strings.EmptyWrapperDisplayName;
			DisplayName = Strings.EmptyWrapperDisplayName;
			return;
		}

		Service = Services.GetAll().FirstOrDefault(service => config.Name == service!.ServiceName, null);
		Enabled = Service?.Status == ServiceControllerStatus.Running;
		ServiceName = Service?.ServiceName ?? Strings.EmptyWrapperServiceName;
		DisplayName = Service?.DisplayName ?? Strings.EmptyWrapperDisplayName;

		if (Service != null)
		{
			CreateMonitor();
		}
	}

	private ServiceController? Service { get; }

	private ServiceStatusMonitor? _serviceStatusMonitor;

	[RelayCommand]
	private async Task UpdateWrapper()
	{
		await WrappersStorage.Save();
		UpdateMonitor();
	}

	[RelayCommand]
	private void RevertChanges()
	{
		Settings = WrappersStorage.GetBackupFor(this);
	}

	[RelayCommand]
	private async Task ToggleService()
	{
		var switchedTo = !Enabled;
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
		Enabled = switchedTo;
		ServiceToggled?.Invoke(Enabled);
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

	private void CreateMonitor()
	{
		if (Service == null) return;
		if (!Settings.Tracked) return;
		var dispatcher = DispatcherQueue.GetForCurrentThread();

		_serviceStatusMonitor = new ServiceStatusMonitor(
			Service,
			Settings.Notified,
			Settings.PollingDelay,
			() => dispatcher.TryEnqueue(DispatcherQueuePriority.High, () => Enabled = Service.Status == ServiceControllerStatus.Running)
		);
		_ = Task.Run(_serviceStatusMonitor!.WatchServiceStatus, _serviceStatusMonitor.CancellationTokenSource.Token);
	}

	private void UpdateMonitor()
	{
		if (!Settings.Tracked)
		{
			_serviceStatusMonitor?.CancellationTokenSource.Cancel();
			_serviceStatusMonitor = null;
			return;
		}
		if (_serviceStatusMonitor != null)
		{
			_serviceStatusMonitor.PollingDelay = Settings.PollingDelay;
			_serviceStatusMonitor.Notify = Settings.Notified;
		} else
		{
			CreateMonitor();
		}
	}
}