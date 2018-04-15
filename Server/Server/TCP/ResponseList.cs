using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.TCP
{
    class ResponseList
    {
        public List<string> response { get; private set; }

        public ResponseList()
        {
            response = new List<string>();
        }
    }
}
