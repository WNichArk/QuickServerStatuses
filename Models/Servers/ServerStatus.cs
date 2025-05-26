namespace QuickServerStatuses.Models.Servers
{
    public enum ServerStatus : int
    {
        Unknown = 0,
        Online = 1,
        Offline = 2,
        Maintenance = 3,
        Error = 4
    }
}
