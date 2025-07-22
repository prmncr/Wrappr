using Microsoft.UI.Xaml.Controls;
using Microsoft.Win32.TaskScheduler;
using Wrappr.Data;
using Wrappr.Resources;
using Wrappr.Services;

namespace Wrappr.Components.Pages;

public partial class PreferencesSubPage
{
	private const string TaskName = "Wrappr";
	private const string SilentArg = "--silent";
	private readonly TaskService _taskService = new();
	private bool _suppressNextChange;

	public PreferencesSubPage()
	{
		AutostartType selectedType;
		InitializeComponent();
		var task = _taskService.FindTask(TaskName);
		if (task != null)
		{
			if (task.Definition.Actions.Count != 1)
			{
				selectedType = AutostartType.Disabled;
			} else
			{
				var action = task.Definition.Actions.First();
				if (action.ActionType == TaskActionType.Execute && ((ExecAction)action).Path == Environment.ProcessPath)
				{
					selectedType = ((ExecAction)action).Arguments == SilentArg ? AutostartType.HideAtStart : AutostartType.ShowAtStart;
				} else
				{
					selectedType = AutostartType.Disabled;
				}
			}
		} else
		{
			selectedType = AutostartType.Disabled;
		}

		AutostartComboBox.ItemsSource = new List<AutostartType>
		{
			AutostartType.Disabled,
			AutostartType.ShowAtStart,
			AutostartType.HideAtStart
		};
		AutostartComboBox.DisplayMemberPath = nameof(AutostartType.Localized);
		_suppressNextChange = true;
		AutostartComboBox.SelectedItem = selectedType;
		AutostartComboBox.IsEnabled = Elevation.IsElevated;
	}

	private void OnAutostartSettingChanged(object sender, SelectionChangedEventArgs e)
	{
		if (_suppressNextChange)
		{
			_suppressNextChange = false;
			return;
		}
		switch ((AutostartType)AutostartComboBox.SelectedItem)
		{
			case AutostartType.DisabledAutostartType:
				_taskService.RootFolder.DeleteTask(TaskName, false);
				break;
			case AutostartType.HideAtStartAutostartType:
				CreateTask(true);
				break;
			case AutostartType.ShowAtStartAutostartType:
				CreateTask(false);
				break;
		}
	}

	private void CreateTask(bool silent)
	{
		try
		{
			var task = _taskService.NewTask();
			task.Triggers.Add(new LogonTrigger { Delay = new TimeSpan(0, 0, 0, 10) });
			task.Actions.Add(silent ? new ExecAction(Environment.ProcessPath!, SilentArg) : new ExecAction(Environment.ProcessPath!));
			task.Principal.RunLevel = TaskRunLevel.Highest;
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

	private abstract class AutostartType(string localizedName)
	{
		public static readonly AutostartType Disabled = new DisabledAutostartType();
		public static readonly AutostartType ShowAtStart = new ShowAtStartAutostartType();
		public static readonly AutostartType HideAtStart = new HideAtStartAutostartType();
		public string Localized => localizedName;

		public class DisabledAutostartType() : AutostartType(Strings.AutostartDisabled);

		public class ShowAtStartAutostartType() : AutostartType(Strings.ShowAtStart);

		public class HideAtStartAutostartType() : AutostartType(Strings.HideAtStart);
	}
}