using Windows.Foundation;
using Windows.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.UI.Composition;
using Microsoft.Graphics.DirectX;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace Wrappr.Utilities;

public partial class BackgroundGradientBrush(Window window) : XamlCompositionBrushBase
{
	protected override void OnConnected()
	{
		var compositor = window.Compositor;
		var size = window.AppWindow.Size;
		var unit = new SizeUnit(size);

		var canvasDevice = CanvasDevice.GetSharedDevice();
		var drawingDevice = CanvasComposition.CreateCompositionGraphicsDevice(compositor, canvasDevice);
		var drawingSurface = drawingDevice.CreateDrawingSurface2(size, DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Premultiplied);

		using (var ds = CanvasComposition.CreateDrawingSession(drawingSurface))
		{
			ds.Clear(Colors.Transparent);
			ds.FillRectangle(
				new Rect(0, 0, size.Width, size.Height),
				new CanvasRadialGradientBrush(ds, Color.FromArgb(255, 19, 66, 148), Colors.Transparent)
				{
					Center = unit * (0.65f, -0.1f),
					RadiusX = unit.D(0.6f),
					RadiusY = unit.D(0.6f)
				}
			);
			ds.FillRectangle(
				new Rect(0, 0, size.Width, size.Height),
				new CanvasRadialGradientBrush(ds, Color.FromArgb(170, 0, 0, 255), Colors.Transparent)
                {
                	Center = unit * (-0.1f, 0.7f),
                	RadiusX = unit.D(1f),
                	RadiusY = unit.D(1f)
                }
			);

			ds.FillRectangle(
				new Rect(0, 0, size.Width, size.Height),
				new CanvasRadialGradientBrush(ds, Colors.Purple, Colors.Transparent)
				{
					Center = unit * (1.05f, 0.4f),
					RadiusX = unit.D(0.4f),
					RadiusY = unit.D(0.4f)
				}
			);

			ds.FillRectangle(new Rect(0, 0, size.Width, size.Height), Color.FromArgb(40, 0, 0, 0));
		}

		var brush = compositor.CreateSurfaceBrush(drawingSurface);
		brush.Stretch = CompositionStretch.UniformToFill;
		CompositionBrush = brush;
	}

	protected override void OnDisconnected()
	{
		CompositionBrush?.Dispose();
		CompositionBrush = null;
	}
}