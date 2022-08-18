using System.Runtime.InteropServices;
using System.Net.NetworkInformation;

namespace RemoteListe
{
    public class RdpHost
    {

        public string HostName { get; set; }
        public DateTime? RefreshTime { get; private set; }
        public bool IsConnected { get; private set; }
        public List<TerminalSessionData>? SessionData { get; private set; }
        public List<TerminalSessionInfo>? ActiveSessions { get; private set; }
        public bool IsRefreshing { get; set; }
        

        public RdpHost(string hostName)
        {
            HostName = hostName;
        }

        [DllImport("wtsapi32.dll")]
        static extern IntPtr WTSOpenServer([MarshalAs(UnmanagedType.LPStr)] string pServerName);

        [DllImport("wtsapi32.dll")]
        static extern void WTSCloseServer(IntPtr hServer);

        [DllImport("Wtsapi32.dll")]
        public static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, WtsInfo wtsInfoClass,
            out IntPtr ppBuffer, out uint pBytesReturned);

        [DllImport("wtsapi32.dll")]
        static extern int WTSEnumerateSessions(IntPtr hServer, [MarshalAs(UnmanagedType.U4)] int Reserved,
            [MarshalAs(UnmanagedType.U4)] int Version, ref IntPtr ppSessionInfo, [MarshalAs(UnmanagedType.U4)] ref int pCount);

        [DllImport("wtsapi32.dll")]
        static extern void WTSFreeMemory(IntPtr pMemory);

        [StructLayout(LayoutKind.Sequential)]
        private struct WtsSessionInfo
        {
            public int SessionID;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pWinStationName;
            public WtsConnectionState State;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WtsClientAdress
        {
            public uint AddressFamily;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] Address;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WtsClientDisplay
        {
            public uint HorizontalResolution;
            public uint VerticalResolution;
            public uint ColorDepth;
        }
        
        private static IntPtr OpenServer(string hostName)
        {
            IntPtr server = WTSOpenServer(hostName);
            return server;
        }
        
        private static void CloseServer(IntPtr ServerHandle)
        {
            WTSCloseServer(ServerHandle);
        }
        
        public async Task<bool> Query()
        {
            return await Task.Run(() =>
            {
                IsConnected = PingHost();
                RefreshTime = DateTime.Now;
                if (!IsConnected)
                {
                    SessionData = new List<TerminalSessionData>();
                    ActiveSessions = new List<TerminalSessionInfo>();
                    return true;
                }
                SessionData = ListSessions();
                ActiveSessions = GetActiveSessionInfos(SessionData);
                return true;
            });
            
        }

        public List<TerminalSessionData> ListSessions()
        {
            var tsd = new List<TerminalSessionData>();
            var server = OpenServer(HostName);

            try
            {
                var ppSessionInfo = IntPtr.Zero;
                
                var count = 0;
                var retval = WTSEnumerateSessions(server, 0, 1, ref ppSessionInfo, ref count);
                var dataSize = Marshal.SizeOf(typeof(WtsSessionInfo));

                long current = (long)ppSessionInfo;

                if (retval != 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        WtsSessionInfo si = (WtsSessionInfo)Marshal.PtrToStructure((IntPtr)current, typeof(WtsSessionInfo));
                        current += dataSize;

                        tsd.Add(new TerminalSessionData(si.SessionID, si.State, si.pWinStationName));
                    }

                    WTSFreeMemory(ppSessionInfo);
                }
            }
            finally
            {
                CloseServer(server);
            }

            return tsd;
        }

        public bool PingHost()
        {
            var ping = new Ping();   
            try
            {
                
                PingReply reply = ping.Send(HostName);
                return reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                return false;
            }
            finally
            {
                if (ping != null)
                {
                    ping.Dispose();
                }
            }
        }
        
        public List<TerminalSessionInfo> GetActiveSessionInfos(IEnumerable<TerminalSessionData> sessions)
        {
            // Get the list of active sessions
            var activeSessions = sessions.Where(s => s.ConnectionState == WtsConnectionState.Active).ToArray();
            if (activeSessions.Length == 0)
            {
                return new List<TerminalSessionInfo>();
            }

            try
            {
                var sessionInfos = activeSessions.Select(s => GetSessionInfo(HostName, s.SessionId)).ToList();
                return sessionInfos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<TerminalSessionInfo>();
            }


        }

        private static TerminalSessionInfo GetSessionInfo(string hostName, int SessionId)
        {
            var server = OpenServer(hostName);
            var buffer = IntPtr.Zero;
            var data = new TerminalSessionInfo();

            try
            {

                if (!WTSQuerySessionInformation(server, SessionId, WtsInfo.ApplicationName, out buffer, out uint bytesReturned)) return data;

                data.ApplicationName = Marshal.PtrToStringAnsi(buffer);


                if (!WTSQuerySessionInformation(server, SessionId,
                    WtsInfo.ClientAddress, out buffer, out bytesReturned)) return data;

                data.ClientAddress = (WtsClientAdress)Marshal.PtrToStructure(buffer, typeof(WtsClientAdress));


                if (!WTSQuerySessionInformation(server, SessionId,
                    WtsInfo.ClientBuildNumber, out buffer, out bytesReturned)) return data;

                data.ClientBuildNumber = Marshal.ReadInt32(buffer);


                if (!WTSQuerySessionInformation(server, SessionId, WtsInfo.ClientDirectory, out buffer, out bytesReturned)) return data;

                data.ClientDirectory = Marshal.PtrToStringAnsi(buffer);

                if (!WTSQuerySessionInformation(server, SessionId, WtsInfo.ClientDisplay, out buffer, out bytesReturned)) return data;
                
                data.ClientDisplay = (WtsClientDisplay)Marshal.PtrToStructure((IntPtr)buffer, typeof(WtsClientDisplay));


                if (!WTSQuerySessionInformation(server, SessionId,
                    WtsInfo.ClientHardwareId, out buffer, out bytesReturned)) return data;

                data.ClientHardwareId = Marshal.ReadInt32(buffer);

                WTSQuerySessionInformation(server, SessionId,
                    WtsInfo.ClientName, out buffer, out bytesReturned);
                data.ClientName = Marshal.PtrToStringAnsi(buffer);

                WTSQuerySessionInformation(server, SessionId,
                    WtsInfo.ClientProductId, out buffer, out bytesReturned);
                data.ClientProductId = Marshal.ReadInt16(buffer);

                WTSQuerySessionInformation(server, SessionId,
                    WtsInfo.ClientProtocolType, out buffer, out bytesReturned);
                data.ClientProtocolType = Marshal.ReadInt16(buffer);

                WTSQuerySessionInformation(server, SessionId,
                    WtsInfo.ConnectState, out buffer, out bytesReturned);
                data.ConnectState = (WtsConnectionState)Enum.ToObject(typeof(WtsConnectionState), Marshal.ReadInt32(buffer));

                WTSQuerySessionInformation(server, SessionId,
                    WtsInfo.DomainName, out buffer, out bytesReturned);
                data.DomainName = Marshal.PtrToStringAnsi(buffer);

                WTSQuerySessionInformation(server, SessionId,
                    WtsInfo.InitialProgram, out buffer, out bytesReturned);
                data.InitialProgram = Marshal.PtrToStringAnsi(buffer);

                WTSQuerySessionInformation(server, SessionId,
                    WtsInfo.OEMId, out buffer, out bytesReturned);
                data.OEMId = Marshal.PtrToStringAnsi(buffer);

                WTSQuerySessionInformation(server, SessionId,
                    WtsInfo.SessionId, out buffer, out bytesReturned);
                data.SessionId = Marshal.ReadInt32(buffer);

                WTSQuerySessionInformation(server, SessionId,
                    WtsInfo.UserName, out buffer, out bytesReturned);
                data.UserName = Marshal.PtrToStringAnsi(buffer);

                WTSQuerySessionInformation(server, SessionId,
                    WtsInfo.WinStationName, out buffer, out bytesReturned);
                data.WinStationName = Marshal.PtrToStringAnsi(buffer);

                WTSQuerySessionInformation(server, SessionId,
                    WtsInfo.WorkingDirectory, out buffer, out bytesReturned);
                data.WorkingDirectory = Marshal.PtrToStringAnsi(buffer);
            }
            finally
            {
                WTSFreeMemory(buffer);
                CloseServer(server);
            }

            return data;
        }

    }


}