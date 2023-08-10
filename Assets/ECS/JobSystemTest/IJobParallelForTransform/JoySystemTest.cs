using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;


public class JoySystemTest : MonoBehaviour
{
    // 用于存储transform的NativeArray
    private TransformAccessArray m_TransformsAccessArray;
    private NativeArray<Vector3> m_Velocities;

    private PositionUpdateJob m_Job;
    private JobHandle m_PositionJobHandle;
    Transform[] transforms;

    public bool m_isUseJob;
    [BurstCompile]
    struct PositionUpdateJob : IJobParallelForTransform
    {
        // 给每个物体设置一个速度
        [ReadOnly]
        public NativeArray<Vector3> velocity;

        public float deltaTime;

        // 实现IJobParallelForTransform的结构体中Execute方法第二个参数可以获取到Transform
        public void Execute(int i, TransformAccess transform)
        {
            transform.position += velocity[i] * deltaTime;
        }
    }

    void Start()
    {
        m_Velocities = new NativeArray<Vector3>(10000, Allocator.Persistent);

        // 用代码生成一个球体,作为复制的模板
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // 关闭阴影
        var renderer = sphere.GetComponent<MeshRenderer>();
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;

        // 关闭碰撞体
        var collider = sphere.GetComponent<Collider>();
        collider.enabled = false;

        // 保存transform的数组,用于生成transform的Native Array
        transforms = new Transform[10000];
        // 生成1W个球
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

        // 实例化一个job,传入数据
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
            // 调度job执行
            m_PositionJobHandle = m_Job.Schedule(m_TransformsAccessArray);
        }
        else {
            for (int i = 0; i < transforms.Length; i++)
            {
                transforms[i].position += m_Velocities[i] * Time.deltaTime;
            }
        }
    }

    // 保证当前帧内Job执行完毕
    private void LateUpdate()
    {
        if (m_isUseJob)
        {
            m_PositionJobHandle.Complete();
        }
    }

    // OnDestroy中释放NativeArray的内存
    private void OnDestroy()
    {
        m_Velocities.Dispose();
        m_TransformsAccessArray.Dispose();
    }
}
