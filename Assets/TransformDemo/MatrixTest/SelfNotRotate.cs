using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfNotRotate : MonoBehaviour
{
    Vector3 angle;
    // Start is called before the first frame update
    void Start()
    {
        angle = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = angle;
    }
}
