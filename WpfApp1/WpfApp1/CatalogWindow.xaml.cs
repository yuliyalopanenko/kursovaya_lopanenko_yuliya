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
    public partial class CatalogWindow : Window
    {
        private user9_dbEntities _db;

        public CatalogWindow()
        {
            InitializeComponent();
            LoadCountries();
            LoadTours();
        }

        private void LoadCountries()
        {
            try
            {
                _db?.Dispose();
                _db = new user9_dbEntities();

                var countries = _db.tyr_Tury
                    .Select(t => t.Strana)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();

                var list = countries.Cast<object>().ToList();
                list.Insert(0, "(все страны)");

                cmbCountry.ItemsSource = list;
                cmbCountry.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки стран:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

                icTours.ItemsSource = tours;
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

                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    string s = txtSearch.Text.Trim().ToLower();
                    query = query.Where(t =>
                        t.Nazvanie.ToLower().Contains(s) ||
                        t.Strana.ToLower().Contains(s) ||
                        t.Gorod.ToLower().Contains(s));
                }

                if (cmbCountry.SelectedItem != null &&
                    cmbCountry.SelectedItem.ToString() != "(все страны)")
                {
                    string country = cmbCountry.SelectedItem.ToString();
                    query = query.Where(t => t.Strana == country);
                }

                var result = query.OrderBy(t => t.DataNachala).ToList();
                icTours.ItemsSource = result;

                if (result.Count == 0)
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
            txtSearch.Clear();
            cmbCountry.SelectedIndex = 0;
            LoadTours();
        }

        private void TourCard_Click(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border == null) return;

            var tour = border.Tag as tyr_Tury;
            if (tour == null) return;

            var detailsWindow = new TourDetailsWindow(tour);
            detailsWindow.Owner = this;
            detailsWindow.ShowDialog();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Owner = this;

            if (loginWindow.ShowDialog() == true)
            {
                var main = new MainWindow();
                main.Show();
                this.Hide();
            }
        }
    }
}