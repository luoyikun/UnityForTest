using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;


public class JoySystemTest : MonoBehaviour
{
    // ���ڴ洢transform��NativeArray
    private TransformAccessArray m_TransformsAccessArray;
    private NativeArray<Vector3> m_Velocities;

    private PositionUpdateJob m_Job;
    private JobHandle m_PositionJobHandle;
    Transform[] transforms;

    public bool m_isUseJob;
    [BurstCompile]
    struct PositionUpdateJob : IJobParallelForTransform
    {
        // ��ÿ����������һ���ٶ�
        [ReadOnly]
        public NativeArray<Vector3> velocity;

        public float deltaTime;

        // ʵ��IJobParallelForTransform�Ľṹ����Execute�����ڶ����������Ի�ȡ��Transform
        public void Execute(int i, TransformAccess transform)
        {
            transform.position += velocity[i] * deltaTime;
        }
    }

    void Start()
    {
        m_Velocities = new NativeArray<Vector3>(10000, Allocator.Persistent);

        // �ô�������һ������,��Ϊ���Ƶ�ģ��
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // �ر���Ӱ
        var renderer = sphere.GetComponent<MeshRenderer>();
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;

        // �ر���ײ��
        var collider = sphere.GetComponent<Collider>();
        collider.enabled = false;

        // ����transform������,��������transform��Native Array
        transforms = new Transform[10000];
        // ����1W����
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                var go = GameObject.Instantiate(sphere);
                go.transform.position = new Vector3(j, 0, i);

                transforms[i * 100 + j] = go.transform;
                m_Velocities[i * 100 + j] = new Vector3(0.1f * j, 0, 0.1f * j);
            }
        }

        m_TransformsAccessArray = new TransformAccessArray(transforms);

        // ʵ����һ��job,��������
        m_Job = new PositionUpdateJob()
        {
            deltaTime = Time.deltaTime,
            velocity = m_Velocities,
        };
    }

    void Update()
    {
        if (m_isUseJob)
        {
            m_Job.deltaTime = Time.deltaTime;
            // ����jobִ��
            m_PositionJobHandle = m_Job.Schedule(m_TransformsAccessArray);
        }
        else {
            for (int i = 0; i < transforms.Length; i++)
            {
                transforms[i].position += m_Velocities[i] * Time.deltaTime;
            }
        }
    }

    // ��֤��ǰ֡��Jobִ�����
    private void LateUpdate()
    {
        if (m_isUseJob)
        {
            m_PositionJobHandle.Complete();
        }
    }

    // OnDestroy���ͷ�NativeArray���ڴ�
    private void OnDestroy()
    {
        m_Velocities.Dispose();
        m_TransformsAccessArray.Dispose();
    }
}
