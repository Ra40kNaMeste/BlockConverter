using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BlocksLib.Blocks
{
    public enum BlockCycleTypes
    {
        BlockCycle = 8, BlockPostCycle = 9, BlockForCycle = 10
    }
    public class BlockCycle:BlockCycleBase, IBlockMultiType
    {
        static BlockCycle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlockCycle), new FrameworkPropertyMetadata(typeof(BlockCycle)));
            typeNameProperty = DependencyProperty.Register("TypeName", typeof(BlockCycleTypes), typeof(BlockCycle));

        }
        public static readonly DependencyProperty typeNameProperty;
        public BlockCycleTypes TypeName
        {
            get { return (BlockCycleTypes)GetValue(typeNameProperty); }
            set { SetValue(typeNameProperty, value); }
        }

        public string GetBlockType()
        {
            return TypeName.ToString();
        }
        public override object Clone()
        {
            BlockCycle res = (BlockCycle)base.Clone();
            res.TypeName = TypeName;
            return res;
        }
    }
}
