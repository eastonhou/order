using Order.Utility;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Order
{
    /// <summary>
    /// Interaction logic for ClientEntry.xaml
    /// </summary>
    public partial class ClientEntry : Window
    {
        public ClientEntry()
        {
            InitializeComponent();
            RefreshList();
        }

        private IEnumerable<Client> QueryEntries()
        {
            using (var db = new Database())
            {
                var entries = db.QueryClients();
                return entries;
            }
        }

        private void AddEntry(object sender, RoutedEventArgs e)
        {
            using (var db = new Database())
            {
                db.AddClient(nameBox.Text, phoneBox.Text, comment.Text);
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
            var selected = list.SelectedItem as Client;
            if (null == selected)
            {
                MessageBox.Show("未选中条目！", "未选中条目", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            using (var db = new Database())
            {
                db.UpdateClient(selected.Id, nameBox.Text, phoneBox.Text, comment.Text);
                RefreshList();
            }
        }

        private void RemoveEntry(object sender, RoutedEventArgs e)
        {
            var entry = (sender as Button).DataContext as Client;
            using (var db = new Database())
            {
                db.DeleteClient(entry.Id);
                RefreshList();
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list.SelectedItem is Client selected)
            {
                nameBox.Text = selected.Name;
                phoneBox.Text = selected.Phone;
                comment.Text = selected.Comment;
            }
        }
    }
}

