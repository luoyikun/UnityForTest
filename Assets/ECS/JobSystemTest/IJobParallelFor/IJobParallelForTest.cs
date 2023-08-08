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

        // �ô�������һ������,��Ϊ���Ƶ�ģ��
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // �ر���Ӱ
        var renderer = sphere.GetComponent<MeshRenderer>();
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;

        // �ر���ײ��
        var collider = sphere.GetComponent<Collider>();
        collider.enabled = false;

        // ����1W����
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
            positions = m_NatListPosition, // ��ȡ��������λ������
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

    public NativeArray<Vector3> positions; // ����λ������

    [ReadOnly]
    public float  speed;

    public void Execute(int index)
    {
        Vector3 currentPosition = positions[index];

        // �����ٶȵ���Ϣ��������λ��
        currentPosition += new Vector3(speed, 0f, 0f) * deltaTime;

        positions[index] = currentPosition; // ��������λ��
    }
}
