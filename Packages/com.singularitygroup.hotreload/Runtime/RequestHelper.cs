using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SingularityGroup.HotReload.DTO;
using SingularityGroup.HotReload.Newtonsoft.Json;
using SingularityGroup.HotReload.RuntimeDependencies;
using UnityEngine;
using UnityEngine.Networking;

[assembly: InternalsVisibleTo("CodePatcherEditor")]
[assembly: InternalsVisibleTo("TestProject")]
[assembly: InternalsVisibleTo("SingularityGroup.HotReload.IntegrationTests")]
[assembly: InternalsVisibleTo("SingularityGroup.HotReload.EditorTests")]

namespace SingularityGroup.HotReload {
    class HttpResponse {
        public readonly HttpStatusCode statusCode;
        public readonly Exception exception;
        public readonly string responseText;

        public HttpResponse(HttpStatusCode statusCode, Exception exception, string responseText) {
            this.statusCode = statusCode;
            this.exception = exception;
            this.responseText = responseText;
        }
    }
    
    static class RequestHelper {
        internal const ushort port = 33242;
        const string defaultServerHost = "127.0.0.1";
        
        static PatchServerInfo serverInfo = new PatchServerInfo(defaultServerHost, null, null);
        public static PatchServerInfo ServerInfo => serverInfo;
        
        static string cachedUrl;
        static string url => cachedUrl ?? (cachedUrl = CreateUrl(serverInfo));
        
        static readonly HttpClient client = new HttpClient();
        
        /// <summary>
        /// Create url for a hostname and port
        /// </summary>
        internal static string CreateUrl(PatchServerInfo server) {
            return $"http://{server.hostName}:{server.port.ToString()}";
        }
        
        public static string customServerHost => serverInfo.hostName == defaultServerHost ? null : serverInfo.hostName;
        
        public static void SetServerInfo(PatchServerInfo info) {
            if (info != null) Log.Debug($"SetServerInfo to {CreateUrl(info)}");
            serverInfo = info;
            cachedUrl = null;
        }
        
        static string[] assemblySearchPaths;
        public static void ChangeAssemblySearchPaths(string[] paths) {
            assemblySearchPaths = paths;
        }

        internal static Task<UnityWebRequestAsyncOperation> SendRequestAsync(UnityWebRequest www) {
            var req = www.SendWebRequest();
            var tcs = new TaskCompletionSource<UnityWebRequestAsyncOperation>();
            req.completed += op => tcs.TrySetResult((UnityWebRequestAsyncOperation)op);
            return tcs.Task;
        }

        static bool pollPending;
        internal static string lastPatchId;
        internal static async void PollMethodPatches(Action<MethodPatchResponse> onResponseReceived) {
            if (pollPending) return;
        
            pollPending = true;
            var searchPaths = assemblySearchPaths ?? CodePatcher.I.GetAssemblySearchPaths();
            var body = SerializeRequestBody(new MethodPatchRequest(lastPatchId, searchPaths, TimeSpan.FromSeconds(20), Path.GetDirectoryName(Application.dataPath)));
            
            await ThreadUtility.SwitchToThreadPool();
            
            try {
                var result = await PostJson(url + "/patch", body, 30).ConfigureAwait(false);
                if(result.statusCode == HttpStatusCode.OK) {
                    var responses = JsonConvert.DeserializeObject<MethodPatchResponse[]>(result.responseText);
                    await ThreadUtility.SwitchToMainThread();
                    foreach(var response in responses) {
                        onResponseReceived(response);
                        lastPatchId = response.id;
                    }
                } else if(result.statusCode == HttpStatusCode.Unauthorized || result.statusCode == 0) {
                    // Server is not running or not authorized.
                    // We don't want to spam requests in that case.
                    await Task.Delay(5000);
                } else if(result.statusCode == HttpStatusCode.ServiceUnavailable) {
                    //Server shut down
                    await Task.Delay(5000);
                } else {
                    Log.Info("PollMethodPatches failed with code {0} {1} {2}", (int)result.statusCode, result.responseText, result.exception);
                }
            } finally {
                pollPending = false;
            }
        }
        
        public static async Task<FlushErrorsResponse> RequestFlushErrors(int timeoutSeconds = 30) {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            var resp = await PostJson(CreateUrl(serverInfo) + "/flush", "", timeoutSeconds, cts.Token);
            if (resp.statusCode == HttpStatusCode.OK) {
                try {
                    return JsonConvert.DeserializeObject<FlushErrorsResponse>(resp.responseText);
                } catch {
                    return null;
                }
            }
            return null;
        }
        
        internal static async Task<LoginStatusResponse> GetLoginStatus(int timeoutSeconds) {
            var tcs = new TaskCompletionSource<LoginStatusResponse>();
            LoginRequestUtility.RequestLoginStatus(url, timeoutSeconds, resp => tcs.TrySetResult(resp));
            return await tcs.Task;
        }
        
        internal static async Task<LoginStatusResponse> RequestLogin(string email, string password, int timeoutSeconds) {
            var tcs = new TaskCompletionSource<LoginStatusResponse>();
            LoginRequestUtility.RequestLogin(url, email, password, timeoutSeconds, resp => tcs.TrySetResult(resp));
            return await tcs.Task;
        }
        
        internal static async Task<LoginStatusResponse> RequestLogout(int timeoutSeconds = 10) {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            var resp = await PostJson(CreateUrl(serverInfo) + "/logout", "", timeoutSeconds, cts.Token);
            if (resp.statusCode == HttpStatusCode.OK) {
                try {
                    return JsonConvert.DeserializeObject<LoginStatusResponse>(resp.responseText);
                } catch (Exception ex) {
                    return LoginStatusResponse.FromRequestError($"Deserializing response failed with {ex.GetType().Name}: {ex.Message}");
                }
            } else {
                return LoginStatusResponse.FromRequestError(resp.responseText ?? "Request timeout");
            }
        }
        
        public static async Task KillServer() {
            await ThreadUtility.SwitchToThreadPool();
            await KillServerInternal().ConfigureAwait(false);
        }

        internal static async Task KillServerInternal() {
            try {
                using(await client.PostAsync(CreateUrl(serverInfo) + "/kill", new StringContent(Path.GetDirectoryName(UnityHelper.DataPath))).ConfigureAwait(false)) { }
            } catch {
                //ignored
            } 
        }

        public static async Task<bool> PingServer(PatchServerInfo info, int timeoutSeconds) {
            await ThreadUtility.SwitchToThreadPool();
            
            try  {
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
                try {
                    using(var resp = await client.GetAsync(CreateUrl(info) + "/ping", cts.Token).ConfigureAwait(false)) {
                        return resp.StatusCode == HttpStatusCode.OK;
                    }
                } catch(OperationCanceledException) {
                    return false;
                }
            } catch {
                return false;
            } 
        }
        
        public static Task RequestCompile() {
            var body = SerializeRequestBody(new CompileRequest(serverInfo.rootPath));
            return PostJson(url + "/compile", body, 10);
        }
        
        internal static async Task<bool> Post(string route, string json) {
            var resp = await PostJson(url + route, json, 10);
            return resp.statusCode == HttpStatusCode.OK;
        }

        /// <summary>
        /// Send current files state of this Unity project to the server.
        /// </summary>
        /// <param name="buildInfo">If a build would be made now, these values would be inside that build.</param>
        /// <remarks>
        /// Server needs to know the BuildInfo stuff to reply it to a connecting device.
        /// </remarks>
        public static async Task PostFilesState(BuildInfo buildInfo) {
            var body = buildInfo.ToJson();
            await PostJson(url + "/files", body, 5);
        }

        internal static async Task<MobileHandshakeResponse> RequestHandshake(PatchServerInfo info, string[] defineSymbols, string projectExclusionRegex) {
            await ThreadUtility.SwitchToThreadPool();
            
            var body = SerializeRequestBody(new MobileHandshakeRequest(defineSymbols, projectExclusionRegex));
            
            var requestUrl = CreateUrl(info) + "/handshake";
            Log.Debug($"RequestHandshake to {requestUrl}");
            var resp = await PostJson(requestUrl, body, 120).ConfigureAwait(false);
            if (resp.statusCode == HttpStatusCode.OK) {
                return JsonConvert.DeserializeObject<MobileHandshakeResponse>(resp.responseText);
            } else if(resp.statusCode == HttpStatusCode.ServiceUnavailable) {
                return new MobileHandshakeResponse(null, ServerHandshake.Result.WaitForCompiling.ToString());
            } else {
                return new MobileHandshakeResponse(null, resp.responseText);
            }
        }
        
        static string SerializeRequestBody<T>(T request) {
            return JsonConvert.SerializeObject(request);
        }
        
        static async Task<HttpResponse> PostJson(string uri, string json, int timeoutSeconds, CancellationToken token = default(CancellationToken)) {
            await ThreadUtility.SwitchToThreadPool();
            
            try {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                using(var resp = await client.PostAsync(uri, content, token).ConfigureAwait(false)) {
                    var str = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return new HttpResponse(resp.StatusCode, null, str);
                }
            } catch(Exception ex) {
                return new HttpResponse(0, ex, null);
            } 
        }
    }
}