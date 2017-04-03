using System;//STAThread
using System.Diagnostics;//Process
using System.Windows.Forms;//Application

namespace Jp.Co.Kensan.ShortcutTable
{

    static class Program
    {
        [STAThread]//無くても動くけど？
        static void Main()
        {
            //二重起動をチェックする
            Process proOwn = Process.GetCurrentProcess();
            Process[] proArray = Process.GetProcessesByName(proOwn.ProcessName);
            if (proArray.Length > 1)
            {
                //return; // 起動せずに終了
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
