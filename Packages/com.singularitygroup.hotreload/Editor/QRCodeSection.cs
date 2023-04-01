
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using SingularityGroup.HotReload.ZXing;
using SingularityGroup.HotReload.ZXing.QrCode;
using SingularityGroup.HotReload.ZXing.QrCode.Internal;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using SessionPrefs = SingularityGroup.HotReload.Editor.HotReloadPrefs.SessionPrefs;

namespace SingularityGroup.HotReload.Editor {
    internal class QRCodeSection {
        Texture2D qrCodeTexture;

        string payload;
        DateTime payloadCreatedAt;
        
        public const int size = 250;

        Texture2D GetTexture() {
            var previousPayload = payload;
            var currentPayload = GetPayload();
            if (currentPayload != previousPayload || !qrCodeTexture) {
                var writer = new BarcodeWriter {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new QrCodeEncodingOptions {
                        Height = size,
                        Width = size,
                        Margin = 0, // dont use margin >0, it's unreliable
                        CharacterSet = "UTF-8",
                        ErrorCorrection = ErrorCorrectionLevel.L,
                    },
                };
                if (!String.IsNullOrEmpty(currentPayload)) {
                    var pixels = writer.Write(currentPayload);
                    qrCodeTexture = new Texture2D(size, size, TextureFormat.RGBA32, false);
                    qrCodeTexture.SetPixels32(pixels);
                    qrCodeTexture.Apply();
                }
            }
            return qrCodeTexture;
        }

        string GetPayload() {
            // If the payload is old, commit hash might have changed, so clear it
            if (payload != null && (DateTime.UtcNow - payloadCreatedAt).TotalSeconds > 40) {
                ClearPayload();
            }
            if (payload != null) {
                return payload;
            }
        
            var serverInfo = CreatePayload();
            if (serverInfo == null) {
                createPayloadFailed++;
                return payload = "";
            }
            createPayloadFailed = 0;
            string applicationId = PostbuildModifyAndroidManifest.ApplicationIdentiferSlug;
            payload = serverInfo.ToUriString(applicationId);
            payloadCreatedAt = DateTime.UtcNow;
            // qrcodeLabel =  {payload}"; 
            return payload;
        }

        /// <summary>
        /// Clear the stored payload to allow retrying to create the payload
        /// </summary>
        private void ClearPayload() {
            payload = null;
        }

        static PatchServerInfo CreatePayload() {
            var ip = IpHelper.GetIpAddress();
            if(string.IsNullOrEmpty(ip)) return null;
            var shortCommitHash = GitUtil.GetShortCommitHashOrFallback();

            // Player does not need to make compile requests, so it doesn't need project path.
            // Omit it because the long path was making QR-Code dots small and harder to scan.
            return new PatchServerInfo(ip, shortCommitHash, null);
        }

        private int createPayloadFailed = 0;

        public void OnGUI_QrCodeOnly(float maxWidth) {
            // grow texture size as you expand the width of the window
            var height = Mathf.Clamp(maxWidth, 250f, 360f);
            var rect = GUILayoutUtility.GetRect(size, maxWidth, height, height, GUI.skin.box);
            if (rect.width > rect.height) {
                // make it a square, so that texture is rendered left-aligned
                rect.width = rect.height;
            }
            // white background
            GUI.DrawTexture(rect, EditorTextures.White, ScaleMode.StretchToFill);
            
            var texture = GetTexture();
            var backgroundRect = rect;
            var foregroundRect = backgroundRect;
            const float padding = 8f;
            foregroundRect.Set(backgroundRect.x + padding,
                backgroundRect.y + padding,
                backgroundRect.width - padding * 2,
                backgroundRect.height - padding * 2);
            GUI.DrawTexture(foregroundRect, texture, ScaleMode.ScaleToFit);
        }

        public void OnGUI() {
            if (string.IsNullOrEmpty(GetPayload())) {
                EditorGUILayout.LabelField("There was an issue with retrieving the local ip address of your pc");
                var label = "Retry";
                if (createPayloadFailed > 1) {
                    // change label so user knows that retry button actually did something
                    // (otherwise you click button and nothing changes)
                    label += $" (failed {createPayloadFailed} times)";
                }
                // button to retry to get payload (user can click this after turning on wifi for example)
                if (GUILayout.Button(label)) {
                    ClearPayload();
                }
                GUILayout.Space(12f);
                return;
            }

            SessionPrefs.FoldoutQrCode = EditorGUILayout.Foldout(SessionPrefs.FoldoutQrCode,
                "Scan with a phone to Hot Reload...",
                true, HotReloadWindowStyles.FoldoutStyle);
            if (SessionPrefs.FoldoutQrCode) {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField(
                    "Scan this QR-code with a phone to connect to Hot Reload on your pc.",
                    HotReloadWindowStyles.WrapStyle);
                EditorGUILayout.Space();
                OnGUI_QrCodeOnly(size * 1.5f);
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }

            /*
            GUILayout.Space(2f);
            EditorGUILayout.LabelField("OR launch your app and paste this url in the Hot Reload options popup:",
                HotReloadWindowStyles.WrapStyle);

            const string payloadFieldName = "payloadField";
            GUI.SetNextControlName(payloadFieldName);
            EditorGUILayout.SelectableLabel(payload, HotReloadWindowStyles.TextFieldWrapStyle,
                GUILayout.Height(EditorGUIUtility.singleLineHeight * 3.1f));
            var content = EditorGUIUtility.IconContent("Clipboard");
            content.text = " Copy";
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(content, GUILayout.Width(70f), GUILayout.MinHeight(EditorGUIUtility.singleLineHeight))) {
                // copy to clipboard
                EditorGUIUtility.systemCopyBuffer = payload;
                // focus the field for visual feedback, there is no Unity api to select the text.
                EditorGUI.FocusTextInControl(payloadFieldName);
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(2f);
            */
        }
    }

    internal static class GitUtil {
        /// <remarks>
        /// Fallback is PatchServerInfo.UnknownCommitHash
        /// </remarks>
        public static string GetShortCommitHashOrFallback(int timeoutAfterMillis = 5000) {
            var shortCommitHash = PatchServerInfo.UnknownCommitHash;
            
            var commitHash = GetShortCommitHashSafe(timeoutAfterMillis);
            // On MacOS GetShortCommitHash() returns 7 characters, on Windows it returns 8 characters.
            // When git command produced an unexpected result, use a fallback string
            if (commitHash != null && commitHash.Length >= 6) {
                shortCommitHash = commitHash.Length < 8 ? commitHash : commitHash.Substring(0, 8);
            }

            return shortCommitHash;
        }
        
        // only log exception once per domain reload, to prevent spamming the console
        private static bool loggedExceptionInGetShortCommitHashSafe = false;

        /// <summary>
        /// Get the git commit hash, returning null if it takes too long.
        /// </summary>
        /// <param name="timeoutAfterMillis"></param>
        /// <returns></returns>
        /// <remarks>
        /// This method is 'better safe than sorry' because we must not break the user's build.<br/>
        /// It is better to not know the commit hash than to fail the build.
        /// </remarks>
        private static string GetShortCommitHashSafe(int timeoutAfterMillis) {
            Process process = null;
            // Note: don't use ReadToEndAsync because waiting on that task blocks forever.
            try {
                process = StartGitCommand("log", " -n 1 --pretty=format:%h");
                var stdout = process.StandardOutput;
                if (process.WaitForExit(timeoutAfterMillis)) {
                    return stdout.ReadToEnd();
                } else {
                    // In a git repo with git lfs, git log can be blocked by waiting for switch branches / download lfs objects
                    // For that reason I disabled this warning log until a better solution is implemented (e.g. cache the commit and use cached if timeout).
                    // Log.Warning(
                    //     $"[{CodePatcher.TAG}] Timed out trying to get the git commit hash, HotReload will not warn you about" +
                    //     " a build connecting to a server running on a different commit (which is not supported)");
                    return null;
                }
            } catch (Win32Exception ex) {
                if (ex.NativeErrorCode == 2) {
                    // git not found, ignore because user doesn't use git for version control
                    return null;
                } else if (!loggedExceptionInGetShortCommitHashSafe) {
                    loggedExceptionInGetShortCommitHashSafe = true;
                    Debug.LogException(ex);
                } 
            } catch (Exception ex) {
                if (!loggedExceptionInGetShortCommitHashSafe) {
                    loggedExceptionInGetShortCommitHashSafe = true;
                    Log.Exception(ex);
                }
            } finally {
                if (process != null) {
                    process.Dispose();
                }
            }
            return null;
        }

        static Process StartGitCommand(string command, string arguments, Action<ProcessStartInfo> modifySettings = null) {
            var startInfo = new ProcessStartInfo("git", command + " " + arguments) {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            if (modifySettings != null) {
                modifySettings(startInfo);
            }
            return Process.Start(startInfo);
        }
    }
}