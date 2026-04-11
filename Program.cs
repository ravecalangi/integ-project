using System;
using System.Windows.Forms;

namespace Program
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Show splash first
            var splash = new SplashForm();
            splash.ShowDialog(); // ← ShowDialog instead of Application.Run

            // Then run login as main form
            Application.Run(new LoginForm());
        }
    }
}