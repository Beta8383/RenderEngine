using SkiaSharp;
using System.Windows;
using SkiaSharp.Views.Desktop;
using System;
using RenderEngine.Core;

namespace RenderEngine.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int count = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKPixmap pixmap = e.Surface.PeekPixels();
            pixmap.Erase(new SKColor(255, 0, 0));
            IntPtr bmpPixelsPtr = pixmap.GetPixels();
            count++;
            Title = count.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Canvas.InvalidateVisual();
        }
    }
}
