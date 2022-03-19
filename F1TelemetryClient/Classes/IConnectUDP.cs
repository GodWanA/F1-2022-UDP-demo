namespace F1TelemetryApp.Classes
{
    public interface IConnectUDP
    {
        public void SubscribeUDPEvents();
        public void UnsubscribeUDPEvents();
    }
}
