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
        private static readonly Color OpenColor = new Color(.4f, .6f, .4f); //��ɫ
        private static readonly Color ClosedColor = new Color(0.35f, 0.4f, 0.5f); //��ɫ
        
        public static List<NodeBase> FindPath(NodeBase startNode, NodeBase targetNode) {
            var listOpen = new List<NodeBase>() { startNode }; //���ż���,���Խ��м���(��û�б�ѡ��)�Ľڵ� ,����ʼ�ڵ���뵽OpenSet���С����п��ܣ�ÿ��һ����Ҫ����
            var listClose = new List<NodeBase>();//��ռ���,�����Ѿ����������Ҳ�����еĽڵ� ����ʾ�ĵ��Ѿ��߹������Ǵ�����С�ĵ�

            while (listOpen.Any()) {

                //�ҵ���СF todo���Ż���ÿ�νڵ��
                var current = listOpen[0];

                //�ҳ�OpenSet����fCost��С�ĵ�,�������Ǳ�ѡ�еĽ�Ҫ���ߵ���һ���㣬���F�ܺ���ͬ����H��С���������յ���죨H���ֹ۹��ƣ������ϰ�����ƣ�
                foreach (var t in listOpen) 
                    if (t.F < current.F || ( t.F == current.F && t.H < current.H)) current = t;

                //��������OpenSet�����Ƴ�
                listOpen.Remove(current);

                //���������뵽ClostSet����
                listClose.Add(current);
                
                
                current.SetColor(ClosedColor);

                //·���Ѿ�����
                if (current == targetNode)
                {
                    var currentPathTile = targetNode;
                    var path = new List<NodeBase>();
                    //var count = 100;
                    while (currentPathTile != startNode) {
                        path.Add(currentPathTile);
                        currentPathTile = currentPathTile.Connection; //cur����һ�����ӵ�
                        //count--;
                        //if (count < 0) throw new Exception();
                        //Debug.Log("sdfsdf");
                    }
                    
                    foreach (var tile in path) tile.SetColor(PathColor);
                    startNode.SetColor(PathColor);
                    Debug.Log(path.Count);
                    return path;
                }

                //��curNode���ھӣ��ɵ����δ��close�����Ѿ�����Ĳ����ٽ��м��㣩
                foreach (var neighbor in current.Neighbors.Where(t => t.Walkable && !listClose.Contains(t))) {
                    //�Ƿ�δ���������ھ�
                    var inSearch = listOpen.Contains(neighbor);
                    //��cur���ھӵ�G = curG+cur���ھӵ�G����
                    var costToNeighbor = current.G + current.GetDistance(neighbor);

                    //·�������ı䣬g��仯�������ҵ�һ�����õ�·
                    if (!inSearch || costToNeighbor < neighbor.G) {
                        neighbor.SetG(costToNeighbor);
                        neighbor.SetConnection(current);
                        
                        //h���ֹ۹��ƣ�ֻ��Ҫ��һ��
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