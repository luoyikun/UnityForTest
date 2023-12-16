using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigMap : MonoBehaviour
{
    public Button m_btnClose;
    public RectTransform m_playerArrow;
    float m_widthPixel = 1680; //大地图顶视图这张图的尺寸，单位像素
    float m_heightPixel = 960;

    public float m_widthScene = 1680; //场景中真实的最大宽，单位米
    public float m_heightScene = 960;
    public float m_scale = 0; //1米可对应多少像素。单位为  像素/米
    public float m_xOffset = 0; //x 方向偏移像素
    public float m_yOffset = 0; //y 方向偏移像素
    public UICircleClip m_circleClip;
    // Start is called before the first frame update
    void Start()
    {
        m_btnClose.onClick.AddListener(OnBtnClose);
        m_scale = m_widthPixel / m_widthScene;

    }

    void OnBtnClose()
    {
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 posWorldPlayer = CircleMgr.instance.m_player.position;
        Vector2 playerLocalPos = PosWorld2Local(new Vector2(posWorldPlayer.x, posWorldPlayer.z));
        m_playerArrow.anchoredPosition = playerLocalPos;

        Vector3 oriArrow = m_playerArrow.transform.eulerAngles;
        oriArrow.z = -CircleMgr.instance.m_player.eulerAngles.y;
        m_playerArrow.eulerAngles = oriArrow;

        //if (CircleMgr.instance.m_isMove)
        {
            Vector2 bigPos = PosWorld2Local(CircleMgr.instance.m_circleData.bigPos);
            Vector2 smallPos = PosWorld2Local(CircleMgr.instance.m_circleData.smallPos);
            float bigR = CircleMgr.instance.m_circleData.bigR* m_scale;
            float smallR = CircleMgr.instance.m_circleData.smallR* m_scale;

            m_circleClip.SetClip(bigPos,bigR, smallPos, smallR);
        }

    }

    public Vector2 PosWorld2Local(Vector2 pos)
    {
        Vector2 ret = Vector2.zero;
        float x = pos.x * m_scale;
        x -= m_widthPixel / 2;

        float y = pos.y * m_scale;
        y -= m_heightPixel / 2;

        ret = new Vector2(x, y);
        return ret;
    }
}
