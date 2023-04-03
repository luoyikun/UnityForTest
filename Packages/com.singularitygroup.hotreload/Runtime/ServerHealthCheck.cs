using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SingularityGroup.HotReload {
    internal class ServerHealthCheck : IServerHealthCheck {
        private static readonly TimeSpan heartBeatTimeout = TimeSpan.FromMilliseconds(5001);
        public static readonly ServerHealthCheck I = new ServerHealthCheck();
        private readonly HttpClient http;

        private Uri healthCheckEndpoint = null;
        private Task healthCheck;
        private DateTime healthOkayAt;
        private HttpResponseMessage healthCheckResult;

        ServerHealthCheck() {
            http = new HttpClient();
            http.Timeout = heartBeatTimeout;
        }

        public void SetServerInfo(PatchServerInfo serverInfo) {
            if (serverInfo == null) {
                Log.Debug("ServerHealthCheck SetServerInfo to null");
                healthCheckEndpoint = null;
            } else {
                var url = RequestHelper.CreateUrl(serverInfo) + "/ping";
                Log.Debug("ServerHealthCheck SetServerInfo using url {0}", url);
                healthCheckEndpoint = new Uri(url);
            }
            WasServerResponding = false;
            IsServerHealthy = false;
        }

        public bool IsServerHealthy { get; private set; } = false;

        /// Is it confirmed the server has been running before? 
        public bool WasServerResponding { get; private set; } = false;

        // any thread
        public void CheckHealth() {
            if (healthCheckEndpoint == null) {
                return;
            }
            // update with latest health result
            if (healthCheckResult == null) {
                IsServerHealthy = false;
            } else {
                IsServerHealthy = healthCheckResult.IsSuccessStatusCode
                                  && DateTime.UtcNow - healthOkayAt < heartBeatTimeout;
                if (IsServerHealthy) {
                    WasServerResponding = true;
                }
            }

            // ensure a health check is running
            if (healthCheck == null || healthCheck.IsCompleted) {
                healthCheck = CheckHealthAsync();
            }
        }

        public async Task CheckHealthAsync() {
            healthCheckResult = await http.GetAsync(healthCheckEndpoint);
            if (healthCheckResult.IsSuccessStatusCode) {
                healthOkayAt = DateTime.UtcNow;
            } else {
                Log.Debug("ServerHealthCheck result code {0}", healthCheckResult.StatusCode);
            }
        }
    }
}
