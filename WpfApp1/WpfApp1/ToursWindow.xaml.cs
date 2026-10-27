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
    public partial class ToursWindow : Window
    {
        private user9_dbEntities _db;

        public ToursWindow()
        {
            InitializeComponent();
            LoadTours();
        }

        private void LoadTours()
        {
            try
            {
                _db?.Dispose();
                _db = new user9_dbEntities();

                var tours = _db.tyr_Tury
                    .OrderBy(t => t.DataNachala)
                    .ToList();
                dgTours.ItemsSource = tours;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки туров:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _db?.Dispose();
                _db = new user9_dbEntities();

                var query = _db.tyr_Tury.AsQueryable();

                if (!string.IsNullOrWhiteSpace(txtStrana.Text))
                {
                    string country = txtStrana.Text.Trim().ToLower();
                    query = query.Where(t => t.Strana.ToLower().Contains(country));
                }

                if (dpFrom.SelectedDate.HasValue)
                {
                    var from = dpFrom.SelectedDate.Value;
                    query = query.Where(t => t.DataNachala >= from);
                }

                if (dpTo.SelectedDate.HasValue)
                {
                    var to = dpTo.SelectedDate.Value;
                    query = query.Where(t => t.DataOkonchaniya <= to);
                }

                if (!string.IsNullOrWhiteSpace(txtMaxPrice.Text))
                {
                    if (decimal.TryParse(txtMaxPrice.Text, out decimal maxPrice))
                    {
                        query = query.Where(t => t.Stoimost <= maxPrice);
                    }
                    else
                    {
                        MessageBox.Show("Введите корректное число в поле «Цена до».",
                            "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                dgTours.ItemsSource = query.OrderBy(t => t.DataNachala).ToList();

                if (dgTours.Items.Count == 0)
                {
                    MessageBox.Show("Туры по заданным параметрам не найдены.",
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
            txtStrana.Clear();
            dpFrom.SelectedDate = null;
            dpTo.SelectedDate = null;
            txtMaxPrice.Clear();
            LoadTours();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadTours();
        }

        private tyr_Tury GetSelectedTour()
        {
            return dgTours.SelectedItem as tyr_Tury;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new TourEditWindow();
            editWindow.Owner = this;

            if (editWindow.ShowDialog() == true)
                LoadTours();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var tour = GetSelectedTour();
            if (tour == null)
            {
                MessageBox.Show("Выберите тур для редактирования.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var editWindow = new TourEditWindow(tour);
            editWindow.Owner = this;

            if (editWindow.ShowDialog() == true)
                LoadTours();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var tour = GetSelectedTour();
            if (tour == null)
            {
                MessageBox.Show("Выберите тур для удаления.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Удалить тур «{tour.Nazvanie}»?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _db?.Dispose();
                    _db = new user9_dbEntities();

                    var toDelete = _db.tyr_Tury
                        .FirstOrDefault(t => t.ID_Tura == tour.ID_Tura);

                    if (toDelete != null)
                    {
                        _db.tyr_Tury.Remove(toDelete);
                        _db.SaveChanges();
                    }

                    LoadTours();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не удалось удалить тур. Возможно, на него есть заявки.\n" + ex.Message,
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnCreateRequest_Click(object sender, RoutedEventArgs e)
        {
            var tour = GetSelectedTour();
            if (tour == null)
            {
                MessageBox.Show("Выберите тур для оформления заявки.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var requestWindow = new RequestEditWindow();
            requestWindow.Owner = this;

            if (requestWindow.ShowDialog() == true)
            {
                MessageBox.Show("Заявка успешно оформлена!",
                    "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}