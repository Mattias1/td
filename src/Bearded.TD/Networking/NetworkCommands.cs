using Bearded.TD.Utilities.Console;
using Bearded.Utilities;

namespace Bearded.TD.Networking
{
    static class NetworkCommands
    {
        private static readonly NetworkManager manager = NetworkManager.Instance;

        [Command("lobby.create")]
        public static void CreateLobby(Logger logger, CommandParameters parameters)
        {
            if (parameters.Args.Length != 1)
            {
                logger.Error.Log("Usage: lobby.create username");
                return;
            }
            
            manager.Lobby = new LobbyServer(logger, parameters.Args[0]);
        }

        [Command("lobby.join")]
        public static void JoinLobby(Logger logger, CommandParameters parameters)
        {
            if (parameters.Args.Length != 2)
            {
                logger.Error.Log("Usage: lobby.join host username");
                return;
            }

            manager.Lobby = new LobbyClient(logger, parameters.Args[0], parameters.Args[1]);
        }

        [Command("lobby.chat")]
        public static void ChatInLobby(Logger logger, CommandParameters parameters)
        {
            if (manager.Lobby == null)
            {
                logger.Warning.Log("Create or join a lobby before sending messages");
                return;
            }

            manager.Lobby.SendChatMessage(string.Join(" ", parameters.Args));
        }
    }
}