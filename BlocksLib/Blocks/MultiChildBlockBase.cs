using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BlocksLib.Blocks
{
    public abstract class MultiChildBlockBase:BlockBase
    {
        #region Constructors
        public MultiChildBlockBase()
        {
        }
        static MultiChildBlockBase()
        {
            BottomPaddingProperty = DependencyProperty.Register("BottomPadding", typeof(double), typeof(MultiChildBlockBase), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));
            ParentBlockProperty = DependencyProperty.RegisterAttached("ParentBlock", typeof(MultiChildBlockBase), typeof(MultiChildBlockBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.NotDataBindable));
        }
        #endregion //Constructors

        #region PublicPropertys
        public static readonly DependencyProperty BottomPaddingProperty;
        public double BottomPadding
        {
            get { return (double)GetValue(BottomPaddingProperty); }
            set { SetValue(BottomPaddingProperty, value); }
        }



        //Attached
        public static readonly DependencyProperty ParentBlockProperty;
        protected static void SetParentBlock(UIElement element, MultiChildBlockBase value) => element.SetValue(ParentBlockProperty, value);
        public static MultiChildBlockBase GetParentBlock(UIElement element) => (MultiChildBlockBase)element.GetValue(ParentBlockProperty);

        #endregion //PublicPropertys

        //добавлять ко всем развилкам!!! В качестве функции реагирующей на изменения дочерних панелей
        protected static void OnSourceUpdate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BlockCycleBase block = (BlockCycleBase)sender;
            BlockPanel panel = (BlockPanel)e.NewValue;
            SetParentBlock(panel, block);
        }
    }
}