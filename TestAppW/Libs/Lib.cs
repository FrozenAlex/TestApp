using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace TestApp.Libs
{
    class Helper
    {
        public static void ShowHelp()
        {
            try
            {
                Process.Start("Resources\\Help.chm");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }
    }
}
