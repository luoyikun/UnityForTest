using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class IJobParallelForTest : MonoBehaviour
{

    private NativeList<Vector3> m_NatListPosition;

    const int m_transNum = 10000;
    MoveObjectsJob moveObjectsJob;
    List<Transform> m_listMoveObj = new List<Transform>(m_transNum);
    public bool m_isUseJob = false;
    private void Start()
    {
        m_NatListPosition = new NativeList<Vector3>(m_transNum, Allocator.Persistent);

        // 用代码生成一个球体,作为复制的模板
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // 关闭阴影
        var renderer = sphere.GetComponent<MeshRenderer>();
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;

        // 关闭碰撞体
        var collider = sphere.GetComponent<Collider>();
        collider.enabled = false;

        // 生成1W个球
        for (int i = 0; i < m_transNum; i++)
        {

            var go = GameObject.Instantiate(sphere);
            Vector3 pos = new Vector3(i, 0, 0);
            go.transform.position = pos;

            m_NatListPosition.Add(pos);
            m_listMoveObj.Add(go.transform);
        }

        moveObjectsJob = new MoveObjectsJob
        {
            deltaTime = Time.deltaTime,
            positions = m_NatListPosition, // 获取所有物体位置引用
            speed = 10
        };

    }

    private void Update()
    {
        if (m_isUseJob)
        {
            moveObjectsJob.deltaTime = Time.deltaTime;
            JobHandle jobHandle = moveObjectsJob.Schedule(m_transNum, 64);
            jobHandle.Complete();

            for (int i = 0; i < m_transNum; i++)
            {
                m_listMoveObj[i].position = m_NatListPosition[i];
            }
        }
        else {
            for (int i = 0; i < m_transNum; i++)
            {
                m_listMoveObj[i].position += new Vector3(10, 0f, 0f) * Time.deltaTime;
            }
        }
    }

    private void OnDestroy()
    {
        m_NatListPosition.Dispose();
        
    }
}

[BurstCompile]
public struct MoveObjectsJob : IJobParallelFor
{
    public float deltaTime;

    public NativeArray<Vector3> positions; // 物体位置数组

    [ReadOnly]
    public float  speed;

    public void Execute(int index)
    {
        Vector3 currentPosition = positions[index];

        // 根据速度等信息更新物体位置
        currentPosition += new Vector3(speed, 0f, 0f) * deltaTime;

        positions[index] = currentPosition; // 更新物体位置
    }
}
