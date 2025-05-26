namespace QuickServerStatuses.Models.Servers
{
    public class Server
    {
        public string Name { get; set; }
        public ServerStatus Status { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }
        public DateTime LastUpdate { get; set; }

    }
}
