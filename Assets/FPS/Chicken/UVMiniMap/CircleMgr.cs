using Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMgr : MonoSingleton<CircleMgr>
{
    public Transform m_player;
    public GameObject m_bigMap;
    public Transform m_transCircle;
    public float posXMin = 560;
    public float posXMax = 1120;
    public float posYMin = 320;
    public float posYMax = 640;

    public float m_firstR = 300; //第一次大圆半径
    public float m_rDiff = 0.5f; //半径每次固定减少百分比
    bool m_isFirst = true;
    public CircleData m_circleData = new CircleData();
    public bool m_isMove = false;
    public Transform m_transCircleSmall;
    // Start is called before the first frame update
    void Start()
    {
        //生成第一次数据
        float x = Random.Range(posXMin, posXMax);
        float y = Random.Range(posYMin, posYMax);

        Vector2 bigPos = new Vector2(x, y);
        float bigR = m_firstR;
        float smallR = m_firstR * m_rDiff;

        float smallDelta = Mathf.Sqrt(smallR * smallR * 0.5f);
        float smallX = Random.Range(x - smallDelta, x + smallDelta);
        float smallY = Random.Range(y - smallDelta, y + smallDelta);
        Vector2 smallPos = new Vector2(smallX, smallY);
        m_circleData.SetDataReady(bigPos, bigR, smallPos, smallR);
        UpdateTransCircleSmall();
        UpdateTransCircle();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_isMove = true;
            //生成数据，开始缩圈
            if (m_isFirst == true)
            {
                m_isFirst = false;

            }
            else {
                float x = Random.Range(posXMin, posXMax);
                float y = Random.Range(posYMin, posYMax);

                Vector2 bigPos = new Vector2(x, y);
                float bigR = m_firstR;
                float smallR = m_firstR * m_rDiff;

                float smallDelta = Mathf.Sqrt(smallR * smallR * 0.5f);
                float smallX = Random.Range(x - smallDelta, x + smallDelta);
                float smallY = Random.Range(y - smallDelta, y + smallDelta);
                Vector2 smallPos = new Vector2(smallX, smallY);
                m_circleData.SetDataReady(bigPos, bigR, smallPos, smallR);
            }
            UpdateTransCircleSmall();
        }

        if (m_isMove)
        {
            float diffBigR = m_circleData.speed * Time.deltaTime;
            m_circleData.bigR -= diffBigR;
            if (m_circleData.bigR > m_circleData.smallR + m_circleData.dis)
            {
                Debug.Log("内切");
                UpdateTransCircle();
            }
            else if (m_circleData.bigR > m_circleData.smallR && m_circleData.bigR <= m_circleData.smallR + m_circleData.dis)
            {
                Debug.Log("小圆");
                m_circleData.bigPos.x += diffBigR * m_circleData.cos;
                m_circleData.bigPos.y += diffBigR * m_circleData.sin;
                UpdateTransCircle();
            }
            else if (m_circleData.bigR <= m_circleData.smallR)
            {
                m_isMove = false;
            }
        }
    }

    void UpdateTransCircle()
    {
        m_transCircle.position = new Vector3(m_circleData.bigPos.x, 0, m_circleData.bigPos.y);
        m_transCircle.localScale = new Vector3(m_circleData.bigR * 2, 1, m_circleData.bigR * 2);
    }

    void UpdateTransCircleSmall()
    {
        m_transCircleSmall.position = new Vector3(m_circleData.smallPos.x, 0, m_circleData.smallPos.y);
        m_transCircleSmall.localScale = new Vector3(m_circleData.smallR * 2, 1, m_circleData.smallR * 2);
    }
}


public class CircleData
{
    public float bigR;
    public Vector2 bigPos; //世界坐标，大圆圆心

    public float smallR;
    public Vector2 smallPos;//世界坐标，小圆圆心

    public float time = 10;
    public float speed;
    public float dis;
    public float cos;
    public float sin;
    public void SetDataReady(Vector2 pbigPos,float pbigR, Vector2 psmallPos, float psmallR)
    {
        bigPos = pbigPos;
        bigR = pbigR;
        smallPos = psmallPos;
        smallR = psmallR;

        speed = (bigR - smallR) / time;
        dis = Vector2.Distance(bigPos, smallPos);

        if (bigPos.x == smallPos.x && bigPos.y != smallPos.y)
        {
            cos = 0;
            sin = 1;
        }
        else if (bigPos.x != smallPos.x && bigPos.y == smallPos.y)
        {
            cos = 1;
            sin = 0;
        }
        else if (bigPos.x == smallPos.x && bigPos.y == smallPos.y)
        {
            cos = 0;
            sin = 0;
        }
        else 
        {
            float a = smallPos.x - bigPos.x;
            float b = smallPos.y - bigPos.y;
            
            cos = a / dis;
            sin = b /dis;
        }
        Debug.Log(string.Format("cos = {0};;sin = {1}", cos, sin));
    }


}
