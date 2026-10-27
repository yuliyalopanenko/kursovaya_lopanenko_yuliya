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
    public partial class ReportsWindow : Window
    {
        private user9_dbEntities _db;

        public ReportsWindow()
        {
            InitializeComponent();

            // По умолчанию — весь доступный период
            dpFrom.SelectedDate = new DateTime(2020, 1, 1);
            dpTo.SelectedDate = DateTime.Today.AddYears(1);

            GenerateReport();
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            GenerateReport();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            GenerateReport();
        }

        private void btnQuickPeriod_Click(object sender, RoutedEventArgs e)
        {
            var menu = new System.Windows.Controls.ContextMenu();

            var item1 = new System.Windows.Controls.MenuItem { Header = "Текущий месяц" };
            item1.Click += (s, args) =>
            {
                var t = DateTime.Today;
                dpFrom.SelectedDate = new DateTime(t.Year, t.Month, 1);
                dpTo.SelectedDate = t;
                GenerateReport();
            };

            var item2 = new System.Windows.Controls.MenuItem { Header = "Прошлый месяц" };
            item2.Click += (s, args) =>
            {
                var t = DateTime.Today.AddMonths(-1);
                dpFrom.SelectedDate = new DateTime(t.Year, t.Month, 1);
                dpTo.SelectedDate = new DateTime(t.Year, t.Month,
                    DateTime.DaysInMonth(t.Year, t.Month));
                GenerateReport();
            };

            var item3 = new System.Windows.Controls.MenuItem { Header = "Текущий год" };
            item3.Click += (s, args) =>
            {
                var t = DateTime.Today;
                dpFrom.SelectedDate = new DateTime(t.Year, 1, 1);
                dpTo.SelectedDate = new DateTime(t.Year, 12, 31);
                GenerateReport();
            };

            var item4 = new System.Windows.Controls.MenuItem { Header = "За всё время" };
            item4.Click += (s, args) =>
            {
                dpFrom.SelectedDate = new DateTime(2020, 1, 1);
                dpTo.SelectedDate = DateTime.Today.AddYears(1);
                GenerateReport();
            };

            menu.Items.Add(item1);
            menu.Items.Add(item2);
            menu.Items.Add(item3);
            menu.Items.Add(item4);

            menu.PlacementTarget = (System.Windows.Controls.Button)sender;
            menu.IsOpen = true;
        }

        private void GenerateReport()
        {
            try
            {
                _db?.Dispose();
                _db = new user9_dbEntities();

                if (!dpFrom.SelectedDate.HasValue || !dpTo.SelectedDate.HasValue)
                {
                    MessageBox.Show("Выберите период отчёта.",
                        "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DateTime from = dpFrom.SelectedDate.Value.Date;
                DateTime to = dpTo.SelectedDate.Value.Date;

                if (from > to)
                {
                    MessageBox.Show("Дата начала периода не может быть позже даты окончания.",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var query = _db.tyr_Zayavki
                    .Include("tyr_Klienty")
                    .Include("tyr_Tury")
                    .Include("tyr_Sotrudniki")
                    .Include("tyr_StatusyZayavok")
                    .Where(z => z.DataPodachi >= from && z.DataPodachi <= to);

                if (chkExcludeCancelled.IsChecked == true)
                {
                    query = query.Where(z => z.tyr_StatusyZayavok.Nazvanie != "Отменена");
                }

                var requests = query
                    .OrderBy(z => z.DataPodachi)
                    .ToList();

                // Детализация
                var details = requests.Select(z => new
                {
                    z.NomerZayavki,
                    z.DataPodachi,
                    ClientName = z.tyr_Klienty != null
                        ? $"{z.tyr_Klienty.Familia} {z.tyr_Klienty.Imya}"
                        : "(нет)",
                    TourName = z.tyr_Tury != null ? z.tyr_Tury.Nazvanie : "(нет)",
                    Strana = z.tyr_Tury != null ? z.tyr_Tury.Strana : "",
                    EmployeeName = z.tyr_Sotrudniki != null
                        ? $"{z.tyr_Sotrudniki.Familia} {z.tyr_Sotrudniki.Imya}"
                        : "(не назначен)",
                    StatusName = z.tyr_StatusyZayavok != null
                        ? z.tyr_StatusyZayavok.Nazvanie
                        : "",
                    z.Summa
                }).ToList();

                dgDetails.ItemsSource = details;

                // Сводка по турам
                var byTours = requests
                    .Where(z => z.tyr_Tury != null)
                    .GroupBy(z => new
                    {
                        TourName = z.tyr_Tury.Nazvanie,
                        Strana = z.tyr_Tury.Strana
                    })
                    .Select(g => new
                    {
                        g.Key.TourName,
                        g.Key.Strana,
                        Count = g.Count(),
                        TotalSum = g.Sum(x => x.Summa)
                    })
                    .OrderByDescending(x => x.TotalSum)
                    .ToList();

                dgByTours.ItemsSource = byTours;

                // Сводка по статусам
                var byStatuses = requests
                    .Where(z => z.tyr_StatusyZayavok != null)
                    .GroupBy(z => z.tyr_StatusyZayavok.Nazvanie)
                    .Select(g => new
                    {
                        StatusName = g.Key,
                        Count = g.Count(),
                        TotalSum = g.Sum(x => x.Summa)
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList();

                dgByStatuses.ItemsSource = byStatuses;

                // Итоговые карточки
                int totalCount = requests.Count;
                decimal totalSum = requests.Sum(z => z.Summa);
                decimal avgSum = totalCount > 0 ? totalSum / totalCount : 0;
                int uniqueTours = requests
                    .Where(z => z.tyr_Tury != null)
                    .Select(z => z.ID_Tura)
                    .Distinct()
                    .Count();

                tbTotalCount.Text = totalCount.ToString();
                tbTotalSum.Text = totalSum.ToString("N2") + " ₽";
                tbAvgSum.Text = avgSum.ToString("N2") + " ₽";
                tbUniqueTours.Text = uniqueTours.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка построения отчёта:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}