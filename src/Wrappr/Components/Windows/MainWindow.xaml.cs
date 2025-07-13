using Windows.Graphics;
using Microsoft.UI.Xaml;
using Wrappr.Services;
using Wrappr.Utilities;

namespace Wrappr.Components.Windows;

public partial class MainWindow
{
	public MainWindow()
	{
		ApplicationWindowProvider.Initialize(this);
		var size = new SizeInt32(450, 550);
		AppWindow.Resize(size);
		AppWindow.SetIcon("Assets/Images/logo32.ico");
		InitializeComponent();
	}

	private void FrameLoaded(object sender, RoutedEventArgs e)
	{
		MainFrame.Background = new BackgroundGradientBrush(this);
	}
}