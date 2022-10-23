using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BlocksLib.Blocks.Shapes
{
    class Rhombus : Shape
    {
        static Rhombus()
        {
            AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(Rhombus), new FrameworkPropertyMetadata(120.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender), ValidateAngle);
            SideProperty = DependencyProperty.Register("Side", typeof(double), typeof(Rhombus), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender), ValidateSide);
        }
        public static readonly DependencyProperty AngleProperty;
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }
        private static bool ValidateAngle(object value)
        {
            double val = (double)value;
            return val > 0 && val < 180;
        }
        public static readonly DependencyProperty SideProperty;
        public double Side
        {
            get { return (double)GetValue(SideProperty); }
            set { SetValue(SideProperty, value); }
        }
        private static bool ValidateSide(object value)
        {
            return (double)value >= 0;
        }

        protected override Geometry DefiningGeometry
        {
            get
            {

                double angle = Angle * Math.PI / 180;
                double x1 = Side * Math.Sin(angle / 2);
                double y1 = Side * Math.Cos(angle / 2);

                return new PathGeometry(new List<PathFigure>()
                {
                    new PathFigure(new Point(0, y1),
                    new List<LineSegment>()
                    {
                        new LineSegment(new Point(x1, 0), true),
                        new LineSegment(new Point(2 * x1, y1), true),
                        new LineSegment(new Point(x1, 2 * y1), true)
                    }, true)
                });
            }
        }
    }
}
