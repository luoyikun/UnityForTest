using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AOE
{
    public static class AoeUtil 
    {
        //外积。两个向量v1(x1, y1)和v2(x2, y2)的外积v1×v2=x1y2-y1x2。
        //>0,a在b顺时针方向    <0,a在b逆时针
        public static float Cross(this Vector2 a, Vector2 b)
        {
            return a.x * b.y - b.x * a.y;
        }

        public static bool IsPointInRectangle(Vector2 P, Vector2[] rectCorners)
        {
            return IsPointInRectangle(P, rectCorners[0], rectCorners[1], rectCorners[2], rectCorners[3]);
        }

        //矩形4个点，从第一个点开始逆时针或者顺时针排序
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
        /// 计算线段与点的平方距离，点在线段之间是垂直距离，否则是与最近端点距离
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="u">线段方向至末端点,为两点相减</param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float SegmentPointSqrDistance(Vector2 x0, Vector2 u, Vector2 x)
        {
            float t = Vector2.Dot(x - x0, u) / u.sqrMagnitude;
            return (x - (x0 + Mathf.Clamp(t, 0, 1) * u)).sqrMagnitude;
        }

        /// <summary>
        /// 三角函数法求x到直线x0为起点，u为单位向量的垂直最短距离平方
        /// </summary>
        /// <param name="x0">起点</param>
        /// <param name="u">射线的单位向量</param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float StraightPointSqrMinDistanceByDir(Vector2 x0, Vector2 u, Vector2 x)
        {
            float t = Vector2.Dot(x - x0, u);
            return (x - (x0 + Mathf.Abs(t) * u)).sqrMagnitude;
        }

        /// <summary>
        /// 圆与矩形是否相交
        /// </summary>
        /// <param name="cc">圆心</param>
        /// <param name="r">圆半径</param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool IsCicleRectIntersect(Vector2 cc, float r, Vector2 rectA, Vector2 rectB, Vector2 rectC, Vector2 rectD)
        {
            if (IsPointInRectangle(cc, rectA, rectB, rectC, rectD))//圆心在矩形内部
            {
                return true;
            }
            else//圆心在矩形外部，与任意一条边相交，即相交
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

        // 扇形与圆盘相交测试
        // a 扇形圆心
        // u 扇形方向（单位矢量）
        // theta 扇形扫掠半角 
        // l 扇形边长
        // c 圆盘圆心
        // r 圆盘半径
        public static bool IsCicleSectorIntersect(
            Vector2 a, Vector2 u, float theta, float l,
            Vector2 c, float r)
        {
            // 1. 如果扇形圆心和圆盘圆心的方向能分离，两形状不相交
            Vector2 d = c - a;
            float rsum = l + r;
            if (d.sqrMagnitude > rsum * rsum)
                return false;

            // 2. 计算出扇形局部空间的 p
            float px = Vector2.Dot(d, u);
            float py = Mathf.Abs(Vector2.Dot(d, new Vector2(-u.y, u.x)));//扇形单位方向向量逆时针转90度

            // 3. 如果 p_x > ||p|| cos theta，两形状相交
            if (px > d.magnitude * Mathf.Cos(theta * Mathf.Deg2Rad))
                return true;

            // 4. 求左边线段与圆盘是否相交
            Vector2 q = l * new Vector2(Mathf.Cos(theta * Mathf.Deg2Rad), Mathf.Sin(theta * Mathf.Deg2Rad));
            Vector2 p = new Vector2(px, py);
            return SegmentPointSqrDistance(Vector2.zero, q, p) <= r * r;
        }

        /// <summary>
        /// 点与扇形相交
        /// </summary>
        /// <param name="point"></param>
        /// <param name="sectorCenter">扇形圆心</param>
        /// <param name="sectorDir">扇形单位向量</param>
        /// <param name="sectorAngle">扇形角度</param>
        /// <param name="sectorRadius">扇形半径</param>
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
        /// 点到平面距离，4个点必须组成不共线的两个向量
        /// 原理：1，平面的法向量   2.oa在法向量上投影
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

        //绘制圆形
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
        /// 两个向量之间夹角
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="ori">圆点</param>
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

        //绘制无朝向矩形
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
        /// 绘制扇形
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
            //角度没分割均匀，补最后一段扇形弧面的直线
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
