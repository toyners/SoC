using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SoC.WebApplication.Requests
{
    public class RequestBase
    {
        public string UserName { get; set; }
        public string ConnectionId { get; set; }
    }
}
