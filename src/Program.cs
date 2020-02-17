using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Android_Image_Resizer
{
    static class Program
    {

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Main Form
            Application.Run(new Form1());
        }
    }
}
