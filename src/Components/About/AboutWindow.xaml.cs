using System.Diagnostics;
using System.Windows;

namespace Wrappr.Components.About;

public partial class AboutWindow {
	public AboutWindow() {
		InitializeComponent();
	}

	private void OpenLink(object sender, RoutedEventArgs e) {
		OpenLink("https://www.flaticon.com/free-icons/settings");
	}

	private void OpenRepository(object sender, RoutedEventArgs e) {
		OpenLink("https://github.com/prmncr/Wrappr");
	}

	private static void OpenLink(string url) {
		Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
	}
}