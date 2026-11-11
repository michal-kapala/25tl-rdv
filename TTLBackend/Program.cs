using System;
using System.Windows.Forms;

namespace TTLBackend
{
    static class Program
    {
        /// <summary>
        /// The entry point of the application.
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
