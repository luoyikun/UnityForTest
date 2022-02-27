using System.Collections.Generic;
using UnityEngine;

namespace xasset.editor
{
    public abstract class BuildTaskJob
    {
        protected readonly BuildTask _task;

        protected readonly List<string> changes = new List<string>();
        public string error;

        protected BuildTaskJob(BuildTask task)
        {
            _task = task;
        }

        public abstract void Run();

        protected string GetBuildPath(string filename)
        {
            return _task.GetBuildPath(filename);
        }

        protected void TreatError(string e)
        {
            error = e;
            Debug.LogError(error);
        }
    }
}