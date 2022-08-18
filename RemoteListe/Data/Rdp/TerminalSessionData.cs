namespace RemoteListe
{
    public record class TerminalSessionData
    {
        public int SessionId { get; init; }
        public WtsConnectionState ConnectionState { get; init; }
        public string StationName { get; init; }

        public TerminalSessionData(int sessionId, WtsConnectionState connState, string stationName)
        {
            SessionId = sessionId;
            ConnectionState = connState;
            StationName = stationName;
        }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", SessionId, ConnectionState, StationName);
        }
    }
}
