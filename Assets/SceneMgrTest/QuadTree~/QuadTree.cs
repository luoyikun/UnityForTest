using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://blog.csdn.net/zhangliff/article/details/123058203
/// <summary>
/// 即是树，对每个单独的节点来说也是一个分叉树
/// </summary>
public class QuadTree
{
    //节点内允许的最大对象，当前层分出几个节点
    private int MAX_OBJECTS = 1;
    //最大层级，可展开的层级
    private int MAX_LEVELS = 3;
    //当前层级
    private int level;
    //当前层级内的对象
    public List<RectTransform> rectTrans;
    //rect范围
    private Rect bounds;
    //子节点
    private List<QuadTree> childs;

    /// <summary>
    /// 构建四叉树，也可能是根节点（整个屏幕），也可能子节点
    /// </summary>
    /// <param name="pBounds">当前范围</param>
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
    /// 清理四叉树
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
    /// 分割四叉树
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
        //对象下沉
        for (int i = 0; i < rectTrans.Count; i++)
        {
            Insert(rectTrans[i]);
        }
        rectTrans.Clear();
    }
    /// <summary>
    /// 寻找对象所在节点列表
    /// </summary>
    /// <param name="rect">对象的rect</param>
    /// <returns></returns>
    public List<QuadTree> GetIndexes(Rect rect)
    {
        List<QuadTree> ret = new List<QuadTree>();
        FindQuad(rect, ret);
        return ret;
    }

    /// <summary>
    /// 插入对象，
    /// </summary>
    /// <param name="go"></param>
    public void Insert(RectTransform go)
    {
        Rect rect = GetRect(go);
        List<QuadTree> tempList = GetIndexes(rect);

        Debug.Log("插入父类长度 : " + tempList.Count);
        for (int i = 0; i < tempList.Count; i++)
        {
            QuadTree quad = tempList[i];
            quad.rectTrans.Add(go);
            //判断对象是否可以分割
            if (quad.rectTrans.Count > MAX_OBJECTS && quad.level < MAX_LEVELS)
            {
                quad.Split();
            }
        }
    }


    /// <summary>
    /// 寻找Rect所在的节点列表。。递归
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public void FindQuad(Rect rect, List<QuadTree> quadTrees)
    {

        if (bounds.Overlaps(rect))
        {
            if (childs.Count == 0)
            {
                //这个树还没加入节点，先加入自己
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
    /// 获取所有相交的对象
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
    /// 获取所有叶子节点，递归
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
    /// 获取对象的rect,得到的是左下角+ 长宽，基于屏幕坐标
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <returns></returns>
    public static Rect GetRect(RectTransform rectTransform)
    {
        Rect rect = rectTransform.rect;
        return new Rect(rectTransform.position.x + rect.x, rectTransform.position.y + rect.y, rect.width, rect.height);
    }
}
