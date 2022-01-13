using System;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WurmSermoner.Services;

namespace WurmSermoner
{
    public partial class frmWurmSermoner : Form
    {
        private WurmSermoner sermoner;
        private SermonService sermon;
        public frmWurmSermoner(WurmSermoner wser)
        {
            sermoner = wser;
            sermon = (SermonService)wser.services.GetService(typeof(SermonService));
            InitializeComponent();

            CheckLogFile();
        }

        private void btnStuff_Click(object sender, EventArgs e)
        {
            CheckLogFile();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            sermoner.ConnectBot();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            sermoner.DisconnectBot();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (sermoner.Connected)
            {
                tsBot.Image = global::WurmSermoner.Properties.Resources.check;
            }
            else
            {
                tsBot.Image = global::WurmSermoner.Properties.Resources.multiply;
            }

            if (sermon.preachers.Count > 0)
            {
                txtList.Text = sermon.preachers.GetDiscordList();
            }
        }

        private void CheckLogFile()
        {
            if (txtLogsDir.Text.Length > 0 && txtLogFile.Text.Length > 0)
            {
                if (File.Exists(txtLogsDir.Text + txtLogFile.Text))
                {
                    sermoner.Operator = txtOperator.Text;
                    sermoner.OperatorDir = txtLogsDir.Text;
                    sermoner.LogFile = txtLogFile.Text;
                    sermoner.LogFileConfirmed = true;
                    tsLogFile.Image = global::WurmSermoner.Properties.Resources.check;
                }
                else
                {
                    tsLogFile.Image = global::WurmSermoner.Properties.Resources.multiply;
                }
            }
        }

        private void frmWurmSermoner_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sermoner.Connected)
            {
                sermoner.DisconnectBot();
            }
        }
    }
}
