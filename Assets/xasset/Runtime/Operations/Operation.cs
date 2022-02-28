using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace xasset
{
    public enum OperationStatus
    {
        Idle,
        Processing,
        Success,
        Failed
    }

    public class Operation : IEnumerator
    {
        private static readonly List<Operation> Processing = new List<Operation>();
        public Action<Operation> completed;
        public OperationStatus status { get; protected set; } = OperationStatus.Idle;
        public float progress { get; protected set; }
        public bool isDone => status == OperationStatus.Failed || status == OperationStatus.Success;
        public string error { get; protected set; }

        public Task<Operation> Task
        {
            get
            {
                var tcs = new TaskCompletionSource<Operation>();
                completed += operation => { tcs.SetResult(this); };
                return tcs.Task;
            }
        }

        public bool MoveNext()
        {
            return !isDone;
        }

        public void Reset()
        {
        }

        public object Current => null;

        private static void Process(Operation operation)
        {
            Processing.Add(operation);
        }

        protected virtual void Update()
        {
        }

        public virtual void Start()
        {
            status = OperationStatus.Processing;
            Process(this);
        }

        public void Cancel()
        {
            Finish("User Cancel.");
        }

        protected void Finish(string errorCode = null)
        {
            error = errorCode;
            status = string.IsNullOrEmpty(error) ? OperationStatus.Success : OperationStatus.Failed;
            progress = 1;
        }

        private void Complete()
        {
            if (completed == null)
            {
                return;
            }

            var saved = completed;
            completed.Invoke(this);
            completed -= saved;
        }

        public static void UpdateAll()
        {
            for (var index = 0; index < Processing.Count; index++)
            {
                var item = Processing[index];
                if (Updater.busy)
                {
                    return;
                }

                item.Update();
                if (!item.isDone)
                {
                    continue;
                }

                Processing.RemoveAt(index);
                index--;
                if (item.status == OperationStatus.Failed)
                {
                    Logger.W("Unable to complete {0} with error: {1}", item.GetType().Name, item.error);
                }

                item.Complete();
            }

            InstantiateObject.UpdateObjects();
        }

        public static void ClearAll()
        {
            Processing.Clear();
            InstantiateObject.ClearObjects();
        }
    }
}