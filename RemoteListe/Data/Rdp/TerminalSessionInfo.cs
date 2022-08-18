namespace RemoteListe
{

    public record class TerminalSessionInfo
    {
        public TerminalSessionInfo() { }
        public TerminalSessionInfo(
            string initialProgram,
            string applicationName,
            string workingDirectory,
            string oEMId,
            int sessionId,
            string userName,
            string winStationName,
            string domainName,
            WtsConnectionState connectState,
            int clientBuildNumber,
            string clientName,
            string clientDirectory,
            int clientProductId,
            int clientHardwareId,
            RdpHost.WtsClientAdress clientAddress,
            RdpHost.WtsClientDisplay clientDisplay,
            int clientProtocolType)
        {
            InitialProgram = initialProgram;
            ApplicationName = applicationName;
            WorkingDirectory = workingDirectory;
            OEMId = oEMId;
            SessionId = sessionId;
            UserName = userName;
            WinStationName = winStationName;
            DomainName = domainName;
            ConnectState = connectState;
            ClientBuildNumber = clientBuildNumber;
            ClientName = clientName;
            ClientDirectory = clientDirectory;
            ClientProductId = clientProductId;
            ClientHardwareId = clientHardwareId;
            ClientAddress = clientAddress;
            ClientDisplay = clientDisplay;
            ClientProtocolType = clientProtocolType;
        }

        public string? InitialProgram { get; set; }
        public string? ApplicationName { get; set; }
        public string? WorkingDirectory { get; set; }
        public string? OEMId { get; set; }
        public int SessionId { get; set; }
        public string? UserName { get; set; }
        public string? WinStationName { get; set; }
        public string? DomainName { get; set; }
        public WtsConnectionState ConnectState { get; set; }
        public int ClientBuildNumber { get; set; }
        public string? ClientName { get; set; }
        public string? ClientDirectory { get; set; }
        public int ClientProductId { get; set; }
        public int ClientHardwareId { get; set; }
        public RdpHost.WtsClientAdress ClientAddress { get; set; }
        public RdpHost.WtsClientDisplay ClientDisplay { get; set; }
        public int ClientProtocolType { get; set; }
    }
}
