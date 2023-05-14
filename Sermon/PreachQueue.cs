using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WurmSermoner.Sermon
{
    public class PreachQueue
    {
        private PreachSlot[] Queue;
        private int Front;
        private int Rear;
        private int Size;
        private int Capacity;

        public PreachQueue(int capacity)
        {
            this.Queue = new PreachSlot[capacity];
            this.Front = 0;
            this.Rear = capacity - 1;
            this.Size = 0;
            this.Capacity = capacity;

            DateTime time = DateTime.Now;

            for (int i = 0; i < capacity; i++)
            {
                time = time.AddMinutes(30);
                this.Queue[i] = new PreachSlot("empty", time);
            }
        }

        public bool IsEmpty()
        {
            return (this.Size == 0);
        }

        public bool IsFull()
        {
            return (this.Size == this.Capacity);
        }

        public void Enqueue(PreachSlot item)
        {
            if (IsFull())
            {
                throw new InvalidOperationException("Queue is full");
            }

            this.Rear = (this.Rear + 1) % this.Capacity;
            this.Queue[this.Rear] = item;
            this.Size++;
        }

        public PreachSlot Dequeue()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Queue is empty");
            }

            PreachSlot item = this.Queue[this.Front];
            this.Front = (this.Front + 1) % this.Capacity;
            this.Size--;
            return item;
        }

        public PreachSlot Peek()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Queue is empty");
            }

            return this.Queue[this.Front];
        }

        public int Count()
        {
            return this.Size;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("```diff");
            for (int i = 0; i < this.Capacity; i++)
            {
                if (this.Queue[i].Name == "empty")
                    sb.Append("- ");
                else
                    sb.Append("+ ");
                sb.AppendLine(i.ToString() + " " + this.Queue[i].Name + " " + this.Queue[i].Time.Hour + ":" + this.Queue[i].Time.Minute);
            }
            sb.Append("```");
            return sb.ToString();
        }

        public string ReserveSlot(int Slot, string Name, string DiscordID)
        {
            string err = "";
            if (Slot < 0 || Slot >= this.Capacity)
            {
                err = "Queue Slot out of bounds.";
            }

            if (this.Queue[Slot] == null)
            {
                err = "Queue Slot is null";
            }

            if (!this.Queue[Slot].IsEmpty())
            {
                if (!this.Queue[Slot].IsOwnedOrFree(DiscordID))
                {
                    err = "Slot already reserved.";
                }
            }

            if (String.IsNullOrEmpty(err))
            {
                this.Queue[Slot].Name = Name;
                this.Queue[Slot].DiscordID = DiscordID;
            }
            
            return err;
        }
    }

}
