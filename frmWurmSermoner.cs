using System;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        private WurmSermonerServer sermonerServer;
        private SermonService sermon;
        public frmWurmSermoner(WurmSermoner wser, WurmSermonerServer wserServer)
        {
            sermoner = wser;
            sermonerServer = wserServer;
            sermon = (SermonService)wser.services.GetService(typeof(SermonService));
            InitializeComponent();

            txtOperator.Text = ConfigurationManager.AppSettings.Get("Operator");
            txtLogsDir.Text = ConfigurationManager.AppSettings.Get("LogDir");
            txtLogFile.Text = ConfigurationManager.AppSettings.Get("LogFile");

            txtBotToken.Text = ConfigurationManager.AppSettings.Get("BotToken");
            txtGuildID.Text = ConfigurationManager.AppSettings.Get("GuildID");
            txtChannelID.Text = ConfigurationManager.AppSettings.Get("ChannelID");

            txtAddress.Text = ConfigurationManager.AppSettings.Get("Server");
            txtPort.Text = ConfigurationManager.AppSettings.Get("Port");

            string RunServer = ConfigurationManager.AppSettings.Get("RunServer");
            if (RunServer != null)
            {
                chkRunServer.Checked = bool.Parse(RunServer);
            }

            CheckServerSettings();
            CheckBotSettings();
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
                txtList.Text = sermon.preachers.GetDiscordList(sermon.users);
            }

            if (sermoner.ServerResponded)
                tsSermonServer.Image = global::WurmSermoner.Properties.Resources.check;
            else
                tsSermonServer.Image = global::WurmSermoner.Properties.Resources.multiply;

            if(sermonerServer.ServerRunning)
                tsLocalServer.Image = global::WurmSermoner.Properties.Resources.check;
            else
                tsLocalServer.Image = global::WurmSermoner.Properties.Resources.multiply;
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

        private void CheckBotSettings()
        {
            bool ReconnectBot = false;
            if (sermoner.Connected)
            {
                sermoner.DisconnectBot();
                ReconnectBot = true;
            }

            sermoner.BotToken = txtBotToken.Text;
            if (!string.IsNullOrEmpty(txtGuildID.Text))
                sermoner.GuildID = ulong.Parse(txtGuildID.Text);
            if (!string.IsNullOrEmpty(txtChannelID.Text))
                sermoner.ChannelID = ulong.Parse(txtChannelID.Text);

            if (ReconnectBot)
                sermoner.ConnectBot();
                
        }

        private void CheckServerSettings()
        {
            sermoner.Server = txtAddress.Text;
            sermoner.Port = txtPort.Text;
        }

        private void frmWurmSermoner_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sermoner.Connected)
            {
                sermoner.DisconnectBot();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckBotSettings();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CheckServerSettings();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Helpers.ConfigHelper.addUpdate("Operator", txtOperator.Text);
            Helpers.ConfigHelper.addUpdate("LogDir", txtLogsDir.Text);
            Helpers.ConfigHelper.addUpdate("LogFile", txtLogFile.Text);
            Helpers.ConfigHelper.addUpdate("BotToken", txtBotToken.Text);
            Helpers.ConfigHelper.addUpdate("GuildID", txtGuildID.Text);
            Helpers.ConfigHelper.addUpdate("ChannelID", txtChannelID.Text);
            Helpers.ConfigHelper.addUpdate("Server", txtAddress.Text);
            Helpers.ConfigHelper.addUpdate("Port", txtPort.Text);
            Helpers.ConfigHelper.addUpdate("RunServer", chkRunServer.Checked.ToString());

            CheckServerSettings();
            CheckBotSettings();
            CheckLogFile();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            openFileDialog1.FileName = "_Event." + DateTime.Today.Year.ToString() + "," + DateTime.Today.Month.ToString() + ".txt";


            if (txtLogsDir.Text.Length > 0 && txtLogFile.Text.Length > 0)
            {
                if (File.Exists(txtLogsDir.Text + txtLogFile.Text))
                {
                    openFileDialog1.InitialDirectory = txtLogsDir.Text;
                    openFileDialog1.FileName = txtLogFile.Text;
                }
            }

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileInfo file = new FileInfo(openFileDialog1.FileName);
                txtLogsDir.Text = file.Directory + @"\";
                txtLogFile.Text = file.Name;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            sermon.users.ToggleAfk(textBox1.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            sermoner.irc.Connect();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            sermoner.irc.Disconnect();
        }
    }
}
