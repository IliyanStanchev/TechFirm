using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TechFirm.Models;
using TechFirm.View;

namespace TechFirm
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

            using (var db = new DatabaseContext())
            {
                db.Database.Initialize(true);
            }

            Application.Run(new LoginForm());
        }
    }
}
