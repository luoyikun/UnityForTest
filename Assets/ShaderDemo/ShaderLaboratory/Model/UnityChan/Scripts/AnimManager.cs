using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimManager : MonoBehaviour
{
    public float speed = 5;

    private Rigidbody rgd;
    private Animator anim;

    private void Awake()
    {
        rgd = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {       
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 temp = transform.position;
            temp.z += Time.deltaTime * speed;
            anim.SetFloat("speed", speed);
            transform.position = temp;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Vector3 temp = transform.position;
            temp.z -= Time.deltaTime * speed;
            anim.SetFloat("speed", speed);
            transform.position = temp;
        }
        else
        {
            anim.SetFloat("speed", 0);
        }
    }
}
