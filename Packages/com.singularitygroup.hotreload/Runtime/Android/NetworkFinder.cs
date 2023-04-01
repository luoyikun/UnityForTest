using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace SingularityGroup.HotReload {
    internal static class NetworkFinder {
        public static async Task FindNearbyTcpListeners(string myHostName, int checkPort) {
            if (myHostName == "localhost" || myHostName == "127.0.0.1") {
                // note: revisit this when we ping localhost for standalone builds SG-29499
                return;
            }

            await Task.Run(() => {
                AndroidJNI.AttachCurrentThread();
                try {
                    Android.findNearbyTcpListeners(myHostName, checkPort, new Android.Receiver(OnServerFound));
                } catch (Exception exc) {
                    Log.Exception(exc);
                    throw;
                } finally {
                    AndroidJNI.DetachCurrentThread();
                }
            });
        }

        public static bool TryDequeueUri(out Uri serverUri) {
            return foundServers.TryDequeue(out serverUri);
        }

        // for android because our Csharp in IpHelper was notworking on the phone I tested with.
        public static string GetMyIpAddressAndroid() {
            return Android.getMyIpAddress();
        }

        static readonly ConcurrentQueue<Uri> foundServers = new ConcurrentQueue<Uri>();

        // may be called any thread
        private static void OnServerFound(Uri serverUri) {
            foundServers.Enqueue(serverUri);
        }

        private static class Android {
            static readonly AndroidJavaClass jvmClass = new AndroidJavaClass("com.singularitygroup.networkfinder.NetworkFinder");

            /// <remarks>
            /// Assumes you are connected to a network (use unity Reachability to check)<br/>
            /// First checks the port has a TCP connection (fast), then tests the /ping endpoint of the server.<br/>
            /// Servers that pass both checks are passed along to <paramref name="onListenerFound"/>.
            /// </remarks>
            public static void findNearbyTcpListeners(string myHostName,
                int port,
                Receiver onListenerFound) {
                jvmClass.CallStatic("findNearbyTcpListeners", myHostName, port, onListenerFound);
            }

            public static string getMyIpAddress() {
                return jvmClass.CallStatic<string>("getMyIpAddress");
            }

            internal class Receiver : AndroidJavaProxy {
                private readonly Action<Uri> onServerFound;

                public Receiver(Action<Uri> onServerFound) : base(
                    "com.singularitygroup.networkfinder.NetworkFinder.ConsumerOfString") {
                    this.onServerFound = onServerFound;
                }

                [UsedImplicitly]
                void consume(String foundUriString) {
                    var uri = new UriBuilder(foundUriString).Uri;
                    onServerFound(uri);
                }
            }
        }
    }
}