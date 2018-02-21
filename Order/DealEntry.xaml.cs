using Order.Utility;
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

namespace Order
{
    /// <summary>
    /// Interaction logic for DealEntry.xaml
    /// </summary>
    public partial class DealEntry : Window
    {
        public Utility.Order Order { set; get; }
        public DealEntry()
        {
            InitializeComponent();
            InitializeOptions();
        }

        private void InitializeOptions()
        {
            dateBox.SelectedDate = DateTime.Now;
            using (var db = new Database())
            {
                clientBox.ItemsSource = db.QueryClients();
                productBox.ItemsSource = db.QueryDependentEntries("PRODUCT");
                unitBox.ItemsSource = db.QueryDependentEntries("UNIT");
                cashTypeBox.ItemsSource = db.QueryDependentEntries("CASHTYPE");
            }
        }

        private bool ValidateOptions()
        {
            return dateBox.SelectedDate.HasValue
                && clientBox.SelectedIndex != -1
                && productBox.SelectedIndex != -1
                && unitBox.SelectedIndex != -1
                && cashTypeBox.SelectedIndex != -1
                && long.TryParse(numberBox.Text, out long _0)
                && double.TryParse(costBox.Text, out double _1)
                && double.TryParse(unitPriceBox.Text, out double _2);
        }

        private void EndDialog(object sender, RoutedEventArgs e)
        {
            if(ValidateOptions())
            {
                DialogResult = true;
                Order = new Utility.Order
                {
                    ClientId = (clientBox.SelectedItem as Client).Id,
                    ClientName = (clientBox.SelectedItem as Client).Name,
                    ProductId = (productBox.SelectedItem as Entry).Id,
                    ProductName = (productBox.SelectedItem as Entry).Name,
                    UnitId = (unitBox.SelectedItem as Entry).Id,
                    UnitName = (unitBox.SelectedItem as Entry).Name,
                    CashId = (cashTypeBox.SelectedItem as Entry).Id,
                    CashName = (cashTypeBox.SelectedItem as Entry).Name,
                    Number = long.Parse(numberBox.Text),
                    Cost = double.Parse(costBox.Text),
                    UnitPrice = double.Parse(unitPriceBox.Text),
                    Date = dateBox.SelectedDate.Value,
                    Comment = commentBox.Text
                };
            }
            else
            {
                MessageBox.Show("输入不合法");
            }
        }

        internal void UseTemplate(Utility.Order template)
        {
            clientBox.SelectedItem = (clientBox.ItemsSource as IEnumerable<Client>).First(x => x.Id == template.ClientId);
            productBox.SelectedItem = (productBox.ItemsSource as IEnumerable<Entry>).First(x => x.Id == template.ProductId);
            unitBox.SelectedItem = (unitBox.ItemsSource as IEnumerable<Entry>).First(x => x.Id == template.UnitId);
            cashTypeBox.SelectedItem = (cashTypeBox.ItemsSource as IEnumerable<Entry>).First(x => x.Id == template.CashId);
            numberBox.Text = template.Number.ToString();
            costBox.Text = template.Cost.ToString();
            unitPriceBox.Text = template.UnitPrice.ToString();
            dateBox.SelectedDate = template.Date;
            commentBox.Text = template.Comment;
        }
    }
}
