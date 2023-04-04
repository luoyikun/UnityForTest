using UnityEngine;
using System.Collections;

//时间管理器，用于保存服务器的时间
public class TimeManager {
	float realTime;
    public static TimeManager timeMgrSelf = null;
    public static TimeManager self
    {
        get
        {
            if (timeMgrSelf == null)
            {
                timeMgrSelf = new TimeManager();
            }
            return timeMgrSelf;
        }
    }
    public float currentTime
	{
		get { return realTime; }
		set { 
			initialized = true;
			realTime = value;
		}
	}

    bool initialized = false;
	// Use this for initialization
	public void Start () {

		realTime = Time.time;
	}
	
	// Update is called once per frame
	public void Update (float dt) {
        if (initialized) {
			realTime += dt;
		}
	}
}
