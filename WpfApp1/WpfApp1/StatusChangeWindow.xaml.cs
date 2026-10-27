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
    public partial class StatusChangeWindow : Window
    {
        private user9_dbEntities _db = new user9_dbEntities();
        private tyr_Zayavki _request;

        public StatusChangeWindow(tyr_Zayavki request)
        {
            InitializeComponent();
            _request = _db.tyr_Zayavki.FirstOrDefault(z => z.ID_Zayavki == request.ID_Zayavki);

            if (_request == null)
            {
                MessageBox.Show("Заявка не найдена.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
                Close();
                return;
            }

            tbInfo.Text = $"Заявка №{_request.NomerZayavki}\n" +
                          $"Текущий статус: {_request.tyr_StatusyZayavok.Nazvanie}";

            var statuses = _db.tyr_StatusyZayavok.OrderBy(s => s.Nazvanie).ToList();
            cmbNewStatus.ItemsSource = statuses;
            cmbNewStatus.SelectedValue = _request.ID_Statusa;
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            if (cmbNewStatus.SelectedValue == null)
            {
                MessageBox.Show("Выберите новый статус.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _request.ID_Statusa = (int)cmbNewStatus.SelectedValue;
                _db.SaveChanges();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка смены статуса:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}