using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveCtrl : MonoBehaviour
{
    public float speed = 5.0f;
    public float rotateSpeed = 5.0f;
    Transform m_Transform;

    //�ƶ��ķ���ֵΪ  1,0��-1 ����״̬
    private float m_AxisX = 0f;
    private float m_AxisY = 0f;

    public float KeyBoardAxisX
    {
        get
        {
            return m_AxisX;
        }
    }

    public float KeyBoardAxisY
    {
        get
        {
            return m_AxisY;
        }
    }

    //��ť���£�̧��״̬
    private bool m_keyUp = false;
    private bool m_keyDown = false;
    private bool m_keyLeft = false;
    private bool m_keyRight = false;

    public bool KeyUp
    {
        set
        {
            if (value)
            {
                if (!m_keyDown)
                    m_AxisY = 1f;
            }
            else
            {
                if (!m_keyDown)
                    m_AxisY = 0f;
                else
                    m_AxisY = -1f;
            }
            m_keyUp = value;
        }
    }

    public bool KeyDown
    {
        set
        {
            if (value)
            {
                if (!m_keyUp)
                    m_AxisY = -1f;
            }
            else
            {
                if (!m_keyUp)
                    m_AxisY = 0f;
                else
                    m_AxisY = 1f;
            }
            m_keyDown = value;
        }
    }

    public bool KeyLeft
    {
        set
        {
            if (value)
            {
                if (!m_keyRight)
                    m_AxisX = -1f;
            }
            else
            {
                if (!m_keyRight)
                    m_AxisX = 0f;
                else
                    m_AxisX = 1f;
            }
            m_keyLeft = value;
        }
    }

    public bool KeyRight
    {
        set
        {
            if (value)
            {
                if (!m_keyLeft)
                    m_AxisX = 1f;
            }
            else
            {
                if (!m_keyLeft)
                    m_AxisX = 0f;
                else
                    m_AxisX = -1f;
            }
            m_keyRight = value;
        }
    }

    //�������ķ���
    private float m_HorizontalRaw = 0f;
    private float m_VerticalRaw = 0f;

    

    // Start is called before the first frame update
    void Start()
    {
        m_Transform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        //��
        if (Input.GetKeyDown(KeyCode.W))
        {
            KeyUp = true;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            KeyUp = false;
        }

        //��
        if (Input.GetKeyDown(KeyCode.S))
        {
            KeyDown = true;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            KeyDown = false;
        }

        //��
        if (Input.GetKeyDown(KeyCode.A))
        {
            KeyLeft = true;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            KeyLeft = false;
        }

        //��
        if (Input.GetKeyDown(KeyCode.D))
        {
            KeyRight = true;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            KeyRight = false;
        }

        if (KeyBoardAxisX == 0 && KeyBoardAxisY == 0)
        {
            //û������
            return;
        }

        float newAngle = GetAngleByInput(KeyBoardAxisX, KeyBoardAxisY);

        Vector3 newPos = CalculateTargetPosition(m_Transform.position, m_Transform.rotation.eulerAngles, newAngle, speed * Time.deltaTime);
        m_Transform.position = newPos;

        Quaternion rotation = Quaternion.Euler(m_Transform.eulerAngles.x, newAngle, m_Transform.eulerAngles.z);
        //�µķ����ò�ֵ����������ʹ��סaw��Ȼ���ɿ���w���������ɿ�a�������Ƕȱ仯Ҳ�ǰ��ս��ٶȴ����������ͻ���б�����Ϊ������
        m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, rotation, rotateSpeed * Time.deltaTime);
    }

    //��Ϊ�����z ���򣬶�Ӧangle.y = 0����˳ʱ��ת�� angle.y����
    public static float GetAngleByInput(float X, float Y)
    {
        float Angel = 0;
        if (Y == 0.0f)
        {
            Angel = X > 0 ? 90 : 270;
        }
        else
        {
            float tmpValue = (float)Math.Sqrt(Y * Y + X * X);

            Angel = (float)Math.Acos(Y / tmpValue) * 180.0f / 3.1415926f;
            if (X < 0.0f)
            {
                Angel = 360 - Angel;
            }
        }

        return Angel;
    }

    public static Vector3 CalculateTargetPosition(Vector3 position, Vector3 eulerAngles, float angle, float speed, float angleX = 0)
    {
        //������ƶ�Ŀ���
        return position + Quaternion.Euler(eulerAngles.x + angleX, angle, eulerAngles.z) * Vector3.forward * speed;
    }
}
