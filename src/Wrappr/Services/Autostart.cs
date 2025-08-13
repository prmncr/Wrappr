using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Win32.TaskScheduler;
using Wrappr.Data;
using Wrappr.Resources;

namespace Wrappr.Services;

public partial class Autostart : ObservableObject
{
	[ObservableProperty]
	public partial bool IsEnabled { get; set; }
	private const string TaskName = "Wrappr";
	private readonly TaskService _taskService = new();

	public Autostart()
	{
		var task = _taskService.FindTask(TaskName);
		if (task is not { Definition.Actions.Count: 1 }) return;
		var action = task.Definition.Actions.First();
		if (action.ActionType == TaskActionType.Execute && ((ExecAction)action).Path == Environment.ProcessPath)
		{
			IsEnabled = true;
		}
	}

	[RelayCommand]
	private void ToggleAutostart(bool switchedFrom)
	{
		if (switchedFrom)
		{
			_taskService.RootFolder.DeleteTask(TaskName, false);
		} else
		{
			CreateTask();
		}
		IsEnabled = !switchedFrom;
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
				Snackbars.ShowSnackbar(
					new SnackbarData(
						Strings.AddedToAutoStartTitle,
						InfoBarSeverity.Success
					)
				);
			}
		} catch (Exception exception)
		{
			Snackbars.ShowSnackbar(exception);
		}
	}
}