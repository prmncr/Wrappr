using System.Windows.Controls;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Wrappr.Components.MainMenu;

namespace Wrappr.Utils.UI;

public partial class Messages : ReactiveObject {
	private static Messages? _instance;

	[Reactive] private string? _text;

	[Reactive] private string? _title;
	public static Messages Instance => _instance ??= new Messages();

	public static void ShowMessage(string title, string text) {
		Instance.Title = title;
		Instance.Text = text;
		MainMenuWindow.Instance.DrawerInvoker.Command.Execute(Dock.Bottom);
	}
}