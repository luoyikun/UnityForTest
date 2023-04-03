
using System.Threading.Tasks;

namespace SingularityGroup.HotReload.Editor.Cli {
    class FallbackCliController : ICliController {
        public string BinaryFileName => "";
        public string PlatformName => "";
        public Task Start(StartArgs args) => Task.CompletedTask;

        public Task Stop() => Task.CompletedTask;
    }
}