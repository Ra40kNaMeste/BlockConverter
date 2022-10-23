using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using BlocksLib.Blocks;

namespace BlockConverter.Tools
{
    class BitmapConverter
    {
        public static RenderTargetBitmap GetRenderTargetBitMaps(FrameworkElement element)
        {
            element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)element.DesiredSize.Width + 1, (int)element.DesiredSize.Height + 1, 96, 96, PixelFormats.Default);
            element.Arrange(new Rect(element.DesiredSize));
            bitmap.Render(element);
            return bitmap;
        }
        
    }
}
