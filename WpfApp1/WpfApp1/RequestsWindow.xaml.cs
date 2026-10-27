using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    public partial class RequestsWindow : Window
    {
        private user9_dbEntities _db;

        public RequestsWindow()
        {
            InitializeComponent();
            LoadStatusesCombo();
            LoadRequests();
        }

        private void LoadStatusesCombo()
        {
            try
            {
                _db?.Dispose();
                _db = new user9_dbEntities();

                var statuses = _db.tyr_StatusyZayavok
                    .OrderBy(s => s.Nazvanie)
                    .ToList();

                var list = statuses.Cast<object>().ToList();
                list.Insert(0, new { ID_Statusa = 0, Nazvanie = "(все)" });

                cmbSearchStatus.ItemsSource = list;
                cmbSearchStatus.DisplayMemberPath = "Nazvanie";
                cmbSearchStatus.SelectedValuePath = "ID_Statusa";
                cmbSearchStatus.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки статусов:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRequests()
        {
            try
            {
                _db?.Dispose();
                _db = new user9_dbEntities();

                var requests = _db.tyr_Zayavki
                    .Include("tyr_Klienty")
                    .Include("tyr_Tury")
                    .Include("tyr_Sotrudniki")
                    .Include("tyr_StatusyZayavok")
                    .OrderByDescending(z => z.DataPodachi)
                    .ToList();
                dgRequests.ItemsSource = requests;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки заявок:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _db?.Dispose();
                _db = new user9_dbEntities();

                var query = _db.tyr_Zayavki.AsQueryable();

                if (!string.IsNullOrWhiteSpace(txtSearchNumber.Text))
                {
                    string num = txtSearchNumber.Text.Trim().ToLower();
                    query = query.Where(z => z.NomerZayavki.ToLower().Contains(num));
                }

                if (!string.IsNullOrWhiteSpace(txtSearchClient.Text))
                {
                    string fio = txtSearchClient.Text.Trim().ToLower();
                    query = query.Where(z =>
                        z.tyr_Klienty.Familia.ToLower().Contains(fio) ||
                        z.tyr_Klienty.Imya.ToLower().Contains(fio));
                }

                if (cmbSearchStatus.SelectedValue != null &&
                    (int)cmbSearchStatus.SelectedValue != 0)
                {
                    int idStatus = (int)cmbSearchStatus.SelectedValue;
                    query = query.Where(z => z.ID_Statusa == idStatus);
                }

                var list = query
                    .Include("tyr_Klienty")
                    .Include("tyr_Tury")
                    .Include("tyr_Sotrudniki")
                    .Include("tyr_StatusyZayavok")
                    .OrderByDescending(z => z.DataPodachi)
                    .ToList();

                dgRequests.ItemsSource = list;

                if (list.Count == 0)
                {
                    MessageBox.Show("Заявки по заданным параметрам не найдены.",
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
            txtSearchNumber.Clear();
            txtSearchClient.Clear();
            cmbSearchStatus.SelectedIndex = 0;
            LoadRequests();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadRequests();
        }

        private tyr_Zayavki GetSelectedRequest()
        {
            return dgRequests.SelectedItem as tyr_Zayavki;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new RequestEditWindow();
            editWindow.Owner = this;
            if (editWindow.ShowDialog() == true)
                LoadRequests();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var req = GetSelectedRequest();
            if (req == null)
            {
                MessageBox.Show("Выберите заявку для редактирования.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var editWindow = new RequestEditWindow(req);
            editWindow.Owner = this;
            if (editWindow.ShowDialog() == true)
                LoadRequests();
        }

        private void btnChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            var req = GetSelectedRequest();
            if (req == null)
            {
                MessageBox.Show("Выберите заявку для смены статуса.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var statusWindow = new StatusChangeWindow(req);
            statusWindow.Owner = this;
            if (statusWindow.ShowDialog() == true)
                LoadRequests();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var req = GetSelectedRequest();
            if (req == null)
            {
                MessageBox.Show("Выберите заявку для удаления.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Удалить заявку №{req.NomerZayavki}?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _db?.Dispose();
                    _db = new user9_dbEntities();

                    var toDelete = _db.tyr_Zayavki
                        .FirstOrDefault(z => z.ID_Zayavki == req.ID_Zayavki);

                    if (toDelete != null)
                    {
                        _db.tyr_Zayavki.Remove(toDelete);
                        _db.SaveChanges();
                    }

                    LoadRequests();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка удаления:\n" + ex.Message,
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}