using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChickenCircle : MonoBehaviour
{
    public Transform m_big;
    public Transform m_small;
    float m_bigRadius = 20;
    float m_smallRadius = 5;

    float m_frameClip = 0.02f; //帧运行间隔,单位s
    float m_curFrame = 0;
    float m_threshold = 0.05f; //阈值，当间隔小于此值时，视为长度相等
    float m_speed = 1;  //速度：每1s减少多少半径
    float m_lastCalcuTime = 0;
    float m_k;
    float smallX;
    float smallY;
    public MoveState m_moveState = MoveState.None;
    float centerSpeed = 0;
    Vector3 m_centerMoveDir = Vector3.zero;
    float m_time = 0;
    float disCenter = 0;
    bool isFirstIntersect = true;
    float radiusBigIntersect = 0;
    float cos = 0;
    float sin = 0;
    public enum MoveState
    {
        None,
        Intersect, //第一阶段先内切
        Small ,//第二阶段缩成小圆
    }
    // Start is called before the first frame update
    void Start()
    {
        float bigY = m_big.position.z;
        float bigX = m_big.position.x;
        smallX = m_small.position.x;
        smallY = m_small.position.z;

        float diffRadius = m_bigRadius - m_smallRadius;
        disCenter = Vector3.Distance(m_big.transform.position, m_small.transform.position);

        centerSpeed = diffRadius * m_speed / disCenter;
        m_centerMoveDir = (m_small.transform.position - m_big.transform.position).normalized;
        m_time = Time.time;

        //m_k = (m_small.position.z - m_big.position.z) / (m_small.position.x - m_big.position.x);
        //Debug.Log("夹角k:" + m_k);
        //float angle = Mathf.Atan(m_k);
        ////弧度转角度
        //Debug.Log("夹角:"+ angle / Mathf.PI * 180);

        //cos = Mathf.Cos(angle);
        //Debug.Log("夹角cos:" +cos);

        //sin = Mathf.Sin(angle);
        //Debug.Log("夹角sin:" + sin);
        CalcuSinCos();
    }

    void CalcuSinCos()
    {
        if ((m_small.position.x == m_big.position.x) && (m_small.position.z != m_big.position.z))
        {
            cos = 0;
            sin = 1;
        }
        else if ((m_small.position.x != m_big.position.x) && (m_small.position.z == m_big.position.z))
        {
            cos = 1;
            sin = 0;
        }
        else if ((m_small.position.x == m_big.position.x) && (m_small.position.z == m_big.position.z))
        {
            cos = 0;
            sin = 0;
        }
        else {
            //m_k = (m_small.position.z - m_big.position.z) / (m_small.position.x - m_big.position.x);
            //m_k = Mathf.Abs(m_k);
            //Debug.Log("夹角k:" + m_k);
            //float angle = Mathf.Atan(m_k);
            ////弧度转角度
            //Debug.Log("夹角:" + angle / Mathf.PI * 180);

            //cos = Mathf.Cos(angle);
            //Debug.Log("夹角cos:" + cos);

            //sin = Mathf.Sin(angle);
            //Debug.Log("夹角sin:" + sin);
            float a = m_small.position.x - m_big.position.x;
            float b = m_small.position.z - m_big.position.z;
            float c = Mathf.Sqrt(a * a + b * b);
            cos = a / c;
            sin = b / c;
        }
    }
    // Update is called once per frame
    void Update()
    {
        //每间隔时间只计算一次
        //if (m_lastCalcuTime == 0 || Time.time - m_lastCalcuTime >= m_frameClip )
        {
            //m_lastCalcuTime = Time.time;
            float diffBigRadius = m_speed * Time.deltaTime;
            m_bigRadius = m_bigRadius - diffBigRadius;
            
            float diffRadius = m_bigRadius-m_smallRadius;
            
            Debug.Log("当前半径差：" + diffRadius);
            if (m_bigRadius > m_smallRadius + disCenter)
            {

                m_moveState = MoveState.Intersect;
                Debug.Log("状态：" + m_moveState);
                m_big.transform.localScale = new Vector3(m_bigRadius * 2, 10, m_bigRadius * 2);

                ////大半径未变为小半径，需要接着运动
                //bool isHasIntersect = IsHasIntersect(m_big.transform.position, m_bigRadius, m_small.transform.position, m_smallRadius);
                //if (isHasIntersect == false)
                //{
                //    //还未内切，向内切运动，表现为大圆圆心不动，缩小半径
                //    m_moveState = MoveState.Intersect;
                //    Debug.Log("状态：" + m_moveState);
                //    m_big.transform.localScale = new Vector3(m_bigRadius * 2, 10, m_bigRadius * 2);
                //}
                //else
                //{
                //    //已经内切了
                //    m_moveState = MoveState.Small;
                //    Debug.Log("状态：" + m_moveState);

                //    Vector3 bigPos = m_big.transform.position;
                //    bigPos = bigPos + m_centerMoveDir * centerSpeed * Time.deltaTime;
                //    m_big.transform.position = bigPos;

                //    m_big.transform.localScale = new Vector3(m_bigRadius * 2, 10, m_bigRadius * 2);

                //}
            }
            else if (m_bigRadius > m_smallRadius && m_bigRadius <= m_smallRadius + disCenter)
            {
                if (isFirstIntersect == true)
                {
                    isFirstIntersect = false;

                    radiusBigIntersect = m_bigRadius;
                    Debug.Log(string.Format("距离{0}，内切时大圆半径{1}", disCenter, m_bigRadius));

                }
                //已经内切了
                m_moveState = MoveState.Small;
                Debug.Log("状态：" + m_moveState);

                float bigX = m_big.position.x;
                float bigZ = m_big.position.z;
                float smallX = m_small.position.x;
                float smallZ = m_small.position.z;

                //float k = (m_big.position.z - m_small.position.z) / (m_big.position.x - m_small.position.x);
                //float x_off = 1 * (float)Math.Sqrt(diffBigRadius * diffBigRadius / (k * k + 1));

                //// k<0  x+x_off



                //bigX += (bigX < smallX ? 1 : -1) * x_off;
                //bigZ = k * (bigX - smallX) + smallZ;


                //bigX += (bigX < smallX ? 1 : -1) * diffBigRadius * cos;
                //bigZ += (bigZ < smallZ ? 1 : -1) * diffBigRadius * sin;

                bigX +=  diffBigRadius * cos;
                bigZ +=  diffBigRadius * sin;

                m_big.position = new Vector3(bigX, 0, bigZ);
                m_big.transform.localScale = new Vector3(m_bigRadius * 2, 10, m_bigRadius * 2);
            }
            else
            {
                m_moveState = MoveState.None;
                Debug.Log("状态：" + m_moveState);
                float time = Time.time - m_time;
                Debug.Log("耗时{0}" + time);
            }

            
        }

        
    }

    void CreateSmall()
    {
        Vector3 smallCtr = PointOfRandom(m_big.position, m_bigRadius, m_smallRadius);
        m_small.position = smallCtr;
    }

    /// <summary>
    /// 在圆心为point，半径为radius的圆内，产生一个半径为radius_inner的圆的圆心
    /// </summary>
    /// <param name="point">外圆圆心</param>
    /// <param name="radius_outer">外圆半径</param>
    /// <param name="radius_inner">内圆半径</param>
    /// <returns>内圆圆心</returns>
    public Vector3 PointOfRandom(Vector3 point, float radius_outer, float radius_inner)
    {
        int x = Random.Range((int)(point.x - (radius_outer - radius_inner)), (int)(point.x + (radius_outer - radius_inner)) + 1);
        int z = Random.Range((int)(point.z - (radius_outer - radius_inner)), (int)(point.z + (radius_outer - radius_inner)) + 1);

        while (!isInRegion(x - point.x, z - point.z, radius_outer - radius_inner))
        {
            x = Random.Range((int)(point.x - (radius_outer - radius_inner)), (int)(point.x + (radius_outer - radius_inner)) + 1);
        }   z = Random.Range((int)(point.z - (radius_outer - radius_inner)), (int)(point.z + (radius_outer - radius_inner)) + 1);

        Vector3 p = new Vector3(x, 0,z);
        return p;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x_off">与大圆圆心的x方向偏移量</param>
    /// <param name="y_off">与大圆圆心的y方向偏移量</param>
    /// <param name="distance">大圆与小圆半径的差</param>
    /// <returns>判断点是否在范围内</returns>
    public bool isInRegion(float x_off, float y_off, float distance)
    {
        if (x_off * x_off + y_off * y_off <= distance * distance)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 判断两个圆是否重合，或者是相内切
    /// </summary>
    /// <param name="p_outer">外圆圆心</param>
    /// <param name="r_outer">外圆半径</param>
    /// <param name="p_inner">内圆圆心</param>
    /// <param name="r_inner">内圆半径</param>
    /// <returns>是否相内切</returns>
    public bool IsHasIntersect(Vector3 p_outer, float r_outer, Vector3 p_inner, float r_inner)
    {
        //判定条件：两圆心的距离 + r_inner = r_outer

        float distance = Vector3.Distance(p_outer, p_inner);
        Debug.Log("两个圆心间距：" + distance);
        if (distance + r_inner < r_outer)
        {
            return false;
        }
        return true;
    }

    public bool IsHasIntersect(float dis,float bigR,float smallR)
    {
        //判定条件：两圆心的距离 + r_inner = r_outer
        if (bigR > dis + smallR)
        {
            return false;
        }
        return true;
    }

}
