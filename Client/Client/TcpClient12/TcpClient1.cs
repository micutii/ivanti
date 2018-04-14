using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client.TcpClient12
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TcpClient1
    {
        private Int32 port = 13000;

        private String hostname = "192.168.21.100";

        public TcpClient tcpClient;

        DateTime localDate;

        CultureInfo cultureInfo;

        BackgroundWorker worker1;

        NetworkStream stream;

        public TcpClient1()
        {
            cultureInfo = CultureInfo.DefaultThreadCurrentCulture;

            Connect();

            worker1 = new BackgroundWorker();
            worker1.DoWork += Worker1_DoWorkAsync;
            worker1.RunWorkerAsync();
            while (true) ;
        }

        private void Connect()
        {
            try
            {
                tcpClient = new TcpClient(hostname, port);

                if (tcpClient.Connected)
                {
                    localDate = DateTime.Now;

                    stream = tcpClient.GetStream();
                    worker1.RunWorkerAsync();
                }
                else
                {
                    Console.WriteLine("Not connected");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private async void Worker1_DoWorkAsync(object sender, DoWorkEventArgs e)
        {
            try
            {
                byte[] msg = System.Text.Encoding.ASCII.GetBytes("My Name");
                if (stream.CanWrite)
                {
                    await stream.WriteAsync(msg, 0, msg.Length);
                }
                while (true)
                {

                    Byte[] data = System.Text.Encoding.ASCII.GetBytes("Hallo");

                    data = new Byte[256];

                    // String to store the response ASCII representation.
                    String responseData = String.Empty;

                    // Read the first batch of the TcpServer response bytes.
                    Int32 bytes = 0;
                    if (stream.CanRead)
                    {
                        bytes = await stream.ReadAsync(data, 0, data.Length);
                    }


                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    Console.WriteLine(responseData);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

        }
    }
}