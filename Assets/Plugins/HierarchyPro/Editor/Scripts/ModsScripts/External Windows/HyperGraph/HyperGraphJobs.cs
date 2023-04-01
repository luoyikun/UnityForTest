using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;



namespace EMX.HierarchyPlugin.Editor.Mods.HyperGraph
{
	partial class HyperGraphModInstance 
    {


        static Type serType = typeof(SerializeField);

        static bool IsFieldReturnObject(FieldInfo f)
        {
            bool first = false;

            if ((f.Attributes & FieldAttributes.NotSerialized) != 0) return false;

            if (f.IsPublic) first = true;

            if (!first)
            {
                if (f.GetCustomAttributes(serType, false).Length != 0)
                {
                    first = true;
                }
            }

            if (first && compType.IsAssignableFrom(f.FieldType)) return true;

            return false;
        }


        // static Type ser_type = typeof(SerializeField);


        //static readonly Dictionary<Type, FieldInfo[]> cache_fields = new Dictionary<Type, FieldInfo[]>();
        // readonly List<FieldInfo> fieldsList = new List<FieldInfo>();
        // const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        internal System.Object LOCKER = new System.Object();

        Vector3 zero = new Vector3(0, 0, 0);

        internal class FieldsAccessor
        {
            HyperGraphModInstance hyperGraph;

            internal FieldsAccessor(HyperGraphModInstance hyperGraph, Type c)
            {
                this.hyperGraph = hyperGraph;
                this.type = c;
            }
            /*	public FieldsAccessor(  )
            {
            // reference_comp = c;
            }*/

            // internal Component reference_comp;
            internal List<ObjectDisplay> snannig_callback = new List<ObjectDisplay>();

            internal Type type;
            bool m_completed;

            internal bool completed_thread
            {
                get
                {
                    if (!m_completed && hyperGraph.current_job.stopped == false) hyperGraph.current_job.StarTAllJobs();

                    return m_completed;
                }

                set { m_completed = value; }
            }

            //internal FieldInfo[] f;
            internal Tools.FieldAdapter[] faList;

            internal bool InitializeTarget_InMainThread(UnityEngine.Object reference_comp)
            {
                if (faList.Length == 0 || !reference_comp) return true;

                for (int fIndex = 0; fIndex < faList.Length; fIndex++) // hyperGraph.selectObject_height += hyperGraph.SIZES.SPACE() + hyperGraph.SIZES.VAR();
                {
                    hyperGraph.SelectObject_height += hyperGraph.SIZE.SPACE() + hyperGraph.SIZE.VAR();

                    Dictionary<string, object> values = null;

                    try
                    {
                        values = faList[fIndex].GetAllValues(reference_comp, 0, hyperGraph.adapter.par_e.HYPERGRAPH_EVENTS_MODE | hyperGraph.adapter.par_e.HYPERGRAPH_SKIP_ARRAYS);

                        if (faList[fIndex].GetAllValuesCache != null)
                        {
                            faList[fIndex].CheckID(selection_id, SEL_INC);

                            if (!faList[fIndex].GetAllValuesCache.ContainsKey(reference_comp.GetInstanceID())) faList[fIndex].GetAllValuesCache.Add(reference_comp.GetInstanceID(), values);
                            else faList[fIndex].GetAllValuesCache[reference_comp.GetInstanceID()] = values;
                        }
                    }

                    catch
                    {
                        hyperGraph.CHANGEPLAYMODE();
                        return false;
                    }


                    if (values == null) continue;

                    // fieldsCount++;

                    foreach (var _value in values)
                    {
                        var value = _value.Value as UnityEngine.Object;

                        if (!value) continue;

                        Component comp = value as Component;
                        GameObject go = comp != null ? comp.gameObject : value as GameObject;

                        if (go && !hyperGraph.TARGET_COMPS.ContainsKey(go.GetInstanceID())) //GO
                        {
                            var r = new ObjectDisplay(go.GetInstanceID(), hyperGraph);
                            hyperGraph.TARGET_COMPS.Add(go.GetInstanceID(), r);

                            if (go != hyperGraph.CurrentSelection)
                            {
                                hyperGraph.TARGET_HEIGHT += hyperGraph.SIZE.OBJECT() + hyperGraph.SIZE.padding_y();
                                r.height += hyperGraph.SIZE.OBJECT() + hyperGraph.SIZE.padding_y();
                            }

                            r.DRAW_A_POSES.Add(hyperGraph.zero);
                        }

                        if (comp && !hyperGraph.TARGET_COMPS[go.GetInstanceID()].objecComps.ContainsKey(comp.GetInstanceID())) //COMP
                        {
                            var r = hyperGraph.TARGET_COMPS[go.GetInstanceID()];
                            // TARGET_COMPS.Add(comp.GetInstanceID(), r);
                            r.objecComps.Add(comp.GetInstanceID(), r.objecComps.Count);

                            if (go != hyperGraph.CurrentSelection)
                            {
                                hyperGraph.TARGET_HEIGHT += hyperGraph.SIZE.COMP();
                                r.height += hyperGraph.SIZE.COMP();
                            }

                            r.DRAW_A_POSES.Add(hyperGraph.zero);
                        }

                        if (!go && !comp && !hyperGraph.TARGET_COMPS.ContainsKey(value.GetInstanceID())) //ASSET
                        {
                            var r = new ObjectDisplay(value.GetInstanceID(), hyperGraph);
                            hyperGraph.TARGET_COMPS.Add(value.GetInstanceID(), r);

                            if (value != hyperGraph.CurrentSelection)
                            {
                                hyperGraph.TARGET_HEIGHT += hyperGraph.SIZE.OBJECT() + hyperGraph.SIZE.padding_y();
                                r.height += hyperGraph.SIZE.OBJECT() + hyperGraph.SIZE.padding_y();
                            }

                            r.DRAW_A_POSES.Add(hyperGraph.zero);
                        }
                    }

                    /*// OLD SINLE VALUE
                    UnityEngine.Object value = null;
                    try
                    {   value = f[fIndex].GetValue( reference_comp ) as UnityEngine.Object;
                    }
                    catch
                    {   hyperGraph.CHANGEPLAYMODE();
                        return false;
                    }
                    if (value == null) continue;
                    // fieldsCount++;
                    Component comp = value as Component;
                    GameObject go = comp != null ? comp.gameObject : value as GameObject;

                    if (go && !hyperGraph.TARGET_COMPS.ContainsKey( go.GetInstanceID() )) //GO
                    {   var r = new ObjectDisplay(go.GetInstanceID(), hyperGraph);
                        hyperGraph.TARGET_COMPS.Add( go.GetInstanceID(), r );

                        if (go != hyperGraph.CurrentSelection)
                        {   hyperGraph.TARGET_HEIGHT += hyperGraph.SIZE.OBJECT() + hyperGraph.SIZE.padding_y();
                            r.height += hyperGraph.SIZE.OBJECT() + hyperGraph.SIZE.padding_y();
                        }

                        r.DRAW_A_POSES.Add( hyperGraph.zero );
                    }
                    if (comp && !hyperGraph.TARGET_COMPS[go.GetInstanceID()].objecComps.ContainsKey( comp.GetInstanceID() )) //COMP
                    {   var r = hyperGraph.TARGET_COMPS[go.GetInstanceID()];
                        // TARGET_COMPS.Add(comp.GetInstanceID(), r);
                        r.objecComps.Add( comp.GetInstanceID(), r.objecComps.Count );
                        if (go != hyperGraph.CurrentSelection)
                        {   hyperGraph.TARGET_HEIGHT += hyperGraph.SIZE.COMP();
                            r.height += hyperGraph.SIZE.COMP();
                        }
                        r.DRAW_A_POSES.Add( hyperGraph.zero );
                    }
                    if (!go && !comp && !hyperGraph.TARGET_COMPS.ContainsKey( value.GetInstanceID() )) //ASSET
                    {   var r = new ObjectDisplay(value.GetInstanceID(), hyperGraph);
                        hyperGraph.TARGET_COMPS.Add( value.GetInstanceID(), r );

                        if (value != hyperGraph.CurrentSelection)
                        {   hyperGraph.TARGET_HEIGHT += hyperGraph.SIZE.OBJECT() + hyperGraph.SIZE.padding_y();
                            r.height += hyperGraph.SIZE.OBJECT() + hyperGraph.SIZE.padding_y();
                        }

                        r.DRAW_A_POSES.Add( hyperGraph.zero );
                    }
                    */ // OLD SINLE VALUE
                }

                return true;
                // objectDisplay.Value.DRAW_A_POSES = new Vector2[objectDisplay.Value.comps.Count + 1];
            }


            //internal bool Was_InitializeBroadcasting_InMainThread;
            /* internal void InitializeBroadcasting_InMainThread(Component reference_comp)
             {
                 if (!reference_comp) return;
                 if (f.Length == 0) return;

                 // resilt.ComponentToBPosIndex.Add(currentComps[i], fieldsCount);
                 var needAdd = false;
                 var fds = new Dictionary<string, int>();
                 for (int fIndex = 0; fIndex < f.Length; fIndex++)
                 {
                     bool haveChange = false;
                     if (f[fIndex].FieldType == GameObjectType)
                     {
                         if ((GameObject)f[fIndex].GetValue(reference_comp) == CurrentSelection)
                         {
                             findedList.Add(reference_comp,
                                 new FIELD_PARAMS(f[fIndex], CurrentSelection, null, fdsHotControl++));
                             // activeComps.Add(currentComps[i], i);
                             haveChange = true;
                         }
                     } else
                     {
                         var getComp = (Component)f[fIndex].GetValue(reference_comp);
                         if (getComp && compsSorted.ContainsKey(getComp.GetInstanceID()))
                         {
                             findedList.Add(reference_comp,
                                 new FIELD_PARAMS(f[fIndex], null, getComp, fdsHotControl++));
                             // activeComps.Add(currentComps[i], i);
                             haveChange = true;
                         }
                     }
                     if (haveChange)
                     {
                         // fieldsCount++;
                         height += SIZES.VAR;
                         fds.Add(f[fIndex].Name, fdsIndex++);
                         // result.fields.Add(f[fIndex].Name, result.fields.Count);
                         needAdd = true;
                     }
                 }

                 if (needAdd)
                 {
                     result.AllFields.Add(currentComps[i], fds);
                     height += SIZES.COMP;
                 }
             }
            */



        }
        // [HostProtectionAttribute(SecurityAction.LinkDemand, Synchronization = true)]

        private FieldsAccessor GetReflectionFields(UnityEngine.Object comp, ObjectDisplay callbackobject = null)
        {
            if (FiledsInfo == null)
                FiledsInfo = new Dictionary<Type, FieldsAccessor>()
                {
                    {typeof(Transform), new FieldsAccessor(this, typeof(Transform)) {completed_thread = true, faList = new Tools.FieldAdapter[0]}},
                    {typeof(CanvasRenderer), new FieldsAccessor(this, typeof(CanvasRenderer)) {completed_thread = true, faList = new Tools.FieldAdapter[0]}}
                };

            // var n = comp.GetType().Name;
            // var n = GetTypeFullName(comp);
            var n = comp.GetType();

            if (!FiledsInfo.ContainsKey(n)) // var result = new FieldsAccessor(comp.GetType());
            {
                var result = new FieldsAccessor(this, (comp).GetType());

                if (callbackobject != null) result.snannig_callback.Add(callbackobject);

                /*  lock ( scanQueue )
                  {   scanQueue.Enqueue( result );

                  }*/
                current_job.Push(result);
                FiledsInfo.Add(n, result);

                /*  if ( currentJob == null )
                      StarTJob();*/
                //if ( current_job.stopped) current_job.StarTJob();
            }

            else
            {
                if (callbackobject != null)
                {
                    lock (FiledsInfo[n])
                    {
                        if (!FiledsInfo[n].completed_thread) FiledsInfo[n].snannig_callback.Add(callbackobject);
                    }
                }
            }


            return FiledsInfo[n];
        }

        void mh_Finish()
        {
            RepaintNow();
           // Repaint(WindowHyperController);
        }

        void mh_MainThreadInvoker() { }

        JobFactory _current_job;

        JobFactory current_job
        {
            get
            {
                if (_current_job == null)
                {
                    _current_job = new JobFactory();
                    _current_job.Target = this;
                }

                return _current_job;
            }
        }

        class JobFactory
        {

            const int JOBCOUNT = 4;

            JobInstance[] _currentJob = Enumerable.Repeat(new JobInstance(), JOBCOUNT).ToArray();

            JobInstance[] currentJob
            {
                get { return _currentJob; }
            }

            int currentJobIndex = 0;
            internal HyperGraphModInstance Target;

            //internal bool stopped = true;
            /* internal StarTJob()
             {

             }*/

            internal void Push(FieldsAccessor result)
            {
                lock (currentJob[currentJobIndex].scanQueue)
                {
                    currentJob[currentJobIndex].scanQueue.Enqueue(result);
                    /* if ("MoodBoxManager" == result.type.Name)
                         MonoBehaviour.print(result.type);*/
                }

                StarTJob();
            }

            internal void StarTAllJobs()
            {
                for (int i = 0; i < JOBCOUNT; i++)
                {
                    StarTJob();
                }
            }

            internal bool stopped
            {
                get
                {
                    for (int i = 0; i < JOBCOUNT; i++)
                    {
                        if (_currentJob[0].wasInit) return false;
                    }

                    return true;
                }
            }

            // static double? time__;
            // static System.Diagnostics.Stopwatch DIAG_WATCH_CLONE = null;


            void StarTJob()
            {
                /*  if ( DIAG_WATCH_CLONE == null )
                  {   DIAG_WATCH_CLONE = System.Diagnostics.Stopwatch.StartNew();
                      DIAG_WATCH_CLONE.Stop();
                      DIAG_WATCH_CLONE.Reset();
                      DIAG_WATCH_CLONE.Start();
                  }*/


                currentJobIndex++;

                if (currentJobIndex >= currentJob.Length) currentJobIndex = 0;

                if (!currentJob[currentJobIndex].wasInit)
                {
                    Job tempJob = null;

                    currentJob[currentJobIndex].wasInit = true;

                    var inst = currentJob[currentJobIndex];
                    tempJob = MultiThread.CreateJob(() =>
                    {
                        if (tempJob.RequestStop) return;

                        inst.mh_Action();
                    }, Target.mh_Finish, Target.mh_MainThreadInvoker);

                    inst.job = tempJob;
                    inst.index = currentJobIndex;
                    inst.Target = Target;


                    tempJob.Start();
                }
            }

            class JobInstance
            {
                internal bool wasInit;
                internal Job job;
                internal int index;
                internal FieldsAccessor mh_tempAccessor;
                internal HyperGraphModInstance Target;
                internal Queue<FieldsAccessor> scanQueue = new Queue<FieldsAccessor>();
                Type mh_tempType;


                int scanQueueCount
                {
                    get
                    {
                        var res = 0;

                        lock (scanQueue)
                        {
                            res = scanQueue.Count;
                        }

                        return res;
                    }
                }

                internal void mh_Action()
                {
                    try
                    {
                        while (scanQueueCount != 0)
                        {
                            if (job.RequestStop) // MonoBehaviour.print("ASD");
                            {
                                return;
                            }

                            //  private static BindingFlags flags = ~BindingFlags.Static & ~BindingFlags.FlattenHierarchy;

                            lock (scanQueue) mh_tempAccessor = scanQueue.Dequeue();

                            /*  if ("MoodBoxManager" == mh_tempAccessor.type.Name)
                                  MonoBehaviour.print(mh_tempAccessor.type); */
                            //   BindingFlags.
                            lock (mh_tempAccessor)
                            {
                                mh_tempType = mh_tempAccessor.type;
                            }

                            var t = mh_tempType;


                            /*// OLD FIELDS
                            fieldsList.Clear();
                            while (t != null)
                            {

                                if (!cache_fields.ContainsKey( t ))
                                {   cache_fields.Add( t, t.GetFields( flags ).Where( f =>
                                    {   if (IsFieldReturnObject( f )) return true;

                                        // if (!f.FieldType.IsSerializable) return false;

                                        //  var ser = FindSerializableInSerializable(f);
                                        return false;
                                    } ).ToArray() );
                                }



                                fieldsList.AddRange( cache_fields[t] );

                                t = t.BaseType;
                            }
                            var result = fieldsList.ToArray();
                            */ // OLD FIELDS

                            /// NEW FIELDS
                            // fieldsList.Clear();
                            var result = Tools.GET_FIELDS(t, !Target.adapter.par_e.HYPERGRAPH_SKIP_ARRAYS_BOOL ).Values.ToArray();
                            /*foreach ( var f in typeFields ) {
                                var allValues = f.Value.GetAllValues();

                            }*/
                            /// NEW FIELDS


                            //  MonoBehaviour.print(mh_tempType.Name + " " + fields.Length);

                            if (job.RequestStop) // currentJob = null;
                            { //  MonoBehaviour.print("ASD");
                                return;
                            }

                            lock (mh_tempAccessor)
                            {
                                lock (Target.SCANNING_COMPS)
                                {
                                    foreach (var objectDisplay in mh_tempAccessor.snannig_callback)
                                    {
                                        Target.SCANNING_COMPS.Add(objectDisplay);
                                    }

                                    mh_tempAccessor.faList = result;
                                    mh_tempAccessor.completed_thread = true;
                                }
                            }
                        }

                        /*lock ( currentJob )
                            currentJob[index] = null;*/
                    }

                    catch (Exception ex)
                    {
                        LogProxy.LogError("HyperThread " + ex.Message + " " + ex.StackTrace);
                    }

                    lock (this) wasInit = false;

                    /* if ( Target.current_job.stopped )
                     {   DIAG_WATCH_CLONE.Stop();
                         var res = DIAG_WATCH_CLONE.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency;
                         DIAG_WATCH_CLONE = null;
                         Debug.Log( res );
                     }*/
                }
            }


        }

    }
}
