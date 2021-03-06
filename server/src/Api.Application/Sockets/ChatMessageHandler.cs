using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Api.Model.IServices.Chat;
using Api.Model.IServices.Users;

namespace Api.Application.Sockets
{
  public class ChatMessageHandler : WebSocketHandler
  {
    IChatService ServiceChat;

    IUserService ServiceUser;

    public ChatMessageHandler(ConnectionManager webSocketConnectionManager, IChatService service, IUserService serviceUser) : base(webSocketConnectionManager)
    {
      ServiceChat = service;
      ServiceUser = serviceUser;
    }

    public override async Task OnConnected(string username, WebSocket socket, Dictionary<string, string> values)
    {
      await base.OnConnected(username, socket, values);
    }

    /// <summary>
    /// Receber mensagens
    /// </summary>
    /// <param name="username">Nome do user</param>
    /// <param name="socket"></param>
    /// <param name="result"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public override async Task ReceiveAsync(string username, WebSocket socket, WebSocketReceiveResult result, Dictionary<string, string> values)
    {
      if (!values.ContainsKey("chat") || !values.ContainsKey("message") || !values.ContainsKey("time"))
        await WebSocketConnectionManager.RemoveSocket(WebSocketConnectionManager.GetId(socket));
      else
      {
        DateTime time = DateTime.Parse(values["time"]);
        var chat = await ServiceChat.NewMessage(Convert.ToInt32(values["chat"]), username, values["message"], time);
        var user = await ServiceUser.GetUserById(chat.IdUser);
        if (chat != null && user != null)
        {
          string json = jsonBuilder(user.Name, username, chat.IdChat, chat.Text, chat.Time);
          //
          await SendMessageToAllAsync(json.ToString());
        }
      }
    }


    /// <summary>
    /// Enviar mensagens
    /// </summary>
    /// <param name="socket"></param>
    /// <param name="message">Mensagem a enviar</param>
    /// <returns></returns>
    public override async Task SendMessageAsync(WebSocket socket, string message)
    {
      if (socket.State != WebSocketState.Open)
        return;

      await socket.SendAsync(buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(message),
                                                                  offset: 0,
                                                                  count: message.Length),
                                  messageType: WebSocketMessageType.Text,
                                  endOfMessage: true,
                                  cancellationToken: CancellationToken.None);
    }

    /// <summary>
    /// Enviar uma mensagem para todos os users
    /// </summary>
    /// <param name="json">Mensagem a enviar</param>
    /// <returns></returns>
    public override async Task SendMessageToAllAsync(string json)
    {
      foreach (var pair in WebSocketConnectionManager.GetAll())
      {
        if (pair.Value.State == WebSocketState.Open)
          await SendMessageAsync(pair.Value, json);
      }
    }


    /// <summary>
    /// Construir um json com a mensagem e os dados de envio
    /// </summary>
    /// <param name="name"></param>
    /// <param name="username">Nome do user</param>
    /// <param name="chat">Id do chat</param>
    /// <param name="message">Mensagem a enviar</param>
    /// <param name="time">Data do envio da mensagem</param>
    /// <returns>Retorna um json com os dados formatados</returns>
    public string jsonBuilder(string name, string username, int chat, string message, DateTime? time)
    {
      StringBuilder json = new StringBuilder();
      json.Append("{");
      json.Append("\"type\":");
      json.Append("\"" + 200 + "\",");
      json.Append("\"username\":");
      json.Append("\"" + username + "\",");
      json.Append("\"name\":");
      json.Append("\"" + name + "\",");
      json.Append("\"chat\": ");
      json.Append(chat + ",");
      json.Append("\"message\":");
      json.Append("\"" + message + "\",");
      json.Append("\"date\":");
      json.Append("\"" + time.Value.ToString("dd/MM/yyyy hh:mm") + "\"");
      json.Append("}");
      return json.ToString();
    }
  }
}
