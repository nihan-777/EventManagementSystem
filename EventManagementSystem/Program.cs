using System;
using System.Windows.Forms;
using EventManagementSystem.Database;
using EventManagementSystem.Forms;

namespace EventManagementSystem
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DatabaseHelper.InitializeDatabase();
            Application.Run(new LoginForm());
        }
    }
}