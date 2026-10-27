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
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnTours_Click(object sender, RoutedEventArgs e)
        {
            new ToursWindow().ShowDialog();
        }

        private void btnClients_Click(object sender, RoutedEventArgs e)
        {
            new ClientsWindow().ShowDialog();
        }

        private void btnEmployees_Click(object sender, RoutedEventArgs e)
        {
            new EmployeesWindow().ShowDialog();
        }

        private void btnRequests_Click(object sender, RoutedEventArgs e)
        {
            new RequestsWindow().ShowDialog();
        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {
            
            new ReportsWindow().ShowDialog();
        }
        

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Выйти из системы?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
                Application.Current.Shutdown();
        }
    }
}