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
    public float m_xScale = 0; //表示 uv 的取值w， 0-1,例如x = 0.1
    public float m_yScale = 0;  //表示uv 的取值h，要根据x* （宽/高）
    public Transform m_playerArrow;
    public RectTransform m_targetPoint;
    public float m_totalMeter = 0; //小地图，总像素长代表的米
    public float m_meter2Pixel = 0; //世界坐标的米转为小地图像素，单位 像素/米
    public Button m_btnOpenBigMap;
    public UICircleMask m_mask;
    public UICircleClip m_circleClip;
    public RectTransform m_bigPos;
    public RectTransform m_smallPos;
    Transform m_trans;
    // Start is called before the first frame update
    void Start()
    {
        m_viewWidth = m_imgMap.rectTransform.rect.width;
        m_xScale = 0.1f;
        m_yScale = m_xScale * m_mapWidth / m_mapHeight;

        m_totalMeter = m_mapWidth * m_xScale;
        m_btnOpenBigMap.onClick.AddListener(OnBtnOpenBigMap);
        m_meter2Pixel = m_viewWidth / m_totalMeter;
        m_trans = this.transform;
    }

    void OnBtnOpenBigMap()
    {
        CircleMgr.instance.m_bigMap.SetActive(true);
    }

    Vector3 GetVec3ByVec2(Vector2 vec2)
    {
        Vector3 pos = new Vector3(vec2.x, vec2.y, 0);
        return pos;
    }
    private void Update()
    {
        SetMeInMini();
        Vector2 pos = GetTarget2MiniMapPoint(m_target.position, m_player.position);
        m_targetPoint.localPosition = pos;
        Debug.Log(string.Format("m_targetPoint.localPosition ({0},{1},{2})", m_targetPoint.localPosition.x,
            m_targetPoint.localPosition.y, m_targetPoint.localPosition.z));

        //m_targetPoint.transform.localPosition = GetVec3ByVec2(pos);
        {
            Vector3 bigV3 = new Vector3(CircleMgr.instance.m_circleData.bigPos.x, 0, CircleMgr.instance.m_circleData.bigPos.y);
            Vector3 bigPos = GetTarget2MiniMapPoint(bigV3, m_player.position);
            float bigR = CircleMgr.instance.m_circleData.bigR * m_meter2Pixel;

            Vector3 smallV3 = new Vector3(CircleMgr.instance.m_circleData.smallPos.x, 0, CircleMgr.instance.m_circleData.smallPos.y);
            Vector3 smallPos = GetTarget2MiniMapPoint(smallV3, m_player.position);
            float smallR = CircleMgr.instance.m_circleData.smallR * m_meter2Pixel;

            

            //这两个点能保证在minimap是正确
            m_bigPos.localPosition = bigPos;
            m_smallPos.localPosition = smallPos;
            //bigPos = AnchoredPosition2WorldPos(bigPos);
            //smallPos = AnchoredPosition2WorldPos(smallPos);

            //m_pointTrans.
            //m_maskUGUI.SetClip(bigV3, CircleMgr.instance.m_circleData.bigR);
            //m_mask.SetClipByAnchoredPosition(bigPos, bigR, smallPos, smallR);

            //圆上一点
            
            m_circleClip.SetClip(bigPos, bigR,smallPos,smallR);
        }
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

    public Vector3 GetTarget2MiniMapPoint(Vector3 targetWorldPos,Vector3 posPlayer)
    {
        //世界坐标上与me的差，
        float x = (targetWorldPos.x - posPlayer.x) * m_meter2Pixel;
        float y = ( targetWorldPos.z  - posPlayer.z) * m_meter2Pixel;
        return new Vector3(x, y,0);
    }

    /// <summary>
    /// 世界坐标向画布坐标转换
    /// </summary>
    /// <param name="canvas">画布</param>
    /// <param name="world">世界坐标</param>
    /// <returns>返回画布上的二维坐标</returns>
    private Vector2 WorldToCanvasPos(Canvas canvas, Vector3 world)
    {
        Vector2 position;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
            world, canvas.GetComponent<Camera>(), out position);
        return position;
    }

    public Vector3 AnchoredPosition2WorldPos(Vector2 anchoredPosition)
    {

        // 将向量从本地坐标系转换为世界坐标系
        Vector3 worldPosition = m_trans.TransformPoint(new Vector3(anchoredPosition.x, anchoredPosition.y, 0));

        return worldPosition;
    }
}
