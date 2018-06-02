using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanningPoker2018_backend_2.WebSockets
{
    public class WebSocketException: Exception
    {
        public WebSocketException(string message): base(message)
        {

        }
    }
}
