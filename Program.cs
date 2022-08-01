using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace WurmSermoner
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var sermonerServer = new WurmSermonerServer();
            if (Helpers.AppSettingHelper.RunServer())
                _ = sermonerServer.MainAsync();

            var sermoner = new WurmSermoner();
            _ = sermoner.MainAsync();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmWurmSermoner(sermoner, sermonerServer));
        }
    }
}
