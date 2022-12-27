using SkiaSharp;

namespace RenderEngine.UI;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    void Canvas_PaintSurface(System.Object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
    {
        SKPixmap pixmap = e.Surface.PeekPixels();
        pixmap.Erase(new SKColor(255, 0, 0));
    }
}


