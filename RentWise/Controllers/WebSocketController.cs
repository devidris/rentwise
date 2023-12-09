using System.Net;
using System.Net.WebSockets;
using System.Text; // Add this using directive
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RentWise.Controllers
{
    public class WebSocketController : Controller
    {
        private static List<WebSocket> connectedClients = new List<WebSocket>();
        // GET: /WebSocket/Index
        public ActionResult Index()
        {
            return View();
        }

        // WebSocket endpoint: /WebSocket/Socket
        public async Task<ActionResult> Socket()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync() as WebSocket;

                await HandleWebSocket(webSocket);

                return new EmptyResult();
            }

            return new StatusCodeResult(400);
        }
        private async Task HandleWebSocket(WebSocket webSocket)
        {
            connectedClients.Add(webSocket);

            // Notify the client about the restart
            await SendMessage(webSocket, "Server is restarting. Please reconnect.");

            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;

            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    await SendMessage(webSocket, $"You said: {message}");

                    foreach (var client in connectedClients)
                    {
                        await SendMessage(client, $"Broadcast: {message}");
                    }
                }
                else if (result.MessageType == WebSocketMessageType.Binary)
                {
                   
                }

            } while (!result.CloseStatus.HasValue);

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
        private async Task SendMessage(WebSocket webSocket, string message)
        {
            var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
            await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
