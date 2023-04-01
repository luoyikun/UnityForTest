#if UNITY_ANDROID && !UNITY_EDITOR
#define MOBILE_ANDROID
#endif
#if UNITY_IOS && !UNITY_EDITOR
#define MOBILE_IOS
#endif
#if MOBILE_ANDROID || MOBILE_IOS
#define MOBILE
#endif

using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace SingularityGroup.HotReload {
    static class IpHelper {
        // get my local ip address
        public static string GetIpAddress() {
        #if MOBILE_ANDROID
            // might be mobile data ip (remote ip), so try to filter that
            var myIp = NetworkFinder.GetMyIpAddressAndroid();
            if (string.IsNullOrEmpty(myIp)) {
                return null;
            }

            if (IsLocalIp(FromString(myIp))) {
                return myIp;
            }
            return null;
        #else
            var ip = GetLocalIPv4(NetworkInterfaceType.Wireless80211);
            
            if (string.IsNullOrEmpty(ip)) {
                return GetLocalIPv4(NetworkInterfaceType.Ethernet);
            }
            return ip;
        #endif
        }
        
        private static string GetLocalIPv4(NetworkInterfaceType _type) {
            string output = "";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces()) {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up) {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses) {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork && IsLocalIp(ip.Address.MapToIPv4().GetAddressBytes())) {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
            return output;
        }

        // https://datatracker.ietf.org/doc/html/rfc1918#section-3
        static bool IsLocalIp(byte[] ipAddress) {
            return ipAddress[0] == 10
                || ipAddress[0] == 172
                && ipAddress[1] >= 16
                && ipAddress[1] <= 31
                || ipAddress[0] == 192
                && ipAddress[1] == 168;
        }

        [CanBeNull]
        public static byte[] FromString(string hostName) {
            if (string.IsNullOrEmpty(hostName) || hostName == "localhost") {
                return null;
            }

            var localIP = hostName;
            return localIP.Split('.')
                .Select(byte.Parse)
                .ToArray();
        }

        // assume you're connected to a wifi network
        public static bool UsesMyNetwork(this PatchServerInfo targetServer) {
            if (targetServer == null) {
                return false;
            }

            var myip = GetIpAddress();
            
            // is target computer on same network?
            return AreOnSameNetwork(myip, targetServer.hostName);
        }

        public static bool AreOnSameNetwork(string myHostName, string otherHostName) {
            var myIpAddress = FromString(myHostName);
            var targetServerIP = FromString(otherHostName);
            return AreOnSameNetwork(myIpAddress, targetServerIP);
        }

        static bool AreOnSameNetwork(byte[] ipA, byte[] ipB) {
            if (ipA == ipB) {
                // same device/computer
                return true;
            } else if (ipA == null || ipB == null) {
                // one of them is not a local ip
                return false;
            }
            return ipA[0] == ipB[0]
                   && ipA[1] == ipB[1]
                   && ipA[2] == ipB[2];
                   //Note: we assume a subnet mask of 255.255.255.0 here
        }
        
        /// Search my local wifi network, looking for an http server running on specified port.
        /// Returns the local ip address of the first one found, or null if none found.
        public static Task<string> FindServer(int port) {
            return Task.Run(() => {
                try {
                    return FindServerBlocking(port);
                } catch (Exception ex) {
                    // if you see exception logged here, make sure its not a bug
                    Log.Exception(ex);
                    return null;
                }
            });
        }

        // credit: chatgpt
        private static string FindServerBlocking(int port) {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    var localIP = ip.ToString();
                    var parts = localIP.Split('.');
                    parts[3] = "1";
                    var baseIP = string.Join(".", parts);

                    for (int i = 1; i <= 255; i++) {
                        var testIP = baseIP + i;
                        try {
                            var client = new TcpClient();
                            client.Connect(testIP, port);
                            return testIP;
                        } catch (SocketException) {
                            // Not a server
                        }
                    }
                }
            }
            return null;
        }
    }
}