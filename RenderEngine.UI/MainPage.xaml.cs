using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace RenderEngine.UI;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    int times = 0;
    bool update=false;

    unsafe void Canvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        if (!update) return;
        SKPixmap pixmap = e.Surface.PeekPixels();
        
        pixmap.Erase(new SKColor(0, 0, 0));
        nint pixelsPtr = pixmap.GetPixels();
        Span<byte> a = new Span<byte>(pixelsPtr.ToPointer(), pixmap.BytesSize);
        int height = pixmap.Height;
        int width = pixmap.Width;
        for (int i = 0; i <= times * 100; i++)
            for (int j = 0; j <= 200; j++)
            {
                a[i * width * pixmap.BytesPerPixel + j * pixmap.BytesPerPixel] = 255;
                a[i * width * pixmap.BytesPerPixel + j * pixmap.BytesPerPixel + 1] = 255;
                a[i * width * pixmap.BytesPerPixel + j * pixmap.BytesPerPixel + 2] = 255;
            }
        Message.Text = times + "Width:" + width + "Height:" + height +"BytesPerPixel:" + pixmap.BytesPerPixel;
        times++;
        update = false;
    }

    void Button_Clicked(System.Object sender, System.EventArgs e)
    {
        update = true;
        Canvas.InvalidateSurface();
    }
}


