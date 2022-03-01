using UnityEngine;

public class GameObjectBrush : MonoBehaviour
{
    [SerializeField] private float width = 0.1f;
    [SerializeField] private Color color = Color.grey;

    private LineRenderer currentLR;
    private Vector2 previousPoint;

    private void Start()
    {
        int val = (int)((210000.0f * 0.7f) / 3000.0f) + 1;
        Debug.Log(val);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //线条渲染
            currentLR = new GameObject("LineRenderer").AddComponent<LineRenderer>();
            currentLR.material = new Material(Shader.Find("Sprites/Default")) { color = color };
            currentLR.widthMultiplier = width;
            currentLR.useWorldSpace = false;
            currentLR.positionCount = 1;
            currentLR.SetPosition(0, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));

            //更新数据
            previousPoint = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        else if (Input.GetMouseButton(0))
        {
            if (previousPoint != (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition))
            {
                //线条渲染
                currentLR.positionCount++;
                currentLR.SetPosition(currentLR.positionCount - 1, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));

                //碰撞器
                BoxCollider2D collider = new GameObject("BoxCollider2D").AddComponent<BoxCollider2D>();
                collider.transform.parent = currentLR.transform;
                Vector2 latestPoint = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
                collider.transform.position = (previousPoint + latestPoint) * 0.5f;
                float angle = Mathf.Atan2((latestPoint - previousPoint).y, (latestPoint - previousPoint).x) * Mathf.Rad2Deg;
                collider.transform.eulerAngles = new Vector3(0, 0, angle);
                collider.size = new Vector2(Vector2.Distance(latestPoint, previousPoint), width);

                //更新数据
                previousPoint = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        else if (Input.GetMouseButtonUp(0))
        {
            if (currentLR.transform.childCount > 0)
            {
                currentLR.gameObject.AddComponent<Rigidbody2D>().useAutoMass = true;
            }
        }
    }
}