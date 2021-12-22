using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DOTweenDemo
{
    public class DotweenScript : MonoBehaviour
    {
        public Transform m_obj;
        public Transform m_rotate;
        public Transform m_rotateTarget;
        // Start is called before the first frame update
        void Start()
        {
            m_obj.DoMove(new Vector3(10, 10, 10), 3);

            Vector3 dir = (m_rotateTarget.position - m_rotate.position);


            m_rotate.DoRotate(dir, 3);

            //按照rotate.eulerAngles面向目标来自https://docs.unity3d.com/ScriptReference/Quaternion.LookRotation.html
            //Vector3 relativePos = m_rotateTarget.position - m_rotate.position;
            //Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            ////m_rotate.transform.Rotate(rotation.eulerAngles/3);
            //m_rotate.rotation = Quaternion.Lerp(m_rotate.rotation, rotation, 0.5f);
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
