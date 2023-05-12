using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WurmSermoner.Sermon;
using Discord;
using System.Windows.Forms;

namespace WurmSermoner.Services
{
    public class SermonService
    {
        public PreacherList preachers = new PreacherList();
        public UserList users = new UserList();

        public IUserMessage lastMessage;
        public IUserMessage lastListMessage;
        public DateTime lastListMessageTime;

        public List<IUserMessage> sermonMessages = new List<IUserMessage>();

        public void ListMessageUpdate()
        {
            lastListMessage = lastMessage;
            lastListMessageTime = DateTime.Now;

        }

        public async void ListMessageUpdateTick()
        {
            if (Convert.ToInt32(DateTime.Now.Subtract(lastListMessageTime).TotalMinutes) >= 1 && lastListMessage != null)
            {
                try
                {
                    await lastListMessage.ModifyAsync(m => { m.Content = preachers.GetDiscordList(users); });
                    lastListMessageTime = DateTime.Now;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public async void RemoveSermonMessages()
        {
            foreach (IUserMessage msg in sermonMessages)
            {
                await msg.DeleteAsync();
            }
            sermonMessages.Clear();
        }

        public async void RemoveLastSermonList()
        {
            if (lastListMessage != null) await lastListMessage.DeleteAsync();
            lastListMessage = null;
        }
    }
}
