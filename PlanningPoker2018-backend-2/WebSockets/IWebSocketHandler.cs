using PlanningPoker2018_backend_2.Fleck.Interfaces;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace PlanningPoker2018_backend_2.WebSockets
{
    public interface IWebSocketHandler
    {
        Task handleNewSocket(AppWebSocket socket);
        void handleSocketClose(AppWebSocket socket);
        void handleNewMessage(AppWebSocket socket, string message);

    }
}