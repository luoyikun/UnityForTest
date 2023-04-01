#if HAS_BEEN_TESTED
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SingularityGroup.HotReload {
    static class AutoPair {
        public static Task<bool> RunAutoPair() {
            var targetMachine = PlayerEntrypoint.buildInfo.BuildMachineServer;
            if (targetMachine == null) {
                NetworkFinder.FindNearbyTcpListeners(IpHelper.GetIpAddress(), RequestHelper.port);
                return Task.CompletedTask;
            } else {
                return TryConnect(targetMachine);
            }
        }

        private static DateTime lastRetry;


        public static async Task RunAutoPairAndDialog() {
            var connected = await RunAutoPair();
            if (connected) {
                Prompts.ShowConnectedPrompt();
            } else {
                Prompts.ShowRetryDialog(PlayerEntrypoint.buildInfo.BuildMachineServer);
            }
        }

        public static async Task TryConnect(PatchServerInfo serverInfo) {
            // try reach server
            PlayerCodePatcher.UpdateHost(serverInfo);
            await ThreadUtility.SwitchToMainThread();
            await TaskExtensions.WaitUntil(() => ServerHealthCheck.I.IsServerHealthy);

            if (!ServerHealthCheck.I.IsServerHealthy) {
                Log.Info("ShowRetryPrompt");
                Prompts.ShowRetryDialog(serverInfo);
                // cancel trying to connect. They can use the retry button
                PlayerCodePatcher.UpdateHost(null);
            }

            Log.Debug($"Connected to server within several seconds? {connected}");
        }
    }
}
#endif
