using Windows.Graphics;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Wrappr.Services;
using Wrappr.Utilities;

namespace Wrappr.Components.Windows;

public partial class MainWindow {
	public MainWindow() {
		ApplicationWindowProvider.Initialize(this);
        var presenter = AppWindow.Presenter as OverlappedPresenter ?? throw new Exception();

        AppWindow.Resize(new SizeInt32(Properties.Settings.WindowPosition.Width, Properties.Settings.WindowPosition.Height));
        AppWindow.Move(new PointInt32(Properties.Settings.WindowPosition.PosX, Properties.Settings.WindowPosition.PosY));
		AppWindow.SetIcon("Assets/Images/logo32.ico");

		OneTimeEventExecutor<WindowActivatedEventArgs>.Create(
			it => Activated += it,
			it => Activated -= it,
			(_, _) => {
				if (Properties.Settings.WindowPosition.IsMaximized)
					presenter.Maximize();
			}
		);

        Closed += (_, _) =>
        {
            Properties.Settings.WindowPosition.Width = AppWindow.Size.Width;
            Properties.Settings.WindowPosition.Height = AppWindow.Size.Height;
            Properties.Settings.WindowPosition.PosX = AppWindow.Position.X;
            Properties.Settings.WindowPosition.PosY = AppWindow.Position.Y;
            Properties.Settings.WindowPosition.IsMaximized = presenter.State == OverlappedPresenterState.Maximized;
            Properties.Save();
        };

        InitializeComponent();
	}
}