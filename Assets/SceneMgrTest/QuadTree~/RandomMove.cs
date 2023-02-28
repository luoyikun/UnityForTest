using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMove : MonoBehaviour
{
    float stopTime;
    float moveTime;
    float vel_x, vel_y, vel_z;//速度
    /// <summary>
    /// 最大、最小飞行界限
    /// </summary>
    public float maxPos_x = 500;
    public float maxPos_y = 300;
    public float minPos_x = -500;
    public float minPos_y = -300;
    int curr_frame;
    int total_frame;
    float timeCounter1;
    float timeCounter2;
    // int max_Flys = 128;
    // Use this for initialization
    void Start()
    {
        Change();

    }

    // Update is called once per frame
    void Update()
    {
        timeCounter1 += Time.deltaTime;
        if (timeCounter1 < moveTime)
        {
            transform.Translate(vel_x, vel_y, 0, Space.Self);
        }
        else
        {
            timeCounter2 += Time.deltaTime;
            if (timeCounter2 > stopTime)
            {
                Change();
                timeCounter1 = 0;
                timeCounter2 = 0;
            }
        }
        Check();
    }
    void Change()
    {
        stopTime = Random.Range(1, 5);
        moveTime = Random.Range(1, 20);
        vel_x = Random.Range(-10, 10) * 0.01f;
        vel_y = Random.Range(-10, 10) * 0.01f;
    }
    void Check()
    {
        //如果到达预设的界限位置值，调换速度方向并让它当前的坐标位置等于这个临界边的位置值
        if (transform.localPosition.x > maxPos_x)
        {
            vel_x = -vel_x;
            transform.localPosition = new Vector3(maxPos_x, transform.localPosition.y, 0);
        }
        if (transform.localPosition.x < minPos_x)
        {
            vel_x = -vel_x;
            transform.localPosition = new Vector3(minPos_x, transform.localPosition.y, 0);
        }
        if (transform.localPosition.y > maxPos_y)
        {
            vel_y = -vel_y;
            transform.localPosition = new Vector3(transform.localPosition.x, maxPos_y, 0);
        }
        if (transform.localPosition.y < minPos_y)
        {
            vel_y = -vel_y;
            transform.localPosition = new Vector3(transform.localPosition.x, minPos_y, 0);
        }
    }
}
