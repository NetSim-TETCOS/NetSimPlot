using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NetSimPlot
{
    public class ImageSave
    {
        public ImageSave(Grid grid)
        {
            string filename;
            if (!OxyPlotInterface.cmdValue.TryGetValue("plotfilename", out string x))
                x = "plot.png";
            if (OxyPlotInterface.cmdValue.TryGetValue("path", out string path))
                filename = path + x;
            else
                filename = x;
            RenderTargetBitmap bitmap = GetImage(grid);
            Stream stream = new FileStream(filename, FileMode.Create);
            SaveAsPng(bitmap, stream);
            stream.Close();
            if (OxyPlotInterface.isUIShown)
                System.Diagnostics.Process.Start(filename);
        }
        public static RenderTargetBitmap GetImage(Grid view)
        {
            System.Windows.Size size = new System.Windows.Size(2048, 2048 * (view.ActualHeight / view.ActualWidth));// new System.Windows.Size(view.ActualWidth, view.ActualHeight);
            if (size.IsEmpty)
                return null;

            double dpiScale = 300.0 / 96;

            double dpiX = 300.0;
            double dpiY = 300.0;

            RenderTargetBitmap result = new RenderTargetBitmap(Convert.ToInt32(size.Width * dpiScale),
                Convert.ToInt32(size.Height * dpiScale),
                dpiX, dpiY, PixelFormats.Pbgra32);

            DrawingVisual drawingvisual = new DrawingVisual();
            using (DrawingContext context = drawingvisual.RenderOpen())
            {
                context.DrawRectangle(new VisualBrush(view), null, new Rect(new System.Windows.Point(), size));
                context.Close();
            }

            result.Render(drawingvisual);
            return result;
        }

        public static void SaveAsPng(RenderTargetBitmap src, Stream outputStream)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder
            {
                QualityLevel = 90
            };
            encoder.Frames.Add(BitmapFrame.Create(src));
            encoder.Save(outputStream);
        }
    }
}
