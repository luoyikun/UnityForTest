using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    public Transform m_player;
    public Transform m_target;
    public RawImage m_imgMap;
    public float m_fMapScale = 0.1f;
    public float m_viewWidth = 0; //RawImage的宽
    public float m_mapWidth = 1680; //世界中场景的宽度，米
    public float m_mapHeight = 960; //世界中场景的高度，米
    public float m_xScale = 0; //表示 uv 的取值，x的范围 0-1
    public float m_yScale = 0;
    public Transform m_playerArrow;
    public RectTransform m_targetPoint;
    public float m_totalMeter = 0; //小地图，总像素长代表的米
    public float m_meter2Pixel = 0; //米转换为像素
    public Button m_btnOpenBigMap;
    // Start is called before the first frame update
    void Start()
    {
        m_viewWidth = m_imgMap.rectTransform.rect.width;
        m_xScale = 0.1f;
        m_yScale = m_xScale * m_mapWidth / m_mapHeight;

        m_totalMeter = m_mapWidth * m_xScale;
        m_btnOpenBigMap.onClick.AddListener(OnBtnOpenBigMap);
    }

    void OnBtnOpenBigMap()
    {
        CircleMgr.instance.m_bigMap.SetActive(true);
    }

    private void Update()
    {
        SetMeInMini();
        Vector2 pos = GetTarget2MiniMapPoint(m_target.position, m_player.position);
        m_targetPoint.anchoredPosition = pos;
    }

    public void SetMeInMini()
    {
        float posPlayerX = m_player.position.x / m_mapWidth;
        float posPlayerY = m_player.position.z / m_mapHeight;

        m_imgMap.uvRect = new Rect(posPlayerX - m_xScale / 2, posPlayerY - m_yScale / 2, m_xScale, m_yScale);
        Vector3 oriArrow = m_playerArrow.transform.eulerAngles;
        oriArrow.z = -m_player.eulerAngles.y;
        m_playerArrow.eulerAngles = oriArrow;
    }

    public Vector2 GetTarget2MiniMapPoint(Vector3 targetWorldPos,Vector3 posPlayer)
    {
        //世界坐标上与me的差，
        float x = (targetWorldPos.x - posPlayer.x) * m_viewWidth / m_totalMeter;
        float y = ( targetWorldPos.z  - posPlayer.z) * m_viewWidth / m_totalMeter;
        return new Vector2(x, y);
    }
}
