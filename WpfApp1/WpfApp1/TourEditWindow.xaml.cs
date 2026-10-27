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

using System;
using System.Linq;
using System.Windows;

namespace WpfApp1
{
    public partial class TourEditWindow : Window
    {
        private user9_dbEntities _db;
        private tyr_Tury _tour;   // null = добавление, иначе = редактирование
        private bool _isEditMode;

        /// <summary>
        /// Конструктор для добавления нового тура
        /// </summary>
        public TourEditWindow()
        {
            InitializeComponent();
            _db = new user9_dbEntities();
            _isEditMode = false;
            tbHeader.Text = "Новый тур";
        }

        /// <summary>
        /// Конструктор для редактирования существующего тура
        /// </summary>
        public TourEditWindow(tyr_Tury tour)
        {
            InitializeComponent();
            _db = new user9_dbEntities();
            _isEditMode = true;
            tbHeader.Text = "Редактирование тура";

            // Загружаем актуальную версию тура из БД
            _tour = _db.tyr_Tury.FirstOrDefault(t => t.ID_Tura == tour.ID_Tura);
            if (_tour == null)
            {
                MessageBox.Show("Тур не найден в базе данных.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
                Close();
                return;
            }

            // Заполняем поля
            txtNazvanie.Text = _tour.Nazvanie;
            txtStrana.Text = _tour.Strana;
            txtGorod.Text = _tour.Gorod;
            dpNachala.SelectedDate = _tour.DataNachala;
            dpOkonchaniya.SelectedDate = _tour.DataOkonchaniya;
            txtStoimost.Text = _tour.Stoimost.ToString("0.##");
            txtMest.Text = _tour.KolichestvoMest.ToString();
        }

        /// <summary>
        /// Сохранение (добавление или обновление)
        /// </summary>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // ---------- Валидация ----------
            if (string.IsNullOrWhiteSpace(txtNazvanie.Text))
            {
                MessageBox.Show("Введите название тура.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNazvanie.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtStrana.Text))
            {
                MessageBox.Show("Введите страну.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtStrana.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtGorod.Text))
            {
                MessageBox.Show("Введите город.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtGorod.Focus();
                return;
            }

            if (!dpNachala.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату начала.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!dpOkonchaniya.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату окончания.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpOkonchaniya.SelectedDate.Value < dpNachala.SelectedDate.Value)
            {
                MessageBox.Show("Дата окончания не может быть раньше даты начала.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtStoimost.Text, out decimal stoimost) || stoimost < 0)
            {
                MessageBox.Show("Введите корректную стоимость (неотрицательное число).",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtStoimost.Focus();
                return;
            }

            if (!int.TryParse(txtMest.Text, out int mest) || mest < 0)
            {
                MessageBox.Show("Введите корректное количество мест (целое неотрицательное число).",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtMest.Focus();
                return;
            }

            // ---------- Сохранение ----------
            try
            {
                if (!_isEditMode)
                {
                    // Добавление
                    _tour = new tyr_Tury();
                    _db.tyr_Tury.Add(_tour);
                }

                _tour.Nazvanie = txtNazvanie.Text.Trim();
                _tour.Strana = txtStrana.Text.Trim();
                _tour.Gorod = txtGorod.Text.Trim();
                _tour.DataNachala = dpNachala.SelectedDate.Value;
                _tour.DataOkonchaniya = dpOkonchaniya.SelectedDate.Value;
                _tour.Stoimost = stoimost;
                _tour.KolichestvoMest = mest;

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