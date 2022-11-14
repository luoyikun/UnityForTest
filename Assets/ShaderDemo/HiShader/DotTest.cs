using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotTest : MonoBehaviour
{
    public Transform initial;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        var initialForward = initial.forward;
        var initialTargetVector = target.position - initial.position;
        var dot = Vector3.Dot(initialForward, initialTargetVector);
        if (dot > 0)
        {
            Debug.Log("target��initial�ĺ���");
        }
        else if (dot < 0)
        {
            Debug.Log("target��initial��ǰ��");
        }
        else
        {
            Debug.Log("target��initialƽ��");
        }


    }
}
