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

namespace Server.TCP
{
    public class TcpServer
    {
        static Window1 serverGUI;

        private const Int32 PORT = 13000;

        private const String ADDRESS = "127.0.0.1";

        private IPAddress localAddress;

        public TcpListener tcpServer;

        private static Byte[] bytes;

        private static String data;

        static DateTime localDate;

        static CultureInfo cultureInfo;

        private static int clientCount = 0;

        static NetworkStream stream;

        private TcpClient client;

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
                    localAddress = IPAddress.Parse(ADDRESS);
                    tcpServer = new TcpListener(localAddress, PORT);
                    localDate = DateTime.Now;
                    tcpServer.Start();
                    while (true)
                    {
                        client = await tcpServer.AcceptTcpClientAsync();
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
                bytes = new Byte[256];
                data = null;
                // Get a stream object for reading and writing
                stream = client.GetStream();

                int i = 0;

                while (true)
                {
                    if (stream.CanRead)
                    {
                        i = await stream.ReadAsync(bytes, 0, bytes.Length);
                    }
                    localDate = DateTime.Now;

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
            if (client != null)
            {
                bytes = new Byte[256];
                data = null;
                // Get a stream object for reading and writing
                stream = client.GetStream();
                byte[] msg = null;
                msg = System.Text.Encoding.ASCII.GetBytes(serverGUI.GetMessage());
                if (stream.CanWrite)
                {
                    await stream.WriteAsync(msg, 0, msg.Length);
                }
                msg = System.Text.Encoding.ASCII.GetBytes("ceva");
                if (stream.CanRead)
                {

                    await stream.ReadAsync(bytes, 0, bytes.Length);
                }
            }

        }
    }
}
