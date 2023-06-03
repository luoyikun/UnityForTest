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
    public float m_viewWidth = 0;
    public float m_mapWidth = 1680;
    public float m_mapHeight = 960;
    public float m_xScale = 0;
    public float m_yScale = 0;
    public Transform m_playerArrow;
    public Transform m_targetPoint;
    // Start is called before the first frame update
    void Start()
    {
        m_viewWidth = m_imgMap.rectTransform.rect.width;
        m_xScale = 0.1f;
        m_yScale = m_xScale * m_mapWidth / m_mapHeight;
    }


    private void LateUpdate()
    {
        float posPlayerX = m_player.position.x / m_mapWidth;
        float posPlayerY = m_player.position.z / m_mapHeight;

        m_imgMap.uvRect = new Rect(posPlayerX - m_xScale / 2, posPlayerY - m_yScale / 2, m_xScale, m_yScale);
        Vector3 oriArrow = m_playerArrow.transform.eulerAngles;
        oriArrow.z = -m_player.eulerAngles.y;
        m_playerArrow.eulerAngles = oriArrow;
        Vector2 pos = GetTarget2MiniMapPoint(m_target.position, posPlayerX, posPlayerY);
        m_targetPoint.position = pos;
    }

    public Vector2 GetTarget2MiniMapPoint(Vector3 targetWorldPos, float playerX, float playerY)
    {
        float x = targetWorldPos.x / m_mapWidth - playerX;
        float y = targetWorldPos.z / m_mapHeight - playerY;
        return new Vector2(x, y);
    }
}
