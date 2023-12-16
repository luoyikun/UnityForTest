using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class RaycastCommandTest : MonoBehaviour
{
    private NativeList<RaycastCommand> raycastCommands;
    private NativeList<RaycastHit> raycastResults;
    List<Vector3> listVec = new List<Vector3>(10);
    public bool m_isUseJob;
    RaycastHit _blobRaycastHit;
    int m_sum = 100;
    void Start()
    {
        // 创建一些测试对象和位置
        GameObject[] targets = new GameObject[m_sum];
        for (int i = 0; i < m_sum; i++)
        {
            targets[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            targets[i].name = i.ToString();
            targets[i].transform.position = new Vector3(i * 2, 0f, 0f);

            GameObject rayObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rayObj.name = "ray" + i;
            rayObj.transform.position = new Vector3(i * 2, 0, -2f);
            rayObj.GetComponent<BoxCollider>().enabled = false;
            listVec.Add(new Vector3(i * 2, 0, -1f));
        }

        // 创建需要进行射线检测的原点和方向数组
        Vector3 direction = Vector3.forward;

        // 初始化 NativeArrays 数组来存储结果数据
        raycastCommands = new NativeList<RaycastCommand>(m_sum, Allocator.Persistent);
        raycastResults = new NativeList<RaycastHit>(m_sum, Allocator.Persistent);

        // 构造所有要执行的光束命令，并将其存储在 raycasts 中
        for (int i = 0; i < m_sum; i++)
        {
            RaycastCommand command = new RaycastCommand(listVec[i], Vector3.forward, 100);
            raycastCommands.Add(command);
            raycastResults.Add(new RaycastHit());
        }

        //JobRay();

    }


    void JobRay()
    {
        // 调度光束命令进行批处理射线投射
        JobHandle handle1 = RaycastCommand.ScheduleBatch(raycastCommands, raycastResults, 1, default(JobHandle));
        handle1.Complete();
        int sum = 0;
        // 处理结果
        for (int i = 0; i < m_sum; i++)
        {
            if (raycastResults[i].collider != null)
            {
                sum++;
                Debug.Log("目标 " + raycastResults[i].collider.name + " 被击中！");
            }
        }
        Debug.Log("总数：" + sum);
    }

    private void Update()
    {
        if (m_isUseJob == true)
        {
            JobRay();
        }
        else
        {
            int sum = 0;
            for (int i = 0; i < m_sum; i++)
            {
                bool rayCastSucceed = Physics.Raycast(listVec[i], Vector3.forward, out _blobRaycastHit, 100);

                if (rayCastSucceed)
                {
                    sum++;
                    Debug.Log("目标 " + _blobRaycastHit.collider.name + " 被击中！");
                }
            }
            Debug.Log("总数：" + sum);
        }
    }
    void OnDestroy()
    {
        // 清理 NativeArrays 数组
        raycastCommands.Dispose();
        raycastResults.Dispose();
    }
}
