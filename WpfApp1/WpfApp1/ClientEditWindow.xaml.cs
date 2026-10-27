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
using System.Text.RegularExpressions;

namespace WpfApp1
{
    public partial class ClientEditWindow : Window
    {
        private user9_dbEntities _db;
        private tyr_Klienty _client;
        private bool _isEditMode;

        public ClientEditWindow()
        {
            InitializeComponent();
            _db = new user9_dbEntities();
            _isEditMode = false;
            tbHeader.Text = "Новый клиент";
        }

        public ClientEditWindow(tyr_Klienty client)
        {
            InitializeComponent();
            _db = new user9_dbEntities();
            _isEditMode = true;
            tbHeader.Text = "Редактирование клиента";

            _client = _db.tyr_Klienty.FirstOrDefault(c => c.ID_Klienta == client.ID_Klienta);
            if (_client == null)
            {
                MessageBox.Show("Клиент не найден в базе данных.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
                Close();
                return;
            }

            txtFamilia.Text = _client.Familia;
            txtImya.Text = _client.Imya;
            txtOtchestvo.Text = _client.Otchestvo;
            txtPasport.Text = _client.PasportnyeDannye;
            txtTelefon.Text = _client.Telefon;
            txtEmail.Text = _client.Email;
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

            if (string.IsNullOrWhiteSpace(txtPasport.Text))
            {
                MessageBox.Show("Введите паспортные данные.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPasport.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTelefon.Text))
            {
                MessageBox.Show("Введите телефон.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTelefon.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Введите email.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return;
            }

            // Проверка формата email
            if (!Regex.IsMatch(txtEmail.Text.Trim(),
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Введите корректный email (пример: name@mail.ru).",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return;
            }

            // Проверка уникальности паспорта
            string pasport = txtPasport.Text.Trim();
            bool pasportExists = _db.tyr_Klienty.Any(c =>
                c.PasportnyeDannye == pasport &&
                (!_isEditMode || c.ID_Klienta != _client.ID_Klienta));

            if (pasportExists)
            {
                MessageBox.Show("Клиент с такими паспортными данными уже существует.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPasport.Focus();
                return;
            }

            // ---------- Сохранение ----------
            try
            {
                if (!_isEditMode)
                {
                    _client = new tyr_Klienty();
                    _db.tyr_Klienty.Add(_client);
                }

                _client.Familia = txtFamilia.Text.Trim();
                _client.Imya = txtImya.Text.Trim();
                _client.Otchestvo = string.IsNullOrWhiteSpace(txtOtchestvo.Text)
                    ? null : txtOtchestvo.Text.Trim();
                _client.PasportnyeDannye = pasport;
                _client.Telefon = txtTelefon.Text.Trim();
                _client.Email = txtEmail.Text.Trim();

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