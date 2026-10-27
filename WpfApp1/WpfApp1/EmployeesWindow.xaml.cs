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
using System.Data.Entity;

namespace WpfApp1
{
    public partial class EmployeesWindow : Window
    {
        private user9_dbEntities _db;

        public EmployeesWindow()
        {
            InitializeComponent();
            LoadDolzhnostiCombo();
            LoadEmployees();
        }

        private void LoadDolzhnostiCombo()
        {
            try
            {
                _db?.Dispose();
                _db = new user9_dbEntities();

                var dolzhnosti = _db.tyr_Dolzhnosti
                    .OrderBy(d => d.Nazvanie)
                    .ToList();

                var list = dolzhnosti.Cast<object>().ToList();
                list.Insert(0, new { ID_Dolzhnosti = 0, Nazvanie = "(все)" });

                cmbSearchDolzhnost.ItemsSource = list;
                cmbSearchDolzhnost.DisplayMemberPath = "Nazvanie";
                cmbSearchDolzhnost.SelectedValuePath = "ID_Dolzhnosti";
                cmbSearchDolzhnost.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки должностей:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadEmployees()
        {
            try
            {
                _db?.Dispose();
                _db = new user9_dbEntities();

                var employees = _db.tyr_Sotrudniki
                    .Include("tyr_Dolzhnosti")
                    .OrderBy(s => s.Familia)
                    .ThenBy(s => s.Imya)
                    .ToList();
                dgEmployees.ItemsSource = employees;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки сотрудников:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _db?.Dispose();
                _db = new user9_dbEntities();

                var query = _db.tyr_Sotrudniki.AsQueryable();

                if (!string.IsNullOrWhiteSpace(txtSearchFio.Text))
                {
                    string fio = txtSearchFio.Text.Trim().ToLower();
                    query = query.Where(s =>
                        s.Familia.ToLower().Contains(fio) ||
                        s.Imya.ToLower().Contains(fio) ||
                        (s.Otchestvo != null && s.Otchestvo.ToLower().Contains(fio)));
                }

                if (cmbSearchDolzhnost.SelectedValue != null &&
                    (int)cmbSearchDolzhnost.SelectedValue != 0)
                {
                    int idDolzhnosti = (int)cmbSearchDolzhnost.SelectedValue;
                    query = query.Where(s => s.ID_Dolzhnosti == idDolzhnosti);
                }

                dgEmployees.ItemsSource = query
                    .Include("tyr_Dolzhnosti")
                    .OrderBy(s => s.Familia)
                    .ThenBy(s => s.Imya)
                    .ToList();

                if (dgEmployees.Items.Count == 0)
                {
                    MessageBox.Show("Сотрудники по заданным параметрам не найдены.",
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
            cmbSearchDolzhnost.SelectedIndex = 0;
            LoadEmployees();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadEmployees();
        }

        private tyr_Sotrudniki GetSelectedEmployee()
        {
            return dgEmployees.SelectedItem as tyr_Sotrudniki;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EmployeeEditWindow();
            editWindow.Owner = this;

            if (editWindow.ShowDialog() == true)
                LoadEmployees();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var emp = GetSelectedEmployee();
            if (emp == null)
            {
                MessageBox.Show("Выберите сотрудника для редактирования.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var editWindow = new EmployeeEditWindow(emp);
            editWindow.Owner = this;

            if (editWindow.ShowDialog() == true)
                LoadEmployees();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var emp = GetSelectedEmployee();
            if (emp == null)
            {
                MessageBox.Show("Выберите сотрудника для удаления.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Удалить сотрудника «{emp.Familia} {emp.Imya}»?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _db?.Dispose();
                    _db = new user9_dbEntities();

                    var toDelete = _db.tyr_Sotrudniki
                        .FirstOrDefault(s => s.ID_Sotrudnika == emp.ID_Sotrudnika);

                    if (toDelete != null)
                    {
                        _db.tyr_Sotrudniki.Remove(toDelete);
                        _db.SaveChanges();
                    }

                    LoadEmployees();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не удалось удалить сотрудника. Возможно, на него есть заявки.\n" + ex.Message,
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}