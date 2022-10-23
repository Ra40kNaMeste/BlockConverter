using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlocksLib.Blocks
{
    public interface IBlockMultiType
    {
        public string GetBlockType();
    }
    public enum BaseBlockTypes
    {
        Block, BeginBlock, EndBlock, InputBlock, OutputBlock
    }
    /// <summary>
    /// Логика взаимодействия для Block.xaml
    /// </summary>
    public partial class Block : BlockBase, IBlockMultiType
    {
        static Block()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Block), new FrameworkPropertyMetadata(typeof(Block)));
            typeNameProperty = DependencyProperty.Register("TypeName", typeof(BaseBlockTypes), typeof(Block));
        }
        public static readonly DependencyProperty typeNameProperty;
        public BaseBlockTypes TypeName
        {
            get { return (BaseBlockTypes)GetValue(typeNameProperty); }
            set { SetValue(typeNameProperty, value); }
        }

        public string GetBlockType()
        {
            return TypeName.ToString();
        }
        public override object Clone()
        {
            return new Block() { Text = Text, TypeName = TypeName };
        }
    }
}
