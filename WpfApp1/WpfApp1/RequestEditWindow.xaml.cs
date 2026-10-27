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
    public partial class RequestEditWindow : Window
    {
        private user9_dbEntities _db;
        private tyr_Zayavki _request;
        private bool _isEditMode;

        public RequestEditWindow()
        {
            InitializeComponent();
            _db = new user9_dbEntities();
            _isEditMode = false;
            tbHeader.Text = "Новая заявка";

            LoadCombos();
            txtNomer.Text = GenerateNumber();
            dpDataPodachi.SelectedDate = DateTime.Today;
        }

        public RequestEditWindow(tyr_Zayavki request)
        {
            InitializeComponent();
            _db = new user9_dbEntities();
            _isEditMode = true;
            tbHeader.Text = "Редактирование заявки";

            LoadCombos();

            _request = _db.tyr_Zayavki.FirstOrDefault(z => z.ID_Zayavki == request.ID_Zayavki);
            if (_request == null)
            {
                MessageBox.Show("Заявка не найдена в базе данных.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
                Close();
                return;
            }

            txtNomer.Text = _request.NomerZayavki;
            cmbKlient.SelectedValue = _request.ID_Klienta;
            cmbTur.SelectedValue = _request.ID_Tura;
            cmbSotrudnik.SelectedValue = _request.ID_Sotrudnika;
            cmbStatus.SelectedValue = _request.ID_Statusa;
            dpDataPodachi.SelectedDate = _request.DataPodachi;
            txtSumma.Text = _request.Summa.ToString("0.##");
        }

        /// <summary>
        /// Автогенерация номера заявки
        /// </summary>
        private string GenerateNumber()
        {
            string prefix = "З-" + DateTime.Now.ToString("yyyyMMdd") + "-";
            var last = _db.tyr_Zayavki
                .Where(z => z.NomerZayavki.StartsWith(prefix))
                .OrderByDescending(z => z.NomerZayavki)
                .FirstOrDefault();

            int next = 1;
            if (last != null)
            {
                string tail = last.NomerZayavki.Substring(prefix.Length);
                if (int.TryParse(tail, out int n))
                    next = n + 1;
            }

            return prefix + next.ToString("D3");
        }

        private void LoadCombos()
        {
            try
            {
                // Клиенты
                var clients = _db.tyr_Klienty
                    .OrderBy(c => c.Familia)
                    .ToList()
                    .Select(c => new
                    {
                        c.ID_Klienta,
                        FullName = $"{c.Familia} {c.Imya} {c.Otchestvo}".Trim()
                    })
                    .ToList();
                cmbKlient.ItemsSource = clients;

                // Туры
                var tours = _db.tyr_Tury
                    .OrderBy(t => t.Nazvanie)
                    .ToList()
                    .Select(t => new
                    {
                        t.ID_Tura,
                        DisplayName = $"{t.Nazvanie} ({t.Strana}, {t.Stoimost:N0} ₽)",
                        t.Stoimost
                    })
                    .ToList();
                cmbTur.ItemsSource = tours;

                // Сотрудники
                var employees = _db.tyr_Sotrudniki
                    .OrderBy(s => s.Familia)
                    .ToList()
                    .Select(s => new
                    {
                        s.ID_Sotrudnika,
                        FullName = $"{s.Familia} {s.Imya}".Trim()
                    })
                    .ToList();
                cmbSotrudnik.ItemsSource = employees;

                // Статусы
                var statuses = _db.tyr_StatusyZayavok
                    .OrderBy(s => s.Nazvanie)
                    .ToList();
                cmbStatus.ItemsSource = statuses;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки справочников:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// При выборе тура — автоматически подставляем его стоимость
        /// </summary>
        private void cmbTur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTur.SelectedItem == null) return;

            dynamic selected = cmbTur.SelectedItem;
            if (!_isEditMode)
            {
                txtSumma.Text = ((decimal)selected.Stoimost).ToString("0.##");
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // ---------- Валидация ----------
            if (string.IsNullOrWhiteSpace(txtNomer.Text))
            {
                MessageBox.Show("Введите номер заявки.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNomer.Focus();
                return;
            }

            if (cmbKlient.SelectedValue == null)
            {
                MessageBox.Show("Выберите клиента.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbTur.SelectedValue == null)
            {
                MessageBox.Show("Выберите тур.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbStatus.SelectedValue == null)
            {
                MessageBox.Show("Выберите статус.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!dpDataPodachi.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату подачи.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtSumma.Text, out decimal summa) || summa < 0)
            {
                MessageBox.Show("Введите корректную сумму (неотрицательное число).",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtSumma.Focus();
                return;
            }

            // Проверка уникальности номера заявки
            string nomer = txtNomer.Text.Trim();
            bool nomerExists = _db.tyr_Zayavki.Any(z =>
                z.NomerZayavki == nomer &&
                (!_isEditMode || z.ID_Zayavki != _request.ID_Zayavki));

            if (nomerExists)
            {
                MessageBox.Show("Заявка с таким номером уже существует.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNomer.Focus();
                return;
            }

            // ---------- Сохранение ----------
            try
            {
                if (!_isEditMode)
                {
                    _request = new tyr_Zayavki();
                    _db.tyr_Zayavki.Add(_request);
                }

                _request.NomerZayavki = nomer;
                _request.ID_Klienta = (int)cmbKlient.SelectedValue;
                _request.ID_Tura = (int)cmbTur.SelectedValue;
                _request.ID_Sotrudnika = cmbSotrudnik.SelectedValue != null
                    ? (int?)cmbSotrudnik.SelectedValue : null;
                _request.ID_Statusa = (int)cmbStatus.SelectedValue;
                _request.DataPodachi = dpDataPodachi.SelectedDate.Value;
                _request.Summa = summa;

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
        public void PreselectTour(int tourId)
        {
            cmbTur.SelectedValue = tourId;
        }
    }
}