using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    QueuedPriests.Add(priest);
                }
            }
        }
        
        public void Add(string priestName)
        {
            QueuedPriests.Add(priestName);
        }

        public string ListAll()
        {            
            return String.Join(",", QueuedPriests);
        }

        public string ListQueue()
        {
            return "Queue: " + String.Join(",", QueuedPriests);
        }

        public void RemoveIfFirst(string priestName)
        {
            if (QueuedPriests.Count > 0)
            {
                if (QueuedPriests[0] == priestName)
                {
                    QueuedPriests.RemoveAt(0);
                }
            }
        }

        public void Remove(string priestName)
        {
            QueuedPriests.Remove(priestName);
        }

        public void Push(string priestName)
        {
            QueuedPriests.Remove(priestName);
            QueuedPriests.Add(priestName);
        }
    }
}
