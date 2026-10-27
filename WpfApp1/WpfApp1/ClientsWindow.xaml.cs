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
namespace WpfApp1
{
    public partial class ClientsWindow : Window
    {
        private user9_dbEntities _db;

        public ClientsWindow()
        {
            InitializeComponent();
            LoadClients();
        }

        private void LoadClients()
        {
            try
            {
                _db?.Dispose();
                _db = new user9_dbEntities();

                var clients = _db.tyr_Klienty
                    .OrderBy(c => c.Familia)
                    .ThenBy(c => c.Imya)
                    .ToList();
                dgClients.ItemsSource = clients;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки клиентов:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _db?.Dispose();
                _db = new user9_dbEntities();

                var query = _db.tyr_Klienty.AsQueryable();

                if (!string.IsNullOrWhiteSpace(txtSearchFio.Text))
                {
                    string fio = txtSearchFio.Text.Trim().ToLower();
                    query = query.Where(c =>
                        c.Familia.ToLower().Contains(fio) ||
                        c.Imya.ToLower().Contains(fio) ||
                        (c.Otchestvo != null && c.Otchestvo.ToLower().Contains(fio)));
                }

                if (!string.IsNullOrWhiteSpace(txtSearchPhone.Text))
                {
                    string phone = txtSearchPhone.Text.Trim();
                    query = query.Where(c => c.Telefon.Contains(phone));
                }

                dgClients.ItemsSource = query
                    .OrderBy(c => c.Familia)
                    .ThenBy(c => c.Imya)
                    .ToList();

                if (dgClients.Items.Count == 0)
                {
                    MessageBox.Show("Клиенты по заданным параметрам не найдены.",
                        "Результат поиска", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка поиска:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txtSearchFio.Clear();
            txtSearchPhone.Clear();
            LoadClients();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadClients();
        }

        private tyr_Klienty GetSelectedClient()
        {
            return dgClients.SelectedItem as tyr_Klienty;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new ClientEditWindow();
            editWindow.Owner = this;

            if (editWindow.ShowDialog() == true)
                LoadClients();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var client = GetSelectedClient();
            if (client == null)
            {
                MessageBox.Show("Выберите клиента для редактирования.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var editWindow = new ClientEditWindow(client);
            editWindow.Owner = this;

            if (editWindow.ShowDialog() == true)
                LoadClients();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var client = GetSelectedClient();
            if (client == null)
            {
                MessageBox.Show("Выберите клиента для удаления.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Удалить клиента «{client.Familia} {client.Imya}»?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _db?.Dispose();
                    _db = new user9_dbEntities();

                    var toDelete = _db.tyr_Klienty
                        .FirstOrDefault(c => c.ID_Klienta == client.ID_Klienta);

                    if (toDelete != null)
                    {
                        _db.tyr_Klienty.Remove(toDelete);
                        _db.SaveChanges();
                    }

                    LoadClients();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не удалось удалить клиента. Возможно, у него есть заявки.\n" + ex.Message,
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}