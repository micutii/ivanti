using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.TCP
{
    class ComplexCommand
    {
        public int Id { get; set; }
        public List<string> Parameters { get; private set; }

        public ComplexCommand(int id)
        {
            Id = id;
            Parameters = new List<string>();
        }
    }
}
