using System.Diagnostics;
using System.Reflection;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace Wrappr.Components.About;

public partial class About : ReactiveObject {
	[Reactive] private string _version = Assembly.GetExecutingAssembly().GetName().Version!.ToString(2);

	[ReactiveCommand]
	private static void OpenLink(string url) {
		Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
	}
}