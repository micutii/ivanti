using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.TCP
{
    class Response
    {
        public string response { get; set; }

        public Response(string r)
        {
            response = r;
        }
    }
}
