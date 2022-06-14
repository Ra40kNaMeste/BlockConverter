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
using System.Windows.Shapes;

namespace BlockConverter.ViewModels.SettingViewModel
{
    /// <summary>
    /// Логика взаимодействия для LanguageAcceptShowDialog.xaml
    /// </summary>
    public partial class LanguageAcceptShowDialog : Window
    {
        public LanguageAcceptShowDialog()
        {
            InitializeComponent();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

    }
}
