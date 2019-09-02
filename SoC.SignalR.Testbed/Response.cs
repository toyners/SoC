using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoC.SignalR.Testbed
{
    public class Response
    {
        public Response(string content) => this.Content = content;

        public string Content { get; set; }
    }
}
