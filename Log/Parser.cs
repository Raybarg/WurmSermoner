using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WurmSermoner.Sermon;
using WurmSermoner.Services;

namespace WurmSermoner.Log
{
    public class Parser
    {
        public event EventHandler<SermonEventArgs> SermonEventOccurred;

        private string _operator = null;
        private string _path = null;
        private CancellationTokenSource cts;
        private Task parseTask;
        private bool bInitFirstTime = true;

        public bool IsRunning
        { 
            get
            {
                if (parseTask == null)
                {
                    return false;
                }
                return parseTask.Status == TaskStatus.Running;
            }
        }

        public Parser(SermonService sermon)
        {
            
        }

        public void Init(string path, string op)
        {
            _operator = op;
            _path = path;
        }

        public void Start()
        {
            DateTime curTime = DateTime.ParseExact("00:00:00", "HH:mm:ss", CultureInfo.InvariantCulture);

            cts = new CancellationTokenSource();
            parseTask = Task.Run(() =>
            {
                FileStream fs = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs);

                while (!cts.IsCancellationRequested)
                {
                    if (fs != null && sr != null)
                    {
                        string line;
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
                                    lineSplit[1] = _operator;
                                }

                                Sermon.Sermon s = new Sermon.Sermon();
                                s.Preacher = lineSplit[1];
                                s.Time = td;
                                OnSermonEventOccurred(new SermonEventArgs { RawLine = line, Sermon = s });
                            }
                        }
                    }

                    Thread.Sleep(200);
                }
            }, cts.Token);
        }

        public void Stop()
        {
            cts.Cancel();

            try
            {
                parseTask.Wait();
            } 
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.InnerExceptions)
                {
                    Console.WriteLine(innerEx.Message);
                }
            }
        }

        protected virtual void OnSermonEventOccurred(SermonEventArgs e)
        {
            EventHandler<SermonEventArgs> handler = SermonEventOccurred;
            if(handler != null )
            {
                handler(this, e);
            }
        }
    }
}
