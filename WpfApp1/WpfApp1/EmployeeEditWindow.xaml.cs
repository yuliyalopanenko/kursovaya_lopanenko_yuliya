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
    public partial class EmployeeEditWindow : Window
    {
        private user9_dbEntities _db;
        private tyr_Sotrudniki _employee;
        private bool _isEditMode;

        public EmployeeEditWindow()
        {
            InitializeComponent();
            _db = new user9_dbEntities();
            _isEditMode = false;
            tbHeader.Text = "Новый сотрудник";
            LoadDolzhnosti();
        }

        public EmployeeEditWindow(tyr_Sotrudniki employee)
        {
            InitializeComponent();
            _db = new user9_dbEntities();
            _isEditMode = true;
            tbHeader.Text = "Редактирование сотрудника";
            LoadDolzhnosti();

            _employee = _db.tyr_Sotrudniki.FirstOrDefault(s => s.ID_Sotrudnika == employee.ID_Sotrudnika);
            if (_employee == null)
            {
                MessageBox.Show("Сотрудник не найден в базе данных.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
                Close();
                return;
            }

            txtFamilia.Text = _employee.Familia;
            txtImya.Text = _employee.Imya;
            txtOtchestvo.Text = _employee.Otchestvo;
            txtTelefon.Text = _employee.Telefon;
            cmbDolzhnost.SelectedValue = _employee.ID_Dolzhnosti;
        }

        /// <summary>
        /// Загрузка справочника должностей в ComboBox
        /// </summary>
        private void LoadDolzhnosti()
        {
            try
            {
                var dolzhnosti = _db.tyr_Dolzhnosti
                    .OrderBy(d => d.Nazvanie)
                    .ToList();

                cmbDolzhnost.ItemsSource = dolzhnosti;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки должностей:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // ---------- Валидация ----------
            if (string.IsNullOrWhiteSpace(txtFamilia.Text))
            {
                MessageBox.Show("Введите фамилию.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFamilia.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtImya.Text))
            {
                MessageBox.Show("Введите имя.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtImya.Focus();
                return;
            }

            if (cmbDolzhnost.SelectedValue == null)
            {
                MessageBox.Show("Выберите должность.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbDolzhnost.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTelefon.Text))
            {
                MessageBox.Show("Введите телефон.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTelefon.Focus();
                return;
            }

            // ---------- Сохранение ----------
            try
            {
                if (!_isEditMode)
                {
                    _employee = new tyr_Sotrudniki();
                    _db.tyr_Sotrudniki.Add(_employee);
                }

                _employee.Familia = txtFamilia.Text.Trim();
                _employee.Imya = txtImya.Text.Trim();
                _employee.Otchestvo = string.IsNullOrWhiteSpace(txtOtchestvo.Text)
                    ? null : txtOtchestvo.Text.Trim();
                _employee.ID_Dolzhnosti = (int)cmbDolzhnost.SelectedValue;
                _employee.Telefon = txtTelefon.Text.Trim();

                _db.SaveChanges();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения:\n" + ex.Message,
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