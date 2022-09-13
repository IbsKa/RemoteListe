using Serilog;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.DirectoryServices;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace RemoteListe
{
    public interface IActiveDirectoryService
    {
        public SortedDictionary<string, RdpHost> Hosts { get; }
        public void EnableRefresh();
        public void DisableRefresh();
    }

    public class ActiveDirectoryService : BackgroundService
    {
        private bool _refreshEnabled = true;
        private bool _isRefreshing = false;

        private static Dictionary<string, RdpHost> _hosts { get; set; } = new();

        public Dictionary<string, RdpHost> Hosts => _hosts;

        public static List<string> QueryActiveDirectory()
        {
            var hostNames = new List<string>();
            
            var entry = new DirectoryEntry("LDAP://172.16.10.8/OU=Remote,OU=Rechner,DC=ibs-ka,DC=local");
            var searcher = new DirectorySearcher(entry)
            {
                Filter = "(objectClass=computer)",
                SizeLimit = int.MaxValue,
                PageSize = int.MaxValue

            };

            foreach (SearchResult resEnt in searcher.FindAll())
            {
                string hostName = resEnt.GetDirectoryEntry().Name;
                if (hostName.StartsWith("CN="))
                    hostName = hostName.Remove(0, "CN=".Length);
                hostNames.Add(hostName);
            }

            searcher.Dispose();
            entry.Dispose();

            return hostNames;
        }

        void EnableRefresh() => _refreshEnabled = true;
        void DisableRefresh() => _refreshEnabled = false;

        private readonly TimeSpan _period = TimeSpan.FromMinutes(5);

        public event Action OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        private readonly Regex SortRegex = new("([a-zA-Z\\-_]+)(\\d+)");

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(_period);
            
            do
            {
                try
                {
                    if (_refreshEnabled && !_isRefreshing)
                    {
                        _isRefreshing = true;

                        Log.Information("querying active directory");

                        var hostNames = QueryActiveDirectory();

                        Log.Information("active directory hostnames: " + string.Join(',', hostNames));

                        var toRemove = _hosts.Keys.Where(h => !hostNames.Contains(h));
                        foreach (var r in toRemove)
                        {
                            Log.Information("removing from hosts: " + r);
                            _hosts.Remove(r);
                        }

                        foreach (var hostName in hostNames)
                        {
                            var match = SortRegex.Match(hostName);
                            var sort = match.Success ? (match.Groups[1].Value + match.Groups[2].Value.PadLeft(10, '0')) : ("ZZZ" + hostName);

                            if (!_hosts.TryGetValue(hostName, out var rdpHost))
                            {
                                rdpHost = new RdpHost(hostName);
                                _hosts.Add(hostName, rdpHost);
                            }
                            try
                            {
                                Log.Information("querying " + hostName);
                                await rdpHost.Query();
                            } catch
                            {
                                Log.Warning("could not refresh host " + hostName);
                                continue;
                            }
                            
                        }
                    }
                    else
                    {
                        Log.Information(
                            "Skipped PeriodicHostedService");
                    }
                }
                catch (Exception ex)
                {
                    Log.Information(
                        $"Failed to execute PeriodicHostedService with exception message {ex.Message}. Good luck next round!");
                }
                finally
                {
                    Log.Information("refresh completed");
                    _hosts = _hosts.OrderBy(h => {
                        var match = SortRegex.Match(h.Key);
                        return match.Success ? (match.Groups[1].Value + match.Groups[2].Value.PadLeft(10, '0')) : ("ZZZ" + h.Key);
                        }).ToDictionary(x => x.Key, x => x.Value);
                    NotifyStateChanged();
                    _isRefreshing = false;
                }
            } while (
                !stoppingToken.IsCancellationRequested &&
                await timer.WaitForNextTickAsync(stoppingToken));
        }

    }
}
