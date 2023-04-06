using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveCtrl : MonoBehaviour
{
    public float speed = 5.0f;
    public float rotateSpeed = 5.0f;
    Transform m_Transform;

    //移动的方向，值为  1,0，-1 三种状态
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

    //按钮按下，抬起状态
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

    //玩家输入的方向
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
        //上
        if (Input.GetKeyDown(KeyCode.W))
        {
            KeyUp = true;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            KeyUp = false;
        }

        //下
        if (Input.GetKeyDown(KeyCode.S))
        {
            KeyDown = true;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            KeyDown = false;
        }

        //左
        if (Input.GetKeyDown(KeyCode.A))
        {
            KeyLeft = true;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            KeyLeft = false;
        }

        //右
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
            //没有输入
            return;
        }

        float newAngle = GetAngleByInput(KeyBoardAxisX, KeyBoardAxisY);

        Vector3 newPos = CalculateTargetPosition(m_Transform.position, m_Transform.rotation.eulerAngles, newAngle, speed * Time.deltaTime);
        m_Transform.position = newPos;

        Quaternion rotation = Quaternion.Euler(m_Transform.eulerAngles.x, newAngle, m_Transform.eulerAngles.z);
        //新的方向用插值，这样，即使按住aw，然后松开了w，再立马松开a，这样角度变化也是按照角速度处理，不会出现突变从斜方向变为正方向
        m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, rotation, rotateSpeed * Time.deltaTime);
    }

    //因为世界的z 方向，对应angle.y = 0，再顺时针转动 angle.y增加
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
        //计算出移动目标点
        return position + Quaternion.Euler(eulerAngles.x + angleX, angle, eulerAngles.z) * Vector3.forward * speed;
    }
}
