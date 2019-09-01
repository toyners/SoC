using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoC.SignalR.Testbed
{
    public class Request
    {
        //public int RequestType { get; set; }
    }

    public class JoinRequest : Request
    {
        public Guid GameId { get; set; }
    }
}
