using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlocksLib.Blocks.CustomControls
{
    public class TextBoxNotEditable : TextBox
    {

        public static readonly DependencyProperty IsEditProperty = DependencyProperty.Register("IsEdit", typeof(bool), typeof(TextBoxNotEditable),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool IsEdit
        {
            get { return (bool)GetValue(IsEditProperty); }
            set { SetValue(IsEditProperty, value); }
        }

    }
    public static partial class TextBoxNotEditableExtensions
    {
        public static bool? GetMouseDownHandled(UIElement element) => (bool?)element.GetValue(MouseDownHandledProperty);
        public static void SetMouseDownHandled(UIElement element, bool? val) => element.SetValue(MouseDownHandledProperty, val);
        public static DependencyProperty MouseDownHandledProperty = DependencyProperty.RegisterAttached("MouseDownHandled",
            typeof(bool?),
            typeof(TextBoxNotEditableExtensions), new PropertyMetadata(null, OnMouseDown));

        private static void OnMouseDown(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement)d;
            if (e.OldValue is bool val)
                element.RemoveHandler(UIElement.MouseDownEvent, (MouseButtonEventHandler)(val ? OnMouseDownTrueHandler : OnMouseDownFalseHandler));
            if (e.NewValue is bool newVal)
                element.AddHandler(UIElement.MouseDownEvent, (MouseButtonEventHandler)(newVal ? OnMouseDownTrueHandler : OnMouseDownFalseHandler), true);
        }
        private static void OnMouseDownTrueHandler(object sender, MouseButtonEventArgs e) => e.Handled = true;
        private static void OnMouseDownFalseHandler(object sender, MouseButtonEventArgs e)
        {
            TextBoxNotEditable textBox = (TextBoxNotEditable)sender;
            e.Handled = textBox.IsEdit;
        }
    }

}
