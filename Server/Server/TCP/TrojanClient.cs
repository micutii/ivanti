﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.TCP
{
    public class TrojanClient
    {
        public TcpClient Client { get; set; }
        public string Name { get; set; }

        public TrojanClient(TcpClient tcpClient, string name)
        {
            Client = tcpClient;
            Name = name;
        }
    }
}
