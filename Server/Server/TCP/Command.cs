using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.TCP
{
    class Command
    {
        public int Id { get; set; }
        public string Parameters { get; set; }

        public Command(int id, string parameters)
        {
            Id = id;
            Parameters = parameters;
        }
    }
}
