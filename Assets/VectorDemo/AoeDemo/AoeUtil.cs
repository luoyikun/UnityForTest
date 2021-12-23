using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AOE
{
    public static class AoeUtil 
    {
        //�������������v1(x1, y1)��v2(x2, y2)�����v1��v2=x1y2-y1x2��
        //>0,a��b˳ʱ�뷽��    <0,a��b��ʱ��
        public static float Cross(this Vector2 a, Vector2 b)
        {
            return a.x * b.y - b.x * a.y;
        }

        public static bool IsPointInRectangle(Vector2 P, Vector2[] rectCorners)
        {
            return IsPointInRectangle(P, rectCorners[0], rectCorners[1], rectCorners[2], rectCorners[3]);
        }

        //����4���㣬�ӵ�һ���㿪ʼ��ʱ�����˳ʱ������
        public static bool IsPointInRectangle(Vector2 P, Vector2 A, Vector2 B, Vector2 C, Vector2 D)
        {
            Vector2 AB = A - B;
            Vector2 AP = A - P;
            Vector2 CD = C - D;
            Vector2 CP = C - P;

            Vector2 DA = D - A;
            Vector2 DP = D - P;
            Vector2 BC = B - C;
            Vector2 BP = B - P;

            bool isBetweenAB_CD = AB.Cross(AP) * CD.Cross(CP) > 0;
            bool isBetweenDA_BC = DA.Cross(DP) * BC.Cross(BP) > 0;
            return isBetweenAB_CD && isBetweenDA_BC;
        }

        /// <summary>
        /// �����߶�����ƽ�����룬�����߶�֮���Ǵ�ֱ���룬������������˵����
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="u">�߶η�����ĩ�˵�,Ϊ�������</param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float SegmentPointSqrDistance(Vector2 x0, Vector2 u, Vector2 x)
        {
            float t = Vector2.Dot(x - x0, u) / u.sqrMagnitude;
            return (x - (x0 + Mathf.Clamp(t, 0, 1) * u)).sqrMagnitude;
        }

        /// <summary>
        /// ���Ǻ�������x��ֱ��x0Ϊ��㣬uΪ��λ�����Ĵ�ֱ��̾���ƽ��
        /// </summary>
        /// <param name="x0">���</param>
        /// <param name="u">���ߵĵ�λ����</param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float StraightPointSqrMinDistanceByDir(Vector2 x0, Vector2 u, Vector2 x)
        {
            float t = Vector2.Dot(x - x0, u);
            return (x - (x0 + Mathf.Abs(t) * u)).sqrMagnitude;
        }

        /// <summary>
        /// Բ������Ƿ��ཻ
        /// </summary>
        /// <param name="cc">Բ��</param>
        /// <param name="r">Բ�뾶</param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool IsCicleRectIntersect(Vector2 cc, float r, Vector2 rectA, Vector2 rectB, Vector2 rectC, Vector2 rectD)
        {
            if (IsPointInRectangle(cc, rectA, rectB, rectC, rectD))//Բ���ھ����ڲ�
            {
                return true;
            }
            else//Բ���ھ����ⲿ��������һ�����ཻ�����ཻ
            {
                float sqR = r * r;
                float disA = SegmentPointSqrDistance(rectA, rectB - rectA, cc);
                if (disA < sqR)
                {
                    return true;
                }

                float disB = SegmentPointSqrDistance(rectB, rectC - rectB, cc);
                if (disB < sqR)
                {
                    return true;
                }

                float disC = SegmentPointSqrDistance(rectC, rectD - rectC, cc);
                if (disC < sqR)
                {
                    return true;
                }

                float disD = SegmentPointSqrDistance(rectD, rectA - rectD, cc);
                if (disD < r * r)
                {
                    return true;
                }
            }
            return false;
        }

        // ������Բ���ཻ����
        // a ����Բ��
        // u ���η��򣨵�λʸ����
        // theta ����ɨ�Ӱ�� 
        // l ���α߳�
        // c Բ��Բ��
        // r Բ�̰뾶
        public static bool IsCicleSectorIntersect(
            Vector2 a, Vector2 u, float theta, float l,
            Vector2 c, float r)
        {
            // 1. �������Բ�ĺ�Բ��Բ�ĵķ����ܷ��룬����״���ཻ
            Vector2 d = c - a;
            float rsum = l + r;
            if (d.sqrMagnitude > rsum * rsum)
                return false;

            // 2. ��������ξֲ��ռ�� p
            float px = Vector2.Dot(d, u);
            float py = Mathf.Abs(Vector2.Dot(d, new Vector2(-u.y, u.x)));//���ε�λ����������ʱ��ת90��

            // 3. ��� p_x > ||p|| cos theta������״�ཻ
            if (px > d.magnitude * Mathf.Cos(theta * Mathf.Deg2Rad))
                return true;

            // 4. ������߶���Բ���Ƿ��ཻ
            Vector2 q = l * new Vector2(Mathf.Cos(theta * Mathf.Deg2Rad), Mathf.Sin(theta * Mathf.Deg2Rad));
            Vector2 p = new Vector2(px, py);
            return SegmentPointSqrDistance(Vector2.zero, q, p) <= r * r;
        }

        /// <summary>
        /// ���������ཻ
        /// </summary>
        /// <param name="point"></param>
        /// <param name="sectorCenter">����Բ��</param>
        /// <param name="sectorDir">���ε�λ����</param>
        /// <param name="sectorAngle">���νǶ�</param>
        /// <param name="sectorRadius">���ΰ뾶</param>
        /// <returns></returns>
        public static bool IsPointSectorIntersect(Vector2 point, Vector2 sectorCenter, Vector2 sectorDir, float sectorAngle, float sectorRadius)
        {
            bool isIntersect = false;
            Vector2 point2center = point - sectorCenter;

            if (Vector2.Dot(point2center, sectorDir) > 0)
            {
                float disPoint2CenterSqr = (point - sectorCenter).sqrMagnitude;
                if ((disPoint2CenterSqr <= sectorRadius * sectorRadius)  && Vector2.Angle(point2center,sectorDir) < sectorAngle*0.5f)
                {
                    isIntersect = true;
                }
            }

            return isIntersect;
        }

        /// <summary>
        /// �㵽ƽ����룬4���������ɲ����ߵ���������
        /// ԭ��1��ƽ��ķ�����   2.oa�ڷ�������ͶӰ
        /// </summary>
        /// <param name="point"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static float Point2PlaneDis(Vector3 point,Vector3 a,Vector3 b,Vector3 c,Vector3 d)
        {
            float dis = 0;
            Vector3 norDir = Vector3.Cross(a-b, c-d).normalized;
            Vector3 point2a = point - a;
            float angle = Vector3.Angle(point2a, norDir);
            dis = Mathf.Cos(angle * Mathf.Deg2Rad) * point2a.magnitude;

            return dis;
        }

        //����Բ��
        public static void DrawCircle(Vector3 origin, float radius, Color color, float time = 5)
        {
#if UNITY_EDITOR
            Vector3 axis = Vector3.forward;
            Vector3 startDir = Vector3.right;
            Vector3 currentP = origin + Vector3.right * radius;
            Vector3 oldP;

            for (int i = 0; i < 360 / 10; i++)
            {
                Vector3 dir = Quaternion.AngleAxis(10 * i, axis) * startDir;
                oldP = currentP;
                currentP = origin + dir * radius;
                Debug.DrawLine(oldP, currentP, color, time);
            }

            oldP = currentP;
            currentP = origin + Vector3.right * radius;
            Debug.DrawLine(oldP, currentP, color, time);
#endif
        }

        /// <summary>
        /// ��������֮��н�
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="ori">Բ��</param>
        /// <returns>-180---180</returns>
        public static float GetAngleByVector(Vector2 from, Vector2 to, Vector2 ori)
        {
            float angle = 0;
            from = from - ori;
            to = to - ori;
            angle = Vector2.SignedAngle(from, to);
#if UNITY_EDITOR
            Debug.DrawLine(ori, from, Color.magenta, 10);
            Debug.DrawLine(ori, to, Color.blue, 10);
#endif
            return angle;
        }

        //�����޳������
        public static void DrawRectNoDir(float x, float y, float width, float high)
        {
#if UNITY_EDITOR
            Vector2 min = new Vector2(x - width / 2, y - high / 2);
            Vector2 max = new Vector2(x + width / 2, y + high / 2);
            Debug.DrawLine(min, min + new Vector2(width, 0), Color.red, 10);
            Debug.DrawLine(min, min + new Vector2(0, high), Color.red, 10);
            Debug.DrawLine(max, max - new Vector2(width, 0), Color.red, 10);
            Debug.DrawLine(max, max - new Vector2(0, high), Color.red, 10);
#endif
        }
        /// <summary>
        /// ��������
        /// </summary>
        public static void DrawWireSemicircle(Vector3 origin, Vector3 direction, float radius, int angle, Color color, float time = 5)
        {
#if UNITY_EDITOR
            DrawWireSemicircle(origin, direction, radius, angle, Vector3.forward, color, time);
#endif
        }
        public static void DrawWireSemicircle(Vector3 origin, Vector3 direction, float radius, int angle, Vector3 axis, Color color, float time = 5)
        {
            Vector3 leftdir = Quaternion.AngleAxis(-angle / 2, axis) * direction;
            Vector3 rightdir = Quaternion.AngleAxis(angle / 2, axis) * direction;

            Vector3 currentP = origin + leftdir * radius;
            Vector3 oldP;
            if (angle != 360)
            {
                Debug.DrawLine(origin, currentP, color, time);
            }
            for (int i = 0; i < angle / 10; i++)
            {
                Vector3 dir = Quaternion.AngleAxis(10 * i, axis) * leftdir;
                oldP = currentP;
                currentP = origin + dir * radius;
                Debug.DrawLine(oldP, currentP, color, time);
            }
            //�Ƕ�û�ָ���ȣ������һ�����λ����ֱ��
            oldP = currentP;
            currentP = origin + rightdir * radius;
            Debug.DrawLine(oldP, currentP, color, time);
            if (angle != 360)
            {
                Debug.DrawLine(currentP, origin, color, time);
            }

        }
    }
}
