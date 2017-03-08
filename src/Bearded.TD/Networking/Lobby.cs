using System;
using System.Collections.Generic;
using Bearded.Utilities;
using Lidgren.Network;

namespace Bearded.TD.Networking
{
    abstract class Lobby : IDisposable
    {
        protected const char Separator = '\n';

        protected Logger Logger { get; }
        protected string Username { get; }

        protected Lobby(Logger logger, string username)
        {
            Logger = logger;
            Username = username;
        }

        public abstract void Update();
        public abstract void SendChatMessage(string chatMsg);

        protected void PrintChatMessage(string username, string chatMsg)
        {
            Logger.Info.Log("{0}: {1}", username, chatMsg);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    class LobbyServer : Lobby
    {
        private readonly NetServer server;
        private readonly List<NetConnection> connectedPeers = new List<NetConnection>();

        public LobbyServer(Logger logger, string username) : base(logger, username)
        {
            var config = new NetPeerConfiguration(Constants.Network.ApplicationName)
            { Port = Constants.Network.DefaultPort };
            server = new NetServer(config);
            server.Start();
        }

        public override void Update()
        {
            NetIncomingMessage message;
            while ((message = server.ReadMessage()) != null)
            {
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        var data = message.ReadString().Split(Separator);
                        broadcastMessage(data[0], data[1]);
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        switch (message.SenderConnection.Status)
                        {
                            case NetConnectionStatus.Connected:
                                Logger.Debug.Log("Somebody connected :)");
                                connectedPeers.Add(message.SenderConnection);
                                break;
                            case NetConnectionStatus.Disconnected:
                                Logger.Debug.Log("Somebody disconnected :(");
                                connectedPeers.Remove(message.SenderConnection);
                                break;
                            default:
                                Logger.Debug.Log("Unhandled status change of type {0}", message.SenderConnection.Status);
                                break;
                        }
                        break;

                    case NetIncomingMessageType.DebugMessage:
                        Logger.Debug.Log("Debug: {0}", message.ReadString());
                        break;

                    default:
                        Logger.Debug.Log("Unhandled message with type {0}", message.MessageType);
                        break;
                }
            }
        }

        public override void SendChatMessage(string chatMsg)
        {
            broadcastMessage(Username, chatMsg);
        }

        private void broadcastMessage(string username, string chatMsg)
        {
            PrintChatMessage(username, chatMsg);
            if (connectedPeers.Count == 0)
                return;
            var msg = server.CreateMessage();
            msg.Write(username + Separator + chatMsg);
            server.SendMessage(msg, connectedPeers, NetDeliveryMethod.ReliableOrdered, (int)NetworkChannel.Chat);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing) return;
            base.Dispose(true);
            server.Shutdown("I don't hate you.");
        }
    }

    class LobbyClient : Lobby
    {
        private readonly NetClient client;

        public LobbyClient(Logger logger, string host, string username) : base(logger, username)
        {
            var config = new NetPeerConfiguration(Constants.Network.ApplicationName);
            client = new NetClient(config);
            client.Start();
            client.Connect(host, Constants.Network.DefaultPort);
        }

        public override void Update()
        {
            NetIncomingMessage message;
            while ((message = client.ReadMessage()) != null)
            {
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        var data = message.ReadString().Split(Separator);
                        PrintChatMessage(data[0], data[1]);
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        switch (message.SenderConnection.Status)
                        {
                            case NetConnectionStatus.Connected:
                                Logger.Debug.Log("Server connected :)");
                                break;
                            case NetConnectionStatus.Disconnected:
                                Logger.Debug.Log("Server disconnected :(");
                                break;
                            default:
                                Logger.Debug.Log("Unhandled status change of type {0}", message.SenderConnection.Status);
                                break;
                        }
                        break;

                    case NetIncomingMessageType.DebugMessage:
                        Logger.Debug.Log("Debug: {0}", message.ReadString());
                        break;

                    default:
                        Logger.Debug.Log("Unhandled message with type {0}", message.MessageType);
                        break;
                }
            }
        }

        public override void SendChatMessage(string chatMsg)
        {
            var msg = client.CreateMessage();
            msg.Write(Username + Separator + chatMsg);
            client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, (int)NetworkChannel.Chat);
        }
    }
}