using System.Windows;
using System.Windows.Controls;
using BlockConverter.ViewModels;

namespace BlockConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new RootViewModel();
            InitializeComponent();
            if(isFirstLoad)
            {
                ToolTipService.ShowOnDisabledProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(true));
                isFirstLoad = false;
            }
        }
        private static bool isFirstLoad = true;

    }
}
