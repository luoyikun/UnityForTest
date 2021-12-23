using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AOE
{
    public class AoeTest : MonoBehaviour
    {
        public Transform m_tar;
        public Transform m_point2Plane;
        public GameObject m_initOBj;
        // Start is called before the first frame update
        void Start()
        {
            Vector2 a = Vector2.zero;


            //Instantiate(m_initOBj);
        }

        private void Update()
        {
            //点在扇形内判断
            //Vector2 a = Vector2.zero;
            //AoeUtil.DrawWireSemicircle(a, new Vector2(1, 0), 10, 20,Color.red);
            //Debug.Log(AoeUtil.IsPointSectorIntersect(m_tar.position, a, new Vector2(1, 0), 20, 10));

            //点到面距离，两个向量必须不共线，才能得到一个平面
            Vector3 a = new Vector3(0, 0, 0);
            Vector3 b = new Vector3(0, 0, 1);
            Vector3 c = new Vector3(2, 0, 0);
            Vector3 d = new Vector3(3, 0,2);
            Debug.Log(AoeUtil.Point2PlaneDis(m_point2Plane.position, a, b, c, d));
        }

    }
}
