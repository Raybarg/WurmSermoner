using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace WurmSermonerServer
{
    class Program
    {
        static void Main()
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }


        public async Task MainAsync()
        {
            UdpClient server = new UdpClient(11000);
            while (true)
            {
                var remote = new IPEndPoint(IPAddress.Any, 11000);
                var data = server.Receive(ref remote);
                Console.WriteLine("Receive data from " + remote.ToString());
                Console.WriteLine(ASCIIEncoding.ASCII.GetString(data));
                server.Send(data, data.Length, remote);
            }
        }
    }
}
