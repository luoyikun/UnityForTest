using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveAndScale : MonoBehaviour
{
    int m_iLoop = 99999999;
    public GameObject m_obj;
    public Transform m_trans;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < m_iLoop; i++)
        {
            m_obj.SetActive(false);
            m_obj.SetActive(true);
        }

        for (int i = 0; i < m_iLoop; i++)
        {
            m_trans.localScale = Vector3.zero;
            m_trans.localScale = Vector3.one;
        }
    }
}
