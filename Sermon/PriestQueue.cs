using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WurmSermoner.Helpers;

namespace WurmSermoner.Sermon
{
    public class PriestQueue
    {
        List<string> QueuedPriests = new List<string>();

        public PriestQueue(string initializeListOfPriests)
        {
            var priests = initializeListOfPriests.Split(',');
            if (priests.Length > 0)
            {
                foreach (var priest in priests)
                {
                    Add(priest);
                }
            }
        }
        
        public void Add(string priestName)
        {
            QueuedPriests.Add(TextHelper.ToTitleCase(priestName));
        }

        public string ListAll()
        {            
            return String.Join(",", QueuedPriests);
        }

        public string ListAllEmbed()
        {
            return String.Join("\n", QueuedPriests);
        }

        public string ListAllTimesEmbed()
        {
            List<string> times = new List<string>();
            foreach (var priest in QueuedPriests)
            {
                times.Add("12:34");
            }
            return String.Join("\n", times);
        }

        public string ListQueue()
        {
            return "Queue: " + String.Join(",", QueuedPriests);
        }

        public void RemoveIfFirst(string priestName)
        {
            if (QueuedPriests.Count > 0)
            {
                if (QueuedPriests[0] == TextHelper.ToTitleCase(priestName))
                {
                    QueuedPriests.RemoveAt(0);
                }
            }
        }

        public void Remove(string priestName)
        {
            QueuedPriests.Remove(TextHelper.ToTitleCase(priestName));
        }

        public void Push(string priestName)
        {
            QueuedPriests.Remove(TextHelper.ToTitleCase(priestName));
            QueuedPriests.Add(TextHelper.ToTitleCase(priestName));
        }

        public string FirstInQueue()
        {
            if (QueuedPriests.Count > 0)
            {
                return QueuedPriests[0];
            }
            else
            {
                return "";
            }
        }
        
        public void Swap(string priestFrom, string priestTo)
        {
            priestFrom = TextHelper.ToTitleCase(priestFrom);
            priestTo = TextHelper.ToTitleCase(priestTo);
            int from = QueuedPriests.IndexOf(priestFrom);
            int to = QueuedPriests.IndexOf(priestTo);

            if (from > -1 && to > -1)
            {
                QueuedPriests[from] = priestTo;
                QueuedPriests[to] = priestFrom;
            }
        }

        public void Rewrite(string rewrite)
        {
            QueuedPriests.Clear();
            var priests = rewrite.Split(',');
            if (priests.Length > 0)
            {
                foreach (var priest in priests)
                {
                    Add(priest);
                }
            }
        }
    }
}
