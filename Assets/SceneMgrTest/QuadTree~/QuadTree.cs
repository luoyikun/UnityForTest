using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://blog.csdn.net/zhangliff/article/details/123058203
/// <summary>
/// ����������ÿ�������Ľڵ���˵Ҳ��һ���ֲ���
/// </summary>
public class QuadTree
{
    //�ڵ�������������󣬵�ǰ��ֳ������ڵ�
    private int MAX_OBJECTS = 1;
    //���㼶����չ���Ĳ㼶
    private int MAX_LEVELS = 3;
    //��ǰ�㼶
    private int level;
    //��ǰ�㼶�ڵĶ���
    public List<RectTransform> rectTrans;
    //rect��Χ
    private Rect bounds;
    //�ӽڵ�
    private List<QuadTree> childs;

    /// <summary>
    /// �����Ĳ�����Ҳ�����Ǹ��ڵ㣨������Ļ����Ҳ�����ӽڵ�
    /// </summary>
    /// <param name="pBounds">��ǰ��Χ</param>
    /// <param name="pLevel"></param>
    /// <param name="maxObjs"></param>
    /// <param name="maxLevel"></param>
    public QuadTree(Rect pBounds, int pLevel = 0, int maxObjs = 1, int maxLevel = 3)
    {
        level = pLevel;
        rectTrans = new List<RectTransform>();
        bounds = pBounds;
        childs = new List<QuadTree>();
        MAX_OBJECTS = maxObjs;
        MAX_LEVELS = maxLevel;
    }
    /// <summary>
    /// �����Ĳ���
    /// </summary>
    public void Clear()
    {
        rectTrans.Clear();
        for (int i = 0; i < childs.Count; i++)
        {
            childs[i].Clear();
        }
    }
    /// <summary>
    /// �ָ��Ĳ���
    /// </summary>
    public void Split()
    {
        float halfWidth = bounds.width / 2;
        float halfHeight = bounds.height / 2;
        float x = bounds.x;
        float y = bounds.y;
        childs.Add(new QuadTree(new Rect(x, y + halfHeight, halfWidth, halfHeight), level + 1, MAX_OBJECTS, MAX_LEVELS));
        childs.Add(new QuadTree(new Rect(x + halfWidth, y + halfHeight, halfWidth, halfHeight), level + 1, MAX_OBJECTS, MAX_LEVELS));
        childs.Add(new QuadTree(new Rect(x, y, halfWidth, halfHeight), level + 1, MAX_OBJECTS, MAX_LEVELS));
        childs.Add(new QuadTree(new Rect(x + halfWidth, y, halfWidth, halfHeight), level + 1, MAX_OBJECTS, MAX_LEVELS));
        //�����³�
        for (int i = 0; i < rectTrans.Count; i++)
        {
            Insert(rectTrans[i]);
        }
        rectTrans.Clear();
    }
    /// <summary>
    /// Ѱ�Ҷ������ڽڵ��б�
    /// </summary>
    /// <param name="rect">�����rect</param>
    /// <returns></returns>
    public List<QuadTree> GetIndexes(Rect rect)
    {
        List<QuadTree> ret = new List<QuadTree>();
        FindQuad(rect, ret);
        return ret;
    }

    /// <summary>
    /// �������
    /// </summary>
    /// <param name="go"></param>
    public void Insert(RectTransform go)
    {
        Rect rect = GetRect(go);
        List<QuadTree> tempList = GetIndexes(rect);

        Debug.Log("���븸�೤�� : " + tempList.Count);
        for (int i = 0; i < tempList.Count; i++)
        {
            QuadTree quad = tempList[i];
            quad.rectTrans.Add(go);
            //�ж϶����Ƿ���Էָ�
            if (quad.rectTrans.Count > MAX_OBJECTS && quad.level < MAX_LEVELS)
            {
                quad.Split();
            }
        }
    }


    /// <summary>
    /// Ѱ��Rect���ڵĽڵ��б����ݹ�
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public void FindQuad(Rect rect, List<QuadTree> quadTrees)
    {

        if (bounds.Overlaps(rect))
        {
            if (childs.Count == 0)
            {
                //�������û����ڵ㣬�ȼ����Լ�
                quadTrees.Add(this);
            }
            else
            {
                for (int i = 0; i < childs.Count; i++)
                {
                    childs[i].FindQuad(rect, quadTrees);
                }
            }
        }
    }

    
    /// <summary>
    /// ��ȡ�����ཻ�Ķ���
    /// </summary>
    /// <returns></returns>
    public Dictionary<RectTransform, HashSet<RectTransform>> GetAllCollisions()
    {
        Dictionary<RectTransform, HashSet<RectTransform>> ret = new Dictionary<RectTransform, HashSet<RectTransform>>();
        List<QuadTree> leafs = new List<QuadTree>();
        GetAllLeaf(leafs);
        for (int i = 0; i < leafs.Count; i++)
        {
            for (int j = 0; j < leafs[i].rectTrans.Count; j++)
            {
                for (int k = j + 1; k < leafs[i].rectTrans.Count; k++)
                {
                    if (ret.ContainsKey(leafs[i].rectTrans[j]) && ret[leafs[i].rectTrans[j]].Contains(leafs[i].rectTrans[k])) continue;
                    if (ret.ContainsKey(leafs[i].rectTrans[k]) && ret[leafs[i].rectTrans[k]].Contains(leafs[i].rectTrans[j])) continue;
                    Rect rect1 = GetRect(leafs[i].rectTrans[j]);
                    Rect rect2 = GetRect(leafs[i].rectTrans[k]);
                    if (rect1.Overlaps(rect2))
                    {
                        if (!ret.ContainsKey(leafs[i].rectTrans[j]))
                        {
                            ret.Add(leafs[i].rectTrans[j], new HashSet<RectTransform>());
                        }
                        ret[leafs[i].rectTrans[j]].Add(leafs[i].rectTrans[k]);
                    }
                }
            }
        }
        return ret;
    }
    /// <summary>
    /// ��ȡ����Ҷ�ӽڵ㣬�ݹ�
    /// </summary>
    /// <param name="ret"></param>
    public void GetAllLeaf(List<QuadTree> ret)
    {
        if (childs.Count == 0)
        {
            if (rectTrans.Count > 1)
                ret.Add(this);
        }
        else
        {
            for (int i = 0; i < childs.Count; i++)
            {
                childs[i].GetAllLeaf(ret);
            }
        }
    }
    /// <summary>
    /// ��ȡ�����rect,�õ��������½�+ ����������Ļ����
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <returns></returns>
    public static Rect GetRect(RectTransform rectTransform)
    {
        Rect rect = rectTransform.rect;
        return new Rect(rectTransform.position.x + rect.x, rectTransform.position.y + rect.y, rect.width, rect.height);
    }
}
