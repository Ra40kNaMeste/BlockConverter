using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BlocksLib.Blocks
{
    public class BlockMethod:BlockBase
    {
        static BlockMethod()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlockMethod), new FrameworkPropertyMetadata(typeof(BlockMethod)));
            InputProperty = DependencyProperty.Register("Input", typeof(string), typeof(BlockMethod), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsMeasure));
            OutputProperty = DependencyProperty.Register("Output", typeof(string), typeof(BlockMethod), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsMeasure));
        }
        public static readonly DependencyProperty InputProperty;
        public string Input
        {
            get { return (string)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }
        public static readonly DependencyProperty OutputProperty;
        public string Output
        {
            get { return (string)GetValue(OutputProperty); }
            set { SetValue(OutputProperty, value); }
        }
    }
}
