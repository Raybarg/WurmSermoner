using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Configuration;
using WurmSermoner.Helpers;

namespace WurmSermoner
{
    public class WurmSermonerServer
    {
        public bool ServerRunning;

        private string defaultPort = "11000"; // Default port 11000
        UdpClient server;

        public async Task MainAsync()
        {
            server = new UdpClient(int.Parse(Helpers.ConfigHelper.addGet("Port", defaultPort)));
            ServerRunning = true;
            while (true)
            {
                try
                {
                    server.BeginReceive(new AsyncCallback(recv), null);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                /*
                var remote = new IPEndPoint(IPAddress.Any, 11000);
                var data = server.Receive(ref remote);
                Console.WriteLine("Receive data from " + remote.ToString());
                Console.WriteLine(EncryptionHelper.Decrypt(Encoding.UTF8.GetString(data)));
                server.Send(data, data.Length, remote);
                */
                await Task.Delay(1000);
            }
            //Console.ReadLine();
        }

        private void recv(IAsyncResult res)
        {
            IPEndPoint remote = new IPEndPoint(IPAddress.Any, 11000);
            byte[] received = server.EndReceive(res, ref remote);
            Console.WriteLine("Receive data from " + remote.ToString());
            Console.WriteLine(EncryptionHelper.Decrypt(Encoding.UTF8.GetString(received)));
            server.Send(received, received.Length, remote);
            server.BeginReceive(new AsyncCallback(recv), null);
        }
    }
}
