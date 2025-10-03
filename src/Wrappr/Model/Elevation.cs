using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;

namespace Wrappr.Model;

public static class Elevation
{
	private static ICommand? _elevateCommand;
	public static bool IsElevated { get; } = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

	public static ICommand ElevateCommand => _elevateCommand ??= new RelayCommand(Elevate);

	private static void Elevate()
	{
		var processInfo = new ProcessStartInfo
		{
			Verb = "runas",
			UseShellExecute = true,
			FileName = Environment.ProcessPath
		};

		Process.Start(processInfo);
		Application.Current.Exit();
	}
}