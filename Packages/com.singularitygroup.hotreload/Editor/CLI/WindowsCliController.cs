using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SingularityGroup.HotReload.Editor.Cli {
    class WindowsCliController : ICliController {
        Process process;

        public string BinaryFileName => "CodePatcherCLI.exe";
        public string PlatformName => "win-x64";

        public Task Start(StartArgs args) {
            process = Process.Start(new ProcessStartInfo {
                FileName = Path.GetFullPath(Path.Combine(args.executableTargetDir, "CodePatcherCLI.exe")),
                Arguments = args.cliArguments,
            });
            return Task.CompletedTask;
        }

        public async Task Stop() {
            await RequestHelper.KillServer();
            try {
                process?.CloseMainWindow();
            } catch {
                //ignored
            }  
            process = null;
        }
    }
}