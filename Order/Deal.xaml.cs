using Order.Utility;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace Order
{
    /// <summary>
    /// Interaction logic for Deal.xaml
    /// </summary>
    public partial class Deal : UserControl
    {
        protected ObservableCollection<Utility.Order> Orders { get; private set; }

        public Deal()
        {
            InitializeComponent();
            dateFilter.IsChecked = false;
            EnableDateFilter(false);
            Reload();
        }

        private void Reload()
        {
            using (var db = new Database())
            {
                var entries = db.QueryDeals(
                    nameBox.Text,
                    dateFilter.IsChecked.Value ? startDate.SelectedDate : null,
                    dateFilter.IsChecked.Value ? endDate.SelectedDate : null);
                Orders = new ObservableCollection<Utility.Order>(entries);
                list.ItemsSource = Orders;
                var totalCost = Orders.Sum(x => x.Cost);
                var totalEarn = Orders.Sum(x => x.TotalPrice);
                var totalPending = Orders.Where(x => x.CashName == "挂帐").Sum(x => x.TotalPrice);
                var totalCash = Orders.Where(x => x.CashName == "现金").Sum(x => x.TotalPrice);
                var totalProfit = Orders.Sum(x => x.Profit);
                summaryBox.Text = $"收入: {totalEarn}({totalCash}+{totalPending}) " +
                    $"成本: {totalCost} " +
                    $"盈利: {totalProfit} ";
            }
        }

        private void EnableDateFilter(bool enabled)
        {
            startDate.IsEnabled = enabled;
            endDate.IsEnabled = enabled;
        }

        private void dateFilter_Click(object sender, RoutedEventArgs e)
        {
            EnableDateFilter(dateFilter.IsChecked.Value);
        }

        private void MonthlyButtonClicked(object sender, RoutedEventArgs e)
        {
            endDate.SelectedDate = DateTime.Now;
            startDate.SelectedDate = DateTime.Now.AddMonths(-1);
        }

        private void YearlyButtonClicked(object sender, RoutedEventArgs e)
        {
            endDate.SelectedDate = DateTime.Now;
            startDate.SelectedDate = DateTime.Now.AddYears(-1);
        }

        private void QueryButtonClicked(object sender, RoutedEventArgs e)
        {
            Reload();
        }

        private void NewEntryButtonClicked(object sender, RoutedEventArgs e)
        {
            var diag = new DealEntry();
            if(diag.ShowDialog() == true)
            {
                var order = diag.Order;
                using (var db = new Database())
                {
                    order.Id = db.AddDeal(order.Date, order.ClientId, order.ProductId,
                        order.UnitId, order.Number, order.Cost, order.UnitPrice, order.CashId, order.Comment);
                }
                Orders.Add(order);
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void RemoveEntry(object sender, RoutedEventArgs e)
        {
            var entry = (sender as Button).DataContext as Utility.Order;
            using (var db = new Database())
            {
                db.DeleteDeal(entry.Id);
            }
            Orders.Remove(entry);
        }

        private void EditOrder(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var template = (sender as ListViewItem).Content as Utility.Order;
            var diag = new DealEntry();
            diag.UseTemplate(template);
            if (diag.ShowDialog() == true)
            {
                var order = diag.Order;
                using (var db = new Database())
                {
                    db.UpdateDeal(template.Id, order.Date, order.ClientId, order.ProductId,
                        order.UnitId, order.Number, order.Cost, order.UnitPrice, order.CashId, order.Comment);
                }
                order.Id = template.Id;
                var index = Orders.IndexOf(template);
                Orders[index] = order;
            }
        }
    }
}
