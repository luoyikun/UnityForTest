using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuadTree
{
    public enum QuadrantType
    {
        /// <summary>
        /// 左上
        /// </summary>
        LT = 0,
        /// <summary>
        /// 右上
        /// </summary>
        RT = 1,
        /// <summary>
        /// 右下
        /// </summary>
        RB = 2,
        /// <summary>
        /// 左下
        /// </summary>
        LB = 3,
    }

    public interface IRect
    {
        /// <summary>
        /// 矩形的中心坐标x
        /// </summary>
        float x { get; set; }
        /// <summary>
        /// 矩形的中心坐标y
        /// </summary>
        float y { get; set; }
        /// <summary>
        /// 矩形的宽
        /// </summary>
        float width { get; set; }
        /// <summary>
        /// 矩形的高
        /// </summary>
        float height { get; set; }
    }

    public class QTreeComparer<T> : IComparer<QTree<T>> where T : IRect
    {
        public int Compare(QTree<T> x, QTree<T> y)
        {
            if (x.depth > y.depth)
                return 1;
            else if (x.depth == y.depth)
                return 0;
            else
                return -1;
        }
    }

    public class QTree<T> : IRect where T : IRect
    {
        public float x { get; set; }
        public float y { get; set; }
        public float width { get; set; }
        public float height { get; set; }

        public int depth;  //深度，子树会深度+1
        public int childCount; //元素的个数
        public bool isLeaf; //是否是叶子节点
        public List<T> childList;  //包含的元素个数，例如一个区域内，有10个元素
        public QTree<T>[] childNodes;  //一定是4个分支树，完全的4叉树

        public QTree()
        {
            Init();
        }

        public QTree(int depth)
        {
            Init();
            this.depth = depth;
        }

        private void Init()
        {
            childList = new List<T>();
            isLeaf = true; //每个新创建的节点，一定是叶节点
            childCount = 0;
        }

        public void InitRect(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public void Clear()
        {
            if (isLeaf)
            {
                childList.Clear();
                childList = null;
            }
            else
            {
                for (int i = 0; i < childNodes.Length; ++i)
                {
                    childNodes[i].Clear();
                    childNodes[i] = null;
                }
                childNodes = null;
            }
        }
    }

    public class QTreeManager
    {
        public const int MAXDEPTH = 4;

        public const int MAXCHILDCOUNT = 4;//每个叶子节点可以存放的对象上限,
        //当叶子节点存放的对象数量超过 MAXCHILDCOUNT，则分离生成子树（4个区域象限），并把自身存储的所有对象存放到子树中，然后标记为非叶子节点，清除自身所有保存的对象引用

        private static QTreeManager instance;

        public static QTreeManager Insatnce
        {
            get
            {
                if (instance == null)
                    instance = new QTreeManager();
                return instance;
            }
        }

        /// <summary>
        /// 创建一个四叉树根节点
        /// </summary>
        /// <param name="depth">树的深度</param>
        public QTree<T> CreateQTreeRoot<T>(int depth) where T : IRect
        {
            return new QTree<T>(depth);
        }

        /// <summary>
        /// 插入四叉树
        /// </summary>
        public void InsertQTree<T>(QTree<T> node, T t) where T : IRect
        {
            if(node.isLeaf)
            {
                if(node.depth < QTreeManager.MAXDEPTH && node.childCount + 1 > QTreeManager.MAXCHILDCOUNT)
                {
                    //最多只有4层，不再分裂
                    //当叶子节点存放的对象数量超过 MAXCHILDCOUNT，则分离生成子树（4个区域象限），并把自身存储的所有对象存放到子树中，然后标记为非叶子节点，清除自身所有保存的对象引用
                    // 分裂树(象限)
                    SplitQTree(node);
                    InsertQTree(node, t);
                }
                else
                {
                    //一开始加入，等待插入满了MAXCHILDCOUNT，开始分裂
                    node.childList.Add(t);
                    node.childCount++;
                }
            }
            else
            {
                //当前节点分裂了，会找到4个方向的子树。因为有边界效应，可能出现同时属于分裂后4个区域的多个，例如元素正好在正中心
                List<int> indexs = GetTargetQuadrantIndex<T>(node, t);
                if (indexs != null && indexs.Count > 0)
                {
                    for (int i = 0; i < indexs.Count; ++i)
                    {
                        //把当前节点元素，插入到分裂后的分支数结构
                        InsertQTree<T>(node.childNodes[indexs[i]], t);
                    }
                }
            }
        }

        /// <summary>
        /// 分离树(象限)
        /// </summary>
        private void SplitQTree<T>(QTree<T> node) where T : IRect
        {
            node.isLeaf = false; //当前点可分裂，就不是叶子节点了
            float width = node.width * 0.5f; //分裂的面积为4个，长宽只有 父的一半 
            float height = node.height * 0.5f;
            //相当于分成了田字形
            node.childNodes = new QTree<T>[4];
            // 左上
            node.childNodes[(int)QuadrantType.LT] = CreateChildNode<T>(node.x - width * 0.5f, node.y + height * 0.5f, width, height, node.depth + 1);
            // 右上
            node.childNodes[(int)QuadrantType.RT] = CreateChildNode<T>(node.x + width * 0.5f, node.y + height * 0.5f, width, height, node.depth + 1);
            // 右下
            node.childNodes[(int)QuadrantType.RB] = CreateChildNode<T>(node.x + width * 0.5f, node.y - height * 0.5f, width, height, node.depth + 1);
            // 左下
            node.childNodes[(int)QuadrantType.LB] = CreateChildNode<T>(node.x - width * 0.5f, node.y - height * 0.5f, width, height, node.depth + 1);

            //倒序遍历元素，因为当前已经分裂了，会把之前的根节点，插入到4个方向的子树中
            for(int i = node.childCount - 1; i >= 0 ; --i)
            {
                InsertQTree<T>(node, node.childList[i]);
                node.childList.RemoveAt(i);
                node.childCount--;
            }
        }

        /// <summary>
        /// 创建子树（叶子节点）
        /// </summary>
        private QTree<T> CreateChildNode<T>(float x, float y, float width, float height, int depth) where T : IRect
        {
            QTree<T> node = new QTree<T>();
            node.depth = depth;
            node.InitRect(x, y, width, height);
            return node;
        }

        /// <summary>
        /// 查询树，并返回包含所有树节点的列表
        /// </summary>
        public void QueryQTreeRetrunList<T>(QTree<T> node, ref List<QTree<T>> qTreeList) where T : IRect
        {
            qTreeList.Add(node);
            if (!node.isLeaf)
            {
                for(int i = (int)QuadrantType.LT ; i <= (int)QuadrantType.LB ; ++i)
                {
                    QueryQTreeRetrunList<T>(node.childNodes[i], ref qTreeList);
                }
            }
        }

        /// <summary>
        /// 查询树，并返回包含所有树节点的列表（按depth升序排列）
        /// </summary>
        public void QueryQTreeReturnRiseList<T>(QTree<T> node, ref List<QTree<T>> qTreeList) where T : IRect
        {
            QueryQTreeRetrunList<T>(node, ref qTreeList);
            qTreeList.Sort(new QTreeComparer<T>());
        }
        
        /// <summary>
        /// 获取目标所在的象限索引列表
        /// </summary>
        public List<int> GetTargetQuadrantIndex<T>(QTree<T> node, T target) where T : IRect
        {
            float halfWidth = node.width * 0.5f;
            float halfHeight = node.height * 0.5f;
            float min_x = target.x - target.width * 0.5f;
            float min_y = target.y - target.height * 0.5f;
            float max_x = target.x + target.width * 0.5f;
            float max_y = target.y + target.height * 0.5f;

            // 不在当前节点范围内返回null
            if (min_x > node.x + halfWidth || max_x < node.x - halfWidth
                || min_y > node.y + halfHeight || max_y < node.y - halfHeight)
                return null;

            List<int> indexs = new List<int>();

            bool isLeft = min_x <= node.x ? true : false;
            bool isRight = max_x > node.x ? true : false;
            bool isBottom = min_y <= node.y ? true : false;
            bool isTop = max_y > node.y ? true : false;

            if (isLeft)
            {
                // 左上
                if (isTop)
                    indexs.Add((int)QuadrantType.LT);
                // 左下
                if (isBottom)
                    indexs.Add((int)QuadrantType.LB);
            }
            if (isRight)
            {
                // 右上
                if (isTop)
                    indexs.Add((int)QuadrantType.RT);
                // 右下
                if (isBottom)
                    indexs.Add((int)QuadrantType.RB);
            }

            return indexs;
        }

        /// <summary>
        /// 返回目标周围的可能碰撞对象列表
        /// </summary>
        public List<T> FindTargetAroundObjs<T>(QTree<T> node, T target) where T : IRect
        {
            List<T> objs = null;
            if (node.isLeaf)
            {
                //递归查找，直到找到叶子节点所有元素返回
                objs = node.childList;
            }
            else
            {
                //不是叶子节点说明还要继续找下去，目标在当前节点哪些象限
                List<int> indexs = GetTargetQuadrantIndex<T>(node, target);
                if (indexs != null && indexs.Count > 0)
                {
                    if (objs == null)
                        objs = new List<T>();
                    List<T> temp = null;
                    for (int i = 0; i < indexs.Count; ++i)
                    {
                        //最终叶子节点返回元素列表
                        temp = FindTargetAroundObjs<T>(node.childNodes[indexs[i]], target);
                        if (temp != null)
                        {
                            for (int k = 0; k < temp.Count; ++k)
                            {
                                // 避免边界对象重复加入（边界对象会存在多个象限中），去重返回
                                if (objs.IndexOf(temp[k]) < 0)
                                    objs.Add(temp[k]);
                            }
                        }
                    }
                }
            }
            return objs;
        }

    }

}
