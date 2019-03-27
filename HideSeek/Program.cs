using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

// Tubes2StrAlgo_Ahboy
// Eka Novendra Wahyunadi / 13517011
// Marsa Thoriq Ahmada / 13517071
// Mgs.Muhammad Riandi Ramadhan / 13517080

namespace HideSeek
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
