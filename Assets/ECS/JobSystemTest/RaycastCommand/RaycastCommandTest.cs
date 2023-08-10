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
        // ����һЩ���Զ����λ��
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

        // ������Ҫ�������߼���ԭ��ͷ�������
        Vector3 direction = Vector3.forward;

        // ��ʼ�� NativeArrays �������洢�������
        raycastCommands = new NativeList<RaycastCommand>(m_sum, Allocator.Persistent);
        raycastResults = new NativeList<RaycastHit>(m_sum, Allocator.Persistent);

        // ��������Ҫִ�еĹ������������洢�� raycasts ��
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
        // ���ȹ��������������������Ͷ��
        JobHandle handle1 = RaycastCommand.ScheduleBatch(raycastCommands, raycastResults, 1, default(JobHandle));
        handle1.Complete();
        int sum = 0;
        // ������
        for (int i = 0; i < m_sum; i++)
        {
            if (raycastResults[i].collider != null)
            {
                sum++;
                Debug.Log("Ŀ�� " + raycastResults[i].collider.name + " �����У�");
            }
        }
        Debug.Log("������" + sum);
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
                    Debug.Log("Ŀ�� " + _blobRaycastHit.collider.name + " �����У�");
                }
            }
            Debug.Log("������" + sum);
        }
    }
    void OnDestroy()
    {
        // ���� NativeArrays ����
        raycastCommands.Dispose();
        raycastResults.Dispose();
    }
}
