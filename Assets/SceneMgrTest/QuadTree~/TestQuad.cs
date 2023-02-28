using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestQuad : MonoBehaviour
{
    public GameObject Image;
    QuadTree quadTree;
    List<RectTransform> rectTransforms;
    List<LineRenderer> lineRenderers;
    public int cNums = 200;
    public bool m_isUseQuadTree;
    private void Start()
    {

        rectTransforms = new List<RectTransform>();
        lineRenderers = new List<LineRenderer>();
        Rect sc = Screen.safeArea;
        //创建四叉树，按照当前是屏幕最大，0层，当前层可展开2个节点，最多展开5层
        quadTree = new QuadTree(sc, 0, 2, 5);

        for (int i = 0; i < cNums; i++)
        {
            GameObject go = Instantiate(Image);
            RectTransform rectT = go.GetComponent<RectTransform>();
            //随机生成位置
            float x = Random.Range(0, sc.width);
            float y = Random.Range(0, sc.height);
            rectT.position = new Vector3(x, y, 0);
            go.transform.SetParent(transform);
            var move = go.GetComponent<RandomMove>();
            move.maxPos_x = sc.width / 2;
            move.maxPos_y = sc.height / 2;
            move.minPos_x = -move.maxPos_x;
            move.minPos_y = -move.maxPos_y;
            rectTransforms.Add(rectT);
        }
    }
    private void Update()
    {
        if (m_isUseQuadTree)
        {
            DealByQuadTree();
        }
        else {
            DealByForFor();
        }
    }

    void DealByQuadTree()
    {
        //清理树
        quadTree.Clear();
        //插入节点
        for (int i = 0; i < rectTransforms.Count; i++)
        {
            quadTree.Insert(rectTransforms[i]);
        }
        //获取所有交集
        var temps = quadTree.GetAllCollisions();
        List<Vector3> point = new List<Vector3>();
        int linet = 0;
        //画线
        foreach (var kv in temps)
        {
            if (linet == lineRenderers.Count)
            {
                GameObject go = new GameObject("line_" + linet);
                lineRenderers.Add(go.AddComponent<LineRenderer>());
            }
            LineRenderer lineRenderer = lineRenderers[linet++];
            lineRenderer.gameObject.SetActive(true);
            lineRenderer.positionCount = 0;
            foreach (var v in kv.Value)
            {
                //显示线条(z为-1,才能显示出来)
                lineRenderer.SetPosition(lineRenderer.positionCount++, kv.Key.position + new Vector3(0, 0, -1));
                lineRenderer.SetPosition(lineRenderer.positionCount++, v.position + new Vector3(0, 0, -1));
            }
        }

        while (linet < lineRenderers.Count)
        {
            lineRenderers[linet++].gameObject.SetActive(false);
        }
    }

    void DealByForFor()
    {
        int linet = 0;
        for (int i = 0; i < rectTransforms.Count; i++)
        {
            if (linet == lineRenderers.Count)
            {
                GameObject go = new GameObject("line_" + linet);
                lineRenderers.Add(go.AddComponent<LineRenderer>());
            }
            LineRenderer lineRenderer = lineRenderers[linet++];
            lineRenderer.gameObject.SetActive(true);
            lineRenderer.positionCount = 0;
            for (int j = i + 1; j < rectTransforms.Count; j++)
            {
                Rect rect1 = QuadTree.GetRect(rectTransforms[i]);
                Rect rect2 = QuadTree.GetRect(rectTransforms[j]);
                if (rect1.Overlaps(rect2))
                {
                    lineRenderer.SetPosition(lineRenderer.positionCount++, rectTransforms[i].position + new Vector3(0, 0, -1));
                    lineRenderer.SetPosition(lineRenderer.positionCount++, rectTransforms[j].position + new Vector3(0, 0, -1));
                }
            }
        }

        while (linet < lineRenderers.Count)
        {
            lineRenderers[linet++].gameObject.SetActive(false);
        }

    }
}

