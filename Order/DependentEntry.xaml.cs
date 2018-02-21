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
    /// Interaction logic for DependentEntry.xaml
    /// </summary>
    public partial class DependentEntry : Window
    {
        public string Table { set; get; }

        public DependentEntry()
        {
            InitializeComponent();
        }

        public void Initialize(string table, string title, string label)
        {
            Table = table;
            Title = title;
            nameLabel.Content = label;
            RefreshList();
        }

        private IEnumerable<Entry> QueryEntries()
        {
            using (var db = new Database())
            {
                var entries = db.QueryDependentEntries(Table);
                return entries;
            }
        }

        private void AddEntry(object sender, RoutedEventArgs e)
        {
            using (var db = new Database())
            {
                db.AddDependentEntry(Table, nameBox.Text, comment.Text);
                RefreshList();
            }
        }

        private void RefreshList()
        {
            var entries = QueryEntries();
            list.ItemsSource = entries;
        }

        private void UpdateEntry(object sender, RoutedEventArgs e)
        {
            var selected = list.SelectedItem as Entry;
            if(null == selected)
            {
                MessageBox.Show("未选中条目！", "未选中条目", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            using (var db = new Database())
            {
                db.UpdateDependentEntry(Table, selected.Id, nameBox.Text, comment.Text);
                RefreshList();
            }
        }

        private void RemoveEntry(object sender, RoutedEventArgs e)
        {
            var entry = (sender as Button).DataContext as Entry;
            using (var db = new Database())
            {
                db.DeleteDependentEntry(Table, entry.Id);
                RefreshList();
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list.SelectedItem is Entry selected)
            {
                nameBox.Text = selected.Name;
                comment.Text = selected.Comment;
            }
        }
    }
}
