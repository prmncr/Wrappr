using Windows.Graphics;
using Wrappr.Services;

namespace Wrappr.Components.Windows;

public partial class MainWindow {
	public MainWindow() {
		ApplicationWindowProvider.Initialize(this);
		InitializeComponent();
		AppWindow.Resize(new SizeInt32(580, 800));
		AppWindow.SetIcon("Assets/Images/logo32.ico");
	}
}