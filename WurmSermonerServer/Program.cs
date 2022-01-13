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
            /*
            try
            {
                //---listen at the specified IP and port no.---
                IPAddress localAdd = IPAddress.Parse("127.0.0.1");
                TcpListener listener = new TcpListener(localAdd, 5000);
                Console.WriteLine("Listening...");
                listener.Start();
                while (true)
                {
                    //---incoming client connected---
                    TcpClient client = listener.AcceptTcpClient();

                    //---get the incoming data through a network stream---
                    NetworkStream nwStream = client.GetStream();
                    byte[] buffer = new byte[client.ReceiveBufferSize];

                    //---read incoming stream---
                    int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

                    //---convert the data received into a string---
                    string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Received : " + dataReceived);

                    

                    //---write back the text to the client---
                    Console.WriteLine("Sending back : " + dataReceived);
                    nwStream.Write(buffer, 0, bytesRead);
                    client.Close();
                }
                listener.Stop();
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            */
        }
    }
}
