using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlocksLib.Blocks.Shapes
{
    /// <summary>
    /// Логика взаимодействия для Hexagon.xaml
    /// </summary>
    public partial class Hexagon : Shape
    {
        //содаёт шестиугольник для HoneycomPanel
        protected override Geometry DefiningGeometry
        {
            get
            {
                double h = Height, w = Width;
                return new PathGeometry(new List<PathFigure>()
                {
                    new PathFigure(new Point(0,h/2),new List<PolyLineSegment>()
                    {
                        new PolyLineSegment(new List<Point>()
                        {
                            new Point(h/2, 0),
                            new Point(w - h/2, 0),
                            new Point(w, h/2),
                            new Point(w-h/2, h),
                            new Point(h/2, h)

                        }, true)
                    },true)
                });

            }
        }
    }
}
