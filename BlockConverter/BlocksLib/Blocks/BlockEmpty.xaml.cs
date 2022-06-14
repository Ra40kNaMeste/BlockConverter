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
    /// <summary>
    /// Логика взаимодействия для BlockEmpty.xaml
    /// </summary>
    public partial class BlockEmpty : BlockBase
    {
        static BlockEmpty()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlockEmpty), new FrameworkPropertyMetadata(typeof(BlockEmpty)));
        }
        public BlockEmpty()
        {

        }
    }
}
