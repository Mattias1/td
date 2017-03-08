namespace Bearded.TD.Networking
{
    class NetworkManager
    {
        public static NetworkManager Instance { get; }

        static NetworkManager()
        {
            Instance = new NetworkManager();
        }

        private NetworkManager() { }

        private Lobby lobby;
        public Lobby Lobby
        {
            get { return lobby; }
            set
            {
                lobby?.Dispose();
                lobby = value;
            }
        }
    }
}