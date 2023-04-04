using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveCtrl : MonoBehaviour
{
    public float speed = 1.0f;
     Transform m_Transform;
    // Start is called before the first frame update
    void Start()
    {
        m_Transform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            m_Transform.localRotation = Quaternion.Euler(0, -45, 0);//旋转的四元数
            m_Transform.Translate(new Vector3(-1, 0, 1) * speed * Time.deltaTime, Space.World);
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            m_Transform.localRotation = Quaternion.Euler(0, 45, 0);
            m_Transform.Translate(new Vector3(1, 0, 1) * speed * Time.deltaTime, Space.World);
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            m_Transform.localRotation = Quaternion.Euler(0, -135, 0);
            m_Transform.Translate(new Vector3(-1, 0, -1) * speed * Time.deltaTime, Space.World);
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        {
            m_Transform.localRotation = Quaternion.Euler(0, 135, 0);
            m_Transform.Translate(new Vector3(1, 0, -1) * speed * Time.deltaTime, Space.World);
        }
        else
        {	//单独对四个正方向最后进行检测
            if (Input.GetKey(KeyCode.W))
            {
                m_Transform.localRotation = Quaternion.Euler(0, 0, 0);
                m_Transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey(KeyCode.S))
            {
                m_Transform.localRotation = Quaternion.Euler(0, 180, 0);
                m_Transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey(KeyCode.A))
            {
                m_Transform.localRotation = Quaternion.Euler(0, -90, 0);
                m_Transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey(KeyCode.D))
            {
                m_Transform.localRotation = Quaternion.Euler(0, 90, 0);
                m_Transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
            }
        }




    }
}
