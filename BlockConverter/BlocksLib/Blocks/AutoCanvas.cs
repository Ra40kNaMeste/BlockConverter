using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BlocksLib.Blocks
{
    public partial class AutoCanvas : Panel
    {
        static AutoCanvas()
        {
            MaxBindingHeightProperty = DependencyProperty.RegisterAttached("MaxBindingHeight", typeof(int), typeof(AutoCanvas), new FrameworkPropertyMetadata(0,
                FrameworkPropertyMetadataOptions.AffectsMeasure));
            TopProperty = DependencyProperty.RegisterAttached("Top", typeof(double), typeof(AutoCanvas), new FrameworkPropertyMetadata(0.0,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure));
            LeftProperty = DependencyProperty.RegisterAttached("Left", typeof(double), typeof(AutoCanvas), new FrameworkPropertyMetadata(0.0,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        }
        public static readonly DependencyProperty MaxBindingHeightProperty;
        public static int GetMaxBindingHeight(UIElement element) => (int)element.GetValue(MaxBindingHeightProperty);
        public static void SetMaxBindingHeight(UIElement element, int value) => element.SetValue(MaxBindingHeightProperty, value);

        public static readonly DependencyProperty TopProperty;
        public static double GetTop(UIElement element) => (double)element.GetValue(TopProperty);
        public static void SetTop(UIElement element, double value) => element.SetValue(TopProperty, value);
        public static readonly DependencyProperty LeftProperty;
        public static double GetLeft(UIElement element) => (double)element.GetValue(LeftProperty);
        public static void SetLeft(UIElement element, double value) => element.SetValue(LeftProperty, value);
        private Dictionary<int, double> HeightCaseElements;
        protected override Size MeasureOverride(Size availableSize)
        {
            HeightCaseElements = new();
            Size sizeCanvas = new Size();

            foreach (FrameworkElement item in InternalChildren)
            {
                item.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size size = item.DesiredSize;
                double top = GetTop(item);
                double left = GetLeft(item);

                //проверка на попадание двойной привязки по высоте

                int val;
                if ((val = GetMaxBindingHeight(item)) != 0 )
                {
                    if (!HeightCaseElements.ContainsKey(val) || HeightCaseElements[val] < item.DesiredSize.Height)
                        HeightCaseElements[val] = item.DesiredSize.Height;
                    else
                        size.Height = HeightCaseElements[val];
                }

                sizeCanvas.Width = Math.Max(size.Width + left, sizeCanvas.Width);
                sizeCanvas.Height = Math.Max(size.Height + top, sizeCanvas.Height);
            }
            return sizeCanvas;
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            Size size = new Size();
            int val;
            foreach (UIElement item in InternalChildren)
            {
                double top = GetTop(item);
                double left = GetLeft(item);

                if ((val=GetMaxBindingHeight(item)) != 0)
                {
                    item.Arrange(new Rect(new Point(left, top), new Size(item.DesiredSize.Width, HeightCaseElements[val])));
                }
                else
                    item.Arrange(new Rect(new Point(left, top), item.DesiredSize));
                size.Width = Math.Max(size.Width, item.RenderSize.Width + left);
                size.Height = Math.Max(size.Height, item.RenderSize.Height + top);
            }
            return size;
        }
    }

}
