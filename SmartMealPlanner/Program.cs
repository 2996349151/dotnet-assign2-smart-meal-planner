using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartMealPlanner.Core.Data.Repositories;
using SmartMealPlanner.Core.Services;
using WinFormsAppFinal.UI;
using System.IO;
using System.Text.Json;
using System.Diagnostics;
using SmartMealPlanner.UI;
namespace SmartMealPlanner
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Launch WelcomeForm as main window
                var form = new WelcomeForm();
                Application.Run(form);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Startup error: " + ex.ToString());
            }
        }
    }
}



