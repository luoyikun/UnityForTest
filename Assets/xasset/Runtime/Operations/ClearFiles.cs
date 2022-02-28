using System;
using System.Collections.Generic;
using System.IO;

namespace xasset
{
    /// <summary>
    ///     异步删除文件的操作
    /// </summary>
    public sealed class ClearFiles : Operation
    {
        public readonly List<string> files = new List<string>();
        public Action<ClearFiles, string> updated;

        public int max { get; private set; }
        public int id => max - files.Count;

        public override void Start()
        {
            base.Start();
            max = files.Count;
        }

        protected override void Update()
        {
            if (status != OperationStatus.Processing)
            {
                return;
            }

            while (files.Count > 0)
            {
                progress = id * 1f / max;
                var file = files[0];
                updated?.Invoke(this, file);
                if (File.Exists(file))
                {
                    File.Delete(file);
                }

                files.RemoveAt(0);
                if (Updater.busy)
                {
                    break;
                }
            }

            Finish();
        }
    }
}