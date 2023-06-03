using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigMap : MonoBehaviour
{
    public Button m_btnClose;
    public Transform m_playerArrow;
    public float m_widthPixel = 1280;
    public float m_heightPixel = 720;

    public float m_widthScene = 1680;
    public float m_heightScene = 960;
    
    // Start is called before the first frame update
    void Start()
    {
        m_btnClose.onClick.AddListener(OnBtnClose);
    }

    void OnBtnClose()
    {
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 posWorldPlayer = CircleMgr.instance.m_player.position;
        float x = posWorldPlayer.x * m_widthPixel / m_widthScene;
        x -= m_widthPixel / 2;

        float y = posWorldPlayer.z * m_heightPixel / m_heightScene;
        y -= m_heightPixel / 2;

        m_playerArrow.localPosition = new Vector3(x, y, 0);

        Vector3 oriArrow = m_playerArrow.transform.eulerAngles;
        oriArrow.z = -CircleMgr.instance.m_player.eulerAngles.y;
        m_playerArrow.eulerAngles = oriArrow;
    }
}
