using System.Collections;
using System.Collections.Generic;
using ThreadTest;
using UnityEngine;

public class ThreadDebugLog
{
    public static void Log(string content)
    {
        if (Loom.Current != null)
        {
            Loom.QueueOnMainThread((param) =>
          {
              Debug.Log(content);
          }, null);
        }
    }
}
