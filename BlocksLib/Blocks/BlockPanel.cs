using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BlocksLib.Blocks
{
    public partial class BlockPanel : Panel, IHorizontalCenter
    {
        static BlockPanel()
        {
            HorizontalCenterPropertyKey = DependencyProperty.RegisterReadOnly("HorizontalCenter", typeof(double), typeof(BlockPanel),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
            IsStretchLastElementProperty = DependencyProperty.Register("IsStretchLastElement", typeof(bool), typeof(BlockPanel), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
            CenterProperty = DependencyProperty.RegisterAttached("Center", typeof(double), typeof(BlockPanel),
                new FrameworkPropertyMetadata(-0.1, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure));
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double maxWidthLeft = 0, h = 0, maxWidthRight = 0, maxWidth = 0;
            Size size;
            foreach (UIElement item in InternalChildren)
            {
                item.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                size = item.DesiredSize;
                double center = FindHorizontalCenter(item);

                if(center >= 0)
                {
                    maxWidthLeft = Math.Max(maxWidthLeft, center);
                    maxWidthRight = Math.Max(maxWidthRight, size.Width - center);
                }
                else
                    maxWidth = Math.Max(maxWidth, size.Width);
                h += size.Height;
            }
            double width = Math.Max(maxWidthLeft, maxWidth / 2);
            HorizontalCenter = width;
            sizePanel = new Size(width + Math.Max(maxWidthRight, maxWidth / 2), h);
            return sizePanel;
        }

        public static readonly DependencyProperty CenterProperty;
        public static double GetCenter(UIElement element) => (double)element.GetValue(CenterProperty);
        public static void SetCenter(UIElement element, double value) => element.SetValue(CenterProperty, value);

        private Size sizePanel;

        public static readonly DependencyProperty IsStretchLastElementProperty;
        public bool IsStretchLastElement
        {
            get { return (bool)GetValue(IsStretchLastElementProperty); }
            set { SetValue(IsStretchLastElementProperty, value); }
        }
        public static readonly DependencyPropertyKey HorizontalCenterPropertyKey;
        public double HorizontalCenter
        {
            get { return (double)GetValue(HorizontalCenterPropertyKey.DependencyProperty); }
            private set { SetValue(HorizontalCenterPropertyKey, value); }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double h = 0;
            int length = InternalChildren.Count - 1;
            int i = 0;

            foreach (UIElement item in InternalChildren)
            {
                item.Arrange(new Rect(item.DesiredSize));
                ArrageElement(item, item.DesiredSize, h);
                
                h += item.RenderSize.Height;

                i++;
                if (IsStretchLastElement && i == length)
                    break;
            }
            if(IsStretchLastElement)
            {
                double freeHeight = finalSize.Height - sizePanel.Height;
                var item = InternalChildren[length];
                Size size = item.DesiredSize;
                size.Height += freeHeight;
                ArrageElement(item, size, h);
            }
            return new Size(sizePanel.Width, finalSize.Height);
        }
        private void ArrageElement(UIElement element, Size size, double height)
        {
            double center = FindHorizontalCenter(element);
            if (center >= 0)
            {
                element.Arrange(new Rect(new Point(HorizontalCenter - center, height), size));
            }
            else
                element.Arrange(GetRect(new Point(HorizontalCenter, height), size));

        }

        /// <summary>
        /// находит элемент, котороый учитывается при компановке
        /// </summary>
        /// <param name="element"></param>
        /// <param name="center">результат</param>
        /// <returns>получилось ли найти</returns>
        private static double FindHorizontalCenter(UIElement element)
        {                
            if (element is IHorizontalCenter temp)
                return temp.HorizontalCenter;
            //if (element is ContentPresenter temp2 && temp2.Content is UIElement temp3)
            //    return FindHorizontalCenter(temp3);
            double center;
            if ((center = GetCenter(element)) > 0)
                return center;
            if (element is FrameworkElement el)
                return el.DesiredSize.Width / 2;
            return 0;
        }

        private Rect GetRect(Point horizontalCenter, Size size)
        {
            return new Rect(new Point(horizontalCenter.X - size.Width / 2, horizontalCenter.Y), size);
        }
    }

}
