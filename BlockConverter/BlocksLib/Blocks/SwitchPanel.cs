using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BlocksLib.Blocks
{
    public partial class SwitchPanel : ItemsControl, IHorizontalCenter
    {
        public static readonly DependencyPropertyKey HorizontalCenterProperty = DependencyProperty.RegisterReadOnly
            ("HorizontalCenter", typeof(double), typeof(SwitchPanel), new FrameworkPropertyMetadata(0.0,
                FrameworkPropertyMetadataOptions.NotDataBindable | FrameworkPropertyMetadataOptions.AffectsParentMeasure));
        public double HorizontalCenter
        {
            get { return (double)GetValue(HorizontalCenterProperty.DependencyProperty); }
            private set { SetValue(HorizontalCenterProperty, value); }
        }

        public static readonly DependencyPropertyKey HCExtremeElementsProperty = DependencyProperty.RegisterReadOnly
            ("HCExtremeElements", typeof(Thickness), typeof(SwitchPanel), new FrameworkPropertyMetadata(new Thickness(),
                FrameworkPropertyMetadataOptions.NotDataBindable | FrameworkPropertyMetadataOptions.AffectsArrange));
        public Thickness HCExtremeElements
        {
            get { return (Thickness)GetValue(HCExtremeElementsProperty.DependencyProperty); }
            private set { SetValue(HCExtremeElementsProperty, value); }
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Size res = base.ArrangeOverride(arrangeBounds);
            SetCustomProperties(res);
            return res;
        }
        private void SetCustomProperties(Size size)
        {
            var items = Items;
            Thickness hcExtremeElements = new();
            if (items.Count != 0)
            {
                if (items[0] is SwitchBlockItem first)
                {
                    first.PropertyChanged += OnChangedFirstElement;
                    FrameworkElement temp = (FrameworkElement)ItemContainerGenerator.ContainerFromItem(first);
                    hcExtremeElements.Left = first.Value.ActualWidth + 21 < temp.ActualWidth ? temp.ActualWidth / 2 : first.Value.HorizontalCenter + 10;
                }

                if (items[items.Count - 1] is SwitchBlockItem last)
                {
                    last.PropertyChanged += OnChangedLastElement;
                    FrameworkElement temp = (FrameworkElement)ItemContainerGenerator.ContainerFromItem(last);
                    hcExtremeElements.Right = last.Value.ActualWidth + 21 < temp.ActualWidth ? temp.ActualWidth / 2 : last.Value.ActualWidth - last.Value.HorizontalCenter + 10;

                }
                HCExtremeElements = hcExtremeElements;
                HorizontalCenter = (size.Width + hcExtremeElements.Left - hcExtremeElements.Right) / 2;
            }
        }
        private void OnChangedFirstElement(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "HorizontalCenter" && sender is SwitchBlockItem item)
            {
                Thickness thickness = HCExtremeElements;
                FrameworkElement temp = (FrameworkElement)ItemContainerGenerator.ContainerFromItem(item);
                thickness.Left = item.Value.ActualWidth + 21 < temp.ActualWidth ? temp.ActualWidth / 2 : item.Value.HorizontalCenter + 10;
                SetProperties(ActualWidth, thickness);
            }
        }
        private void OnChangedLastElement(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "HorizontalCenter" && sender is SwitchBlockItem item)
            {
                Thickness thickness = HCExtremeElements;
                FrameworkElement temp = (FrameworkElement)ItemContainerGenerator.ContainerFromItem(item);
                thickness.Right = item.Value.ActualWidth + 21 < temp.ActualWidth ? temp.ActualWidth / 2 : item.Value.ActualWidth - item.Value.HorizontalCenter + 10;
                SetProperties(ActualWidth, thickness);
            }
        }

        private void SetProperties(double width, Thickness thickness)
        {
            HCExtremeElements = thickness;
            HorizontalCenter = (width + thickness.Left - thickness.Right) / 2;
        }
    }
}
