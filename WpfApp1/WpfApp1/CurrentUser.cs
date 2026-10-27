using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public static class CurrentUser
    {
        public static int ID_User { get; set; }
        public static string Login { get; set; }
        public static string Role { get; set; }
        public static string FullName { get; set; }
    }
}