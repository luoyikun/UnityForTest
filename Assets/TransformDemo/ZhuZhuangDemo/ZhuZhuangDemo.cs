using DOTweenDemo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZhuZhuangDemo : MonoBehaviour
{
    float m_dis = 0.2f;

    List<string> m_listOrder = new List<string>(new string[] {
        "Cube0",
        "Cube1",
        "Cube2",
        "Cube3"
    });

    public Transform m_lingJian;
    public Transform m_slotPar;
    public int m_idx = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        InputCtrl();
        //CheckCurIsAnZhuangOK();
        UpdateCurIdx();
    }

    void UpdateCurIdx()
    {
        if (m_idx < m_listOrder.Count)
        {
            SetActiveTwoPair(m_listOrder[m_idx]);
        }
    }

    void InputCtrl()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (m_idx < m_listOrder.Count)
            {
                string name = m_listOrder[m_idx];
                Transform liJian = m_lingJian.Find(name);
                Transform slot = m_slotPar.Find(name);
                if (liJian != null && slot != null)
                {
                    liJian.DoMove(slot.position, 3).SetOnComplete(
                         ()=>
                         {
                             FinishOneStep(m_idx);
                             m_idx++;
                         }
                        );
                }
            }
        }
    }
    void CheckCurIsAnZhuangOK()
    {
        if (m_idx < m_listOrder.Count)
        {
            string name = m_listOrder[m_idx];
            Transform liJian = m_lingJian.Find(name);
            Transform slot = m_slotPar.Find(name);
            if (liJian != null && slot != null)
            {
                if ((liJian.position - slot.position).sqrMagnitude < m_dis * m_dis)
                {
                    FinishOneStep(m_idx);
                    m_idx++;
                }
            }

        }
    }

    void FinishOneStep(int idx)
    {
        if (m_idx < m_listOrder.Count)
        {
            string name = m_listOrder[m_idx];
            Transform liJian = m_lingJian.Find(name);
            Transform slot = m_slotPar.Find(name);

            if (liJian != null && slot != null)
            {
                slot.gameObject.SetActive(false);
                liJian.SetParent(m_slotPar,true);
                //liJian.localScale = slot.localScale;
                liJian.localPosition = slot.localPosition;
                liJian.localEulerAngles = slot.localEulerAngles;
                liJian.GetComponent<HighlightableObject>().ConstantOff();
            }
         }
    }

    void SetActiveTwoPair(string name)
    {
        Transform liJian = m_lingJian.Find(name);
        Transform slot = m_slotPar.Find(name);
        if (liJian != null && slot != null)
        {
            liJian.gameObject.GetComponent<HighlightableObject>().ConstantOn();
            slot.gameObject.SetActive(true);
        }
      
    }

  

}
