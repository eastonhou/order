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

namespace Order
{
    /// <summary>
    /// Interaction logic for Management.xaml
    /// </summary>
    public partial class Management : UserControl
    {
        public Management()
        {
            InitializeComponent();
        }

        private void UnitButtonClicked(object sender, RoutedEventArgs e)
        {
            var diag = new DependentEntry();
            diag.Initialize("UNIT", "单位", "单位");
            diag.ShowDialog();
        }

        private void ProductButtonClicked(object sender, RoutedEventArgs e)
        {
            var diag = new DependentEntry();
            diag.Initialize("PRODUCT", "类别", "类别");
            diag.ShowDialog();
        }

        private void ClientButtonClicked(object sender, RoutedEventArgs e)
        {
            var diag = new ClientEntry();
            diag.ShowDialog();
        }
    }
}
