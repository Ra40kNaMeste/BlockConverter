using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BlocksLib.Blocks.Shapes
{
    public enum RotateDirection
    {
        Up, Down, Left, Right
    }
    /// <summary>
    /// Логика взаимодействия для Arrow.xaml
    /// </summary>
    public partial class Arrow : Shape
    {
        public static readonly DependencyProperty LengthProperty;
        public double Length
        {
            get { return (double)GetValue(LengthProperty); }
            set
            {
                SetValue(LengthProperty, value);
            }
        }

        public static readonly DependencyProperty StartPointProperty;
        public double StartPoint
        {
            get { return (double)GetValue(StartPointProperty); }
            set
            {
                SetValue(StartPointProperty, value);
            }
        }
        public static readonly DependencyProperty RotateProperty;
        public RotateDirection Rotate
        {
            get { return (RotateDirection)GetValue(RotateProperty); }
            set { SetValue(RotateProperty, value); }
        }

        static Arrow()
        {
            LengthProperty = DependencyProperty.Register("Length", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata((double)1 / 2, FrameworkPropertyMetadataOptions.AffectsRender));
            StartPointProperty = DependencyProperty.Register("StartPoint", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata((double)0, FrameworkPropertyMetadataOptions.AffectsRender));
            RotateProperty = DependencyProperty.Register("Rotate", typeof(RotateDirection), typeof(Arrow), new FrameworkPropertyMetadata(RotateDirection.Down, FrameworkPropertyMetadataOptions.AffectsRender));
        }
        protected override Geometry DefiningGeometry
        {
            get
            {
                double h = ActualHeight, w = ActualWidth;
                List<Point> geometry = Rotate switch
                {
                    RotateDirection.Down => GetGeometry(h, w),
                    RotateDirection.Up => GetGeometry(-h, w),
                    RotateDirection.Left => GetGeometry(-w, h),
                    RotateDirection.Right => GetGeometry(w, h),
                    _ => GetGeometry(h, w)
                };
                Func<Point, Point> foo = Rotate switch
                {
                    RotateDirection.Down => (Point point) => point,
                    RotateDirection.Up => (Point point) => new Point(point.X, point.Y + h),
                    RotateDirection.Left => (Point point) => new Point(point.Y + w, point.X),
                    RotateDirection.Right => (Point point) => new Point(point.Y, point.X),
                    _ => (Point point) => point
                };
                int count = geometry.Count;
                for (int i = 0; i < count; i++)
                    geometry[i] = foo(geometry[i]);
                Point point = geometry[0];
                geometry.RemoveAt(0);
                return new PathGeometry(new List<PathFigure>()
                {
                    new PathFigure(point,new List<PolyLineSegment>()
                    {
                        new PolyLineSegment(geometry, true)
                    }, true)
                });
            }
        }
        protected override Size MeasureOverride(Size constraint)
        {
            Size size = base.MeasureOverride(constraint);
            size.Width = Math.Min(constraint.Width, size.Width);
            size.Height = Math.Min(constraint.Height, size.Height);
            return size;
        }
        private List<Point> GetGeometry(double h, double w)
        {
            double wArrow = Math.Min(Length, w);
            double length = h < 0 ? -Length : Length;
            return new List<Point>()
            {
                new Point(w/2, h),
                new Point((w - wArrow) / 2, h - length),
                new Point(w / 2, h - length),
                new Point(w / 2, StartPoint),
                new Point(w / 2, h - length),
                new Point((wArrow + w) / 2, h - length)
            };
        }
    }
}
