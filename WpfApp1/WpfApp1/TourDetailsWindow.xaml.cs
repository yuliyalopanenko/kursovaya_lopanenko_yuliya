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
    public partial class TourDetailsWindow : Window
    {
        public TourDetailsWindow(tyr_Tury tour)
        {
            InitializeComponent();

            if (tour == null)
            {
                MessageBox.Show("Тур не найден.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            tbTitle.Text = tour.Nazvanie;

            if (!string.IsNullOrWhiteSpace(tour.FotoUrl))
            {
                try
                {
                    imgPhoto.Source = new System.Windows.Media.Imaging.BitmapImage(
                        new Uri(tour.FotoUrl, UriKind.RelativeOrAbsolute));
                }
                catch { }
            }

            tbLocation.Text = $"{tour.Strana}, {tour.Gorod}";
            tbDates.Text = $"{tour.DataNachala:dd.MM.yyyy} — {tour.DataOkonchaniya:dd.MM.yyyy}";
            tbPlaces.Text = tour.KolichestvoMest.ToString();
            tbPrice.Text = tour.Stoimost.ToString("N0") + " ₽";

            tbDescription.Text = !string.IsNullOrWhiteSpace(tour.Opisanie)
                ? tour.Opisanie
                : "Описание для этого тура пока не добавлено.";
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}