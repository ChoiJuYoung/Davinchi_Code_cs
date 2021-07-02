using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Davinchi_Code
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// Davinchi Code is made by Oh Zi-Seok, Choi Ju-Young
        /// Computer Science, University Of Seoul
        /// if you find any error, send mail to hajuu96123@naver.com thnks
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());
        }
    }
}
