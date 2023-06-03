using Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMgr : MonoSingleton<CircleMgr>
{
    public Transform m_player;
    public GameObject m_bigMap;
    public Transform m_circle;
    public float posXMin = 560;
    public float posXMax = 1120;
    public float posYMin = 320;
    public float posYMax = 640;

    public float m_firstR = 300; //��һ�δ�Բ�뾶
    public float m_rDiff = 0.5f; //�뾶ÿ�ι̶����ٰٷֱ�
    bool m_isFirst = true;
    CircleData m_circleData = new CircleData();
    // Start is called before the first frame update
    void Start()
    {
        //���ɵ�һ������

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //�������ݣ���ʼ��Ȧ
            if (m_isFirst == true)
            {
                float x = Random.Range(posXMin, posXMax);
                float y = Random.Range(posYMin, posYMax);

            }
        }
    }
}

public class CircleData
{
    public float bigR;
    public Vector2 bigPos; //�������꣬��ԲԲ��

    public float smallR;
    public Vector2 smallPos;//�������꣬СԲԲ��

    public float time = 30;
    public float speed;
    public float dis;
    public float cos;
    public float sin;
    public void SetDataReady()
    {
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
        else if (bigPos.x != smallPos.x && bigPos.y != smallPos.y)
        {
            float a = smallPos.y - bigPos.y;
            float b = smallPos.x - bigPos.x;
            
            cos = a / dis;
            sin = b /dis;
        }
    }


}
