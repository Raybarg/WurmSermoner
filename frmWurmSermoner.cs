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
        public frmWurmSermoner(WurmSermoner wser)
        {
            sermoner = wser;
            InitializeComponent();
        }

        private void btnStuff_Click(object sender, EventArgs e)
        {
            bool bWokeUp = true;
            string Operator = txtOperator.Text;
            string OperatorDir = txtLogsDir.Text;
            string LogFile = txtLogFile.Text;

            try
            {
                bool bInitFirstTime = true;
                DateTime curTime = DateTime.ParseExact("00:00:00", "HH:mm:ss", CultureInfo.InvariantCulture);
                var fs = new FileStream(OperatorDir + LogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (StreamReader sr = new StreamReader(fs))
                {
                    string line;
                    SermonService sermon = (SermonService)sermoner.services.GetService(typeof(SermonService));

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("Logging started"))
                        {
                            string temp = line.Substring(16);
                            DateTime dt = DateTime.Parse(temp);
                            Console.WriteLine("Logstart: " + dt.ToString());

                            curTime = new DateTime(dt.Year, dt.Month, dt.Day, curTime.Hour, curTime.Minute, curTime.Second);
                            bInitFirstTime = true;
                        }

                        if (line.StartsWith("["))
                        {
                            string temp = line.Substring(1, 8);
                            DateTime td = DateTime.ParseExact(temp, "HH:mm:ss", CultureInfo.InvariantCulture);

                            if (bInitFirstTime)
                            {
                                bInitFirstTime = false;
                                curTime = new DateTime(curTime.Year, curTime.Month, curTime.Day, td.Hour, td.Minute, td.Second);
                            }

                            if (curTime.Hour > td.Hour)
                            {
                                DateTime tempTime = new DateTime(curTime.Year, curTime.Month, curTime.Day + 1, td.Hour, td.Minute, td.Second);
                                curTime = tempTime;
                                Console.WriteLine("Day changed.");
                            }
                            else
                            {
                                DateTime tempTime = new DateTime(curTime.Year, curTime.Month, curTime.Day, td.Hour, td.Minute, td.Second);
                                curTime = tempTime;
                            }
                        }

                        // Sermon
                        if (line.Contains("finish") && line.Contains("sermon"))
                        {
                            string time = line.Substring(1, 8);
                            DateTime td = DateTime.ParseExact(time, "HH:mm:ss", CultureInfo.InvariantCulture);
                            td = new DateTime(curTime.Year, curTime.Month, curTime.Day, td.Hour, td.Minute, td.Second);

                            string[] lineSplit = line.Split(' ');
                            if (lineSplit[1] == "You")
                            {
                                lineSplit[1] = Operator;
                            }

                            sermon.preachers.AddPreacher(lineSplit[1], td);

                            Console.WriteLine("At: " + td.ToString("dd-MM-yyyy HH:mm:ss") + " by " + lineSplit[1]);
                            Console.WriteLine(line);

                            sermon.preachers.ResetAnnouncements(false);
                        }
                    }
                    Thread.Sleep(200);

                    if (bWokeUp)
                    {
                        Thread.Sleep(2000);
                        DateTime last;
                        bWokeUp = false;
                        //preachers
                        if (sermon.preachers.Count > 0)
                        {
                            Console.WriteLine("I woke up and last sermon was at " + sermon.preachers.LastSermon().ToString("dd-MM-yyyy HH:mm:ss"));
                            last = sermon.preachers.LastSermon();
                        }
                        else
                        {
                            last = DateTime.Now;
                        }
                        double diff = DateTime.Now.Subtract(last).TotalMinutes;
                        Console.WriteLine("I woke up and last sermon was at " + last.ToString("dd-MM-yyyy HH:mm:ss") + " this is `" + Convert.ToInt32(diff).ToString() + "` minutes ago");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            sermoner.ConnectBot();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            sermoner.DisconnectBot();
        }
    }
}
