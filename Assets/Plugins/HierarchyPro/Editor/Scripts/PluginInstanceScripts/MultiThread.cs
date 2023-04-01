#define USE_STACK

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Threading.Tasks;
using System.Threading;

namespace EMX.HierarchyPlugin.Editor
{
	internal class MultiThread
    {
        //static MultyThead _get;
        /*  internal static MultyThread get {
			  get { return _get ?? (_get = GameObject.FindObjectOfType<MultyThread>()); }
		  }
		  void Awake()
		  {
			  _get = this;


		  }*/

        private static readonly object AvgBuyPriceLocker = new object();
        static int? _INSTANCE_ID;
        internal static int INSTANCE_ID {
            get {
                lock ( AvgBuyPriceLocker )
                {
                    return _INSTANCE_ID ?? (_INSTANCE_ID = SessionState.GetInt( "EMX_MultiThread_ID", 0 )).Value;
                }
            }
            set {
                if ( value > 10000 ) value = 0;
                lock ( AvgBuyPriceLocker )
                {
                    SessionState.SetInt( "EMX_MultiThread_ID", (_INSTANCE_ID = value).Value );
                }
            }
        }


        internal static ParallelOptions po = new ParallelOptions();
        internal static CancellationTokenSource cts = new CancellationTokenSource();

        static MultiThread()
        {
            //return;
            //#pragma warning disable
            INSTANCE_ID++;
            StopJobs();
            po = new ParallelOptions();
            cts = new CancellationTokenSource();
            EditorApplication.update -= updater;
            EditorApplication.update += updater;
        }

        internal static void StopJobs()
        {
            if ( myJob != null )
                foreach ( var threadedJob in myJob )
                {
                    threadedJob.Abort();
                }
            myJob.Clear();
            if ( !cts.IsCancellationRequested ) cts.Cancel();
            Tools.OnThreadChange();
        }
        internal static bool isComp = false;
        static void updater()
        {
            if ( EditorApplication.isCompiling && isComp != EditorApplication.isCompiling )
            {
                isComp = EditorApplication.isCompiling;
                INSTANCE_ID++;
                StopJobs();
                Root.ClearCacheOnCompile();
                return;
            }
            if ( !EditorApplication.isCompiling ) isComp = false;

            if ( myJob.Count == 0 ) return;
            myJob.RemoveAll( j => j.Update() );
        }

        static List<ThreadedJob> myJob = new List<ThreadedJob>();

        static internal Job CreateJob( Action action, Action onFinished, Action invokeInLock )
        {
            var j = new Job() { action = action, onFinish = onFinished, invokeInLock = invokeInLock };
            myJob.Add( j );

            return j;
        }



        static internal void Remove( ThreadedJob j )
        {
            myJob.Remove( j );
        }
        static internal bool Contains( ThreadedJob j )
        {
            return myJob.Contains( j );
        }
    }


    internal class Job : ThreadedJob
    {
        internal Action action;
        internal Action onFinish;
        internal Action invokeInLock;

        protected override void ThreadFunction()
        {
            action();
        }
        protected override void OnFinished()
        {
            onFinish();
        }
        protected override void InvokeInLock( Action ac )
        {
            onFinish();
        }
    }



    internal class ThreadedJob
    {

        int ID;
        internal ThreadedJob()
        {
            ID = MultiThread.INSTANCE_ID;
        }

        internal bool RequestStop = false;
        private bool m_IsDone = false;
        private object m_Handle = new object();
        private System.Threading.Thread m_Thread = null;
        public bool IsDone {
            get {
                bool tmp;
                lock ( m_Handle )
                {
                    tmp = m_IsDone || ID != MultiThread.INSTANCE_ID;
                }
                return tmp;
            }
            set {
                lock ( m_Handle )
                {
                    m_IsDone = value;
                }
            }
        }

        protected virtual void InvokeInLock( Action ac )
        {
            lock ( m_Handle )
            {
                ac();
            }
        }

        public virtual void Start()
        {
            m_Thread = new System.Threading.Thread( Run );

            m_Thread.Start();
        }
        public virtual void Abort()
        {
            lock ( m_Handle )
            {
                RequestStop = true;
            }

        }
        public bool IsAlive()
        {
            return !IsDone && MultiThread.Contains( this );
        }

        protected virtual void ThreadFunction() { }

        protected virtual void OnFinished() { }

        public virtual bool Update()
        {
            if ( IsDone )
            {
                MultiThread.Remove( this );
                OnFinished();
                return true;
            }
            return false;
        }
        public IEnumerator WaitFor()
        {
            while ( !Update() )
            {
                yield return null;
            }
        }
        private void Run()
        {
            ThreadFunction();
            IsDone = true;
        }
    }
}


