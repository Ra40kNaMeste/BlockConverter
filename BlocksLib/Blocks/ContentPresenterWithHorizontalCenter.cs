using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BlocksLib.Blocks
{
    class ContentPresenterWithHorizontalCenter : ContentPresenter, IHorizontalCenter
    {
        public static readonly DependencyPropertyKey HorizontalCenterProperty = DependencyProperty.RegisterReadOnly("HorizontalCenter",
            typeof(double), typeof(ContentPresenterWithHorizontalCenter),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
        public double HorizontalCenter 
        {
            get { return (double)GetValue(HorizontalCenterProperty.DependencyProperty); }
            private set { SetValue(HorizontalCenterProperty, value); }
        }
        protected override Size MeasureOverride(Size constraint)
        {
            Size res = base.MeasureOverride(constraint);
            if (Content is IHorizontalCenter center)
                HorizontalCenter = center.HorizontalCenter;
            return res;
        }

    }
}
