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

namespace NMAT
{
    /// <summary>
    /// Interaction logic for help.xaml
    /// </summary>
    public partial class help : Window
    {
        public help()
        {
            InitializeComponent();
            ImportGrid.Visibility = Visibility.Visible;
            SearchGrid.Visibility = Visibility.Hidden;
            VulnGrid.Visibility = Visibility.Hidden;
        }

        private void exit_button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void help_import_Click(object sender, RoutedEventArgs e)
        {
            ImportGrid.Visibility = Visibility.Visible;
            SearchGrid.Visibility = Visibility.Hidden;
            VulnGrid.Visibility = Visibility.Hidden;
        }

        private void help_search_Click(object sender, RoutedEventArgs e)
        {
            SearchGrid.Visibility = Visibility.Visible;
            ImportGrid.Visibility = Visibility.Hidden;
            VulnGrid.Visibility = Visibility.Hidden;
        }

        private void vulnid_button_Click(object sender, RoutedEventArgs e)
        {
            VulnGrid.Visibility = Visibility.Visible;
            ImportGrid.Visibility = Visibility.Hidden;
            SearchGrid.Visibility = Visibility.Hidden;
            
        }
    }
}
