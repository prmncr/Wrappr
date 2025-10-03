using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32.TaskScheduler;
using Wrappr.Data;
using Wrappr.Models;
using Wrappr.Resources;

namespace Wrappr.ViewModels;

public partial class Autostart : ObservableObject
{
	public bool IsEnabled
	{
		get => _isEnabled;
		set
		{
			OnPropertyChanging();
			if (!value)
			{
				_taskService.RootFolder.DeleteTask(TaskName, false);
				Notifications.ShowNearestNotification(
					new Notification(
						Strings.DeletedFromAutoStartTitle,
						NotificationSeverity: Notification.Severity.Success
					)
				);
			} else
			{
				CreateTask();
			}
			_isEnabled = value;
			OnPropertyChanged();
		}
	}

	private bool _isEnabled;
	private const string TaskName = "Wrappr";
	private readonly TaskService _taskService = new();

	public Autostart()
	{
		var task = _taskService.FindTask(TaskName);
		if (task is not { Definition.Actions.Count: 1 }) return;
		var action = task.Definition.Actions.First();
		if (action.ActionType == TaskActionType.Execute && ((ExecAction)action).Path == Environment.ProcessPath)
		{
			_isEnabled = true;
		}
	}

	private void CreateTask()
	{
		try
		{
			var task = _taskService.NewTask();
			task.Triggers.Add(new LogonTrigger { Delay = new TimeSpan(0, 0, 0, 10) });
			task.Actions.Add(new ExecAction(Environment.ProcessPath!));
			task.Principal.RunLevel = TaskRunLevel.Highest;
			task.Settings.IdleSettings.StopOnIdleEnd = false;
			task.Settings.StopIfGoingOnBatteries = false;
			task.Settings.DisallowStartIfOnBatteries = false;
			task.Settings.StartWhenAvailable = true;
			task.Settings.ExecutionTimeLimit = TimeSpan.Zero;
			var createdTask = _taskService.RootFolder.RegisterTaskDefinition(
				TaskName,
				task,
				TaskCreation.CreateOrUpdate,
				null,
				null,
				TaskLogonType.InteractiveToken
			);
			if (createdTask != null)
			{
				Notifications.ShowNearestNotification(
					new Notification(
						Strings.AddedToAutoStartTitle,
						NotificationSeverity: Notification.Severity.Success
					)
				);
			}
		} catch (Exception exception)
		{
			Notifications.ShowNearestNotification(exception);
		}
	}
}