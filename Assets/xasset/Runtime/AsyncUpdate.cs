using System;
using System.Collections.Generic;
using UnityEngine;

namespace xasset
{
    /// <summary>
    ///     异步更新组件，可以让程序运行的更流畅
    /// </summary>
    public class AsyncUpdate
    {
        private static readonly List<AsyncUpdate> Unused = new List<AsyncUpdate>();
        private static readonly List<AsyncUpdate> Progressing = new List<AsyncUpdate>();
        private Action completed;
        private Func<bool> isDone;
        private bool running;
        private Action updated;

        private static AsyncUpdate CreateInstance()
        {
            if (Unused.Count <= 0)
            {
                return new AsyncUpdate();
            }

            var item = Unused[0];
            Unused.RemoveAt(0);
            return item;
        }

        public static void RunAsync(Action action, Action update = null, Func<bool> check = null)
        {
            var asyncUpdate = CreateInstance();
            asyncUpdate.isDone = check ?? (() => true);
            asyncUpdate.completed = action;
            asyncUpdate.updated = update;
            asyncUpdate.Run();
        }

        private bool Update()
        {
            if (!running)
            {
                return false;
            }

            updated?.Invoke();
            if (isDone == null || !isDone())
            {
                return true;
            }

            completed?.Invoke();
            return false;
        }

        private void Run()
        {
            if (running)
            {
                return;
            }

            Progressing.Add(this);
            running = true;
        }

        private void Stop()
        {
            completed = null;
            updated = null;
            isDone = null;
            running = false;
        }

        public void Cancel()
        {
            Stop();
        }

        public static void UpdateAll()
        {
            for (var index = 0; index < Progressing.Count; index++)
            {
                var item = Progressing[index];
                try
                {
                    if (item.Update())
                    {
                        continue;
                    }

                    Remove(item, ref index);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Remove(item, ref index);
                }

                if (Updater.busy)
                {
                    return;
                }
            }
        }

        public static void ClearAll()
        {
            Progressing.Clear();
            Unused.Clear();
        }

        private static void Remove(AsyncUpdate item, ref int index)
        {
            item.running = false;
            Progressing.RemoveAt(index);
            item.Stop();
            Unused.Add(item);
            index--;
        }
    }
}