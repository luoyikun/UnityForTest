using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Tiles;
using UnityEngine;

namespace Tarodev_Pathfinding._Scripts {
    /// <summary>
    /// This algorithm is written for readability. Although it would be perfectly fine in 80% of games, please
    /// don't use this in an RTS without first applying some optimization mentioned in the video: https://youtu.be/i0x5fj4PqP4
    /// If you enjoyed the explanation, be sure to subscribe!
    ///
    /// Also, setting colors and text on each hex affects performance, so removing that will also improve it marginally.
    /// </summary>
    public static class Pathfinding {
        private static readonly Color PathColor = new Color(0.65f, 0.35f, 0.35f);
        private static readonly Color OpenColor = new Color(.4f, .6f, .4f); //绿色
        private static readonly Color ClosedColor = new Color(0.35f, 0.4f, 0.5f); //紫色
        
        public static List<NodeBase> FindPath(NodeBase startNode, NodeBase targetNode) {
            var listOpen = new List<NodeBase>() { startNode }; //开放集合,可以进行计算(但没有被选中)的节点 ,将初始节点加入到OpenSet当中。所有可能，每走一步都要放入
            var listClose = new List<NodeBase>();//封闭集合,所有已经计算过而且也被算中的节点 。表示改点已经走过，且是代价最小的点

            while (listOpen.Any()) {

                //找到最小F todo：优化，每次节点多
                var current = listOpen[0];

                //找出OpenSet当中fCost最小的点,这个点就是被选中的将要行走的下一个点，如果F总和相同，找H最小，即到达终点最快（H是乐观估计，不算障碍物估计）
                foreach (var t in listOpen) 
                    if (t.F < current.F || ( t.F == current.F && t.H < current.H)) current = t;

                //将这个点从OpenSet当中移除
                listOpen.Remove(current);

                //将这个点加入到ClostSet当中
                listClose.Add(current);
                
                
                current.SetColor(ClosedColor);

                //路径已经到了
                if (current == targetNode)
                {
                    var currentPathTile = targetNode;
                    var path = new List<NodeBase>();
                    //var count = 100;
                    while (currentPathTile != startNode) {
                        path.Add(currentPathTile);
                        currentPathTile = currentPathTile.Connection; //cur的上一个连接点
                        //count--;
                        //if (count < 0) throw new Exception();
                        //Debug.Log("sdfsdf");
                    }
                    
                    foreach (var tile in path) tile.SetColor(PathColor);
                    startNode.SetColor(PathColor);
                    Debug.Log(path.Count);
                    return path;
                }

                //找curNode的邻居，可到达和未在close表（即已经算过的不会再进行计算）
                foreach (var neighbor in current.Neighbors.Where(t => t.Walkable && !listClose.Contains(t))) {
                    //是否未计算过这个邻居
                    var inSearch = listOpen.Contains(neighbor);
                    //从cur到邻居的G = curG+cur到邻居的G代价
                    var costToNeighbor = current.G + current.GetDistance(neighbor);

                    //路径发生改变，g会变化，例如找到一条更好的路
                    if (!inSearch || costToNeighbor < neighbor.G) {
                        neighbor.SetG(costToNeighbor);
                        neighbor.SetConnection(current);
                        
                        //h是乐观估计，只需要算一遍
                        if (!inSearch) {
                            neighbor.SetH(neighbor.GetDistance(targetNode));
                            listOpen.Add(neighbor);
                            neighbor.SetColor(OpenColor);
                        }
                    }
                }
            }
            return null;
        }
    }
}