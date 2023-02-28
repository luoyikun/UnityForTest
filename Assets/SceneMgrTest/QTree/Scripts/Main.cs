using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QuadTree;

// Emum
public enum ColorType
{
    Default = 0,
    Yellow,
    Green,
    Red,
}

// Class
public class Element : IRect
{
    public int id { get; set; }
    public ColorType color { get; set; }
    public bool isMoving { get; private set; }
    public float from_x { get; private set; }
    public float from_y { get; private set; }
    public float to_x { get; private set; }
    public float to_y { get; private set; }
    public float duration { get; private set; }

    public float x { get; set; }
    public float y { get; set; }
    public float width { get; set; }
    public float height { get; set; }

    private float factorSpeed;
    private float factor;

    public void Init(int id, float x, float y, float width, float height)
    {
        this.id = id;
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.color = 0;
    }

    public void Move(float from_x, float from_y, float to_x, float to_y, float duration)
    {
        if (duration <= 0)
            return;

        //Debug.Log($"id = {id}, duration = {duration}, from_x = {from_x}, from_y = {from_y}, to_x = {to_x}, to_y = {to_y}");

        this.from_x = from_x;
        this.from_y = from_y;
        this.to_x = to_x;
        this.to_y = to_y;
        this.duration = duration;
        isMoving = true;
        factorSpeed = 1f / duration;
        factor = 0;
        x = from_x;
        y = from_y;
    }

    public void Update(float deltaTime)
    {
        factor += deltaTime * factorSpeed;
        if (factor >= 1f)
        {
            isMoving = false;
            x = to_x;
            y = to_y;
        }
        else
        {
            x = to_x * factor + from_x * (1f - factor);
            y = to_y * factor + from_y * (1f - factor);
        }
    }
}

public class Main : MonoBehaviour
{
    // Const
    public const int ZOOMFACTOR = 10;
    public static Color DEFAULTCOLOR = new Color(1, 1, 1, 0.8f);
    public static Color GREENCOLOR = new Color(0, 1, 0, 0.8f);
    public static Color YELLOWCOLOR = new Color(1, 1, 0, 0.8f);
    public static Color REDCOLOR = new Color(1, 0, 0, 0.8f);

    // Field
    private static Main self;
    [SerializeField]
    private GameObject button_Reset;
    [SerializeField]
    private GameObject button_Add;
    [SerializeField]
    private GameObject button_AddTen;
    [SerializeField]
    private GameObject button_Play;
    [SerializeField]
    private GameObject button_Stop;
    [SerializeField]
    private UIQTree originUIQTree;
    [SerializeField]
    private UIElement originUIElement;
    [SerializeField]
    private Transform mUIQTreeParent;
    [SerializeField]
    private Transform mUIElementParent;
    [SerializeField]
    private Text label_Time;
    [SerializeField]
    private Text label_InsertTime;
    [SerializeField]
    private Text label_ObjCount;
    [SerializeField]
    private Text label_TotalCount;
    [SerializeField]
    [Range(10, 100)]
    private int rootWidth = 100;
    [SerializeField]
    [Range(10, 100)]
    private int rootHeight = 100;
    [SerializeField]
    [Range(1, 30)]
    private int targetWidth = 10;
    [SerializeField]
    [Range(1, 30)]
    private int targetHeight = 10;
    [SerializeField]
    [Range(0.01f, 0.5f)]
    private float factor = 0.25f;
    [SerializeField]
    [Range(0.01f, 10f)]
    private float moveSpeed = 0.1f;
    [SerializeField]
    [Range(1, 10000)]
    private int maxCount = 10;
    [SerializeField]
    [Range(10, 100)]
    private int delayCount = 10;
    [SerializeField]
    private Color[] colors;

    private List<Element> mElementList = new List<Element>();
    private List<UIQTree> mUIQTreeList = new List<UIQTree>();
    private List<UIElement> mUIElementList = new List<UIElement>();
    private Coroutine delayInitCoroutine = null;
    private QTree<Element> root = null;
    private Element rootElement;
    private UIElement mouseUIElement = null;
    private float deltaTime;
    private bool isInitFinish = false;
    private bool isRunning = false;

    // Property
    public static Main Self
    {
        get
        {
            return self;
        }
    }

    // Method
    private void Awake()
    {
        self = this;
        UIEventTriggerListener.GetListener(button_Reset.gameObject).onClick = Button_Reset;
        UIEventTriggerListener.GetListener(button_Add.gameObject).onClick = Button_Add;
        UIEventTriggerListener.GetListener(button_AddTen.gameObject).onClick = Button_AddTen;
        UIEventTriggerListener.GetListener(button_Play.gameObject).onClick = Button_Play;
        UIEventTriggerListener.GetListener(button_Stop.gameObject).onClick = Button_Stop;

        CanvasScaler canvasScaler = gameObject.GetComponent<CanvasScaler>();
        if (canvasScaler != null)
        {
            float height = rootHeight * 10;
            height = height < 800 ? 800 : height;
            float width = Screen.width * height / Screen.height;
            canvasScaler.referenceResolution = new Vector2(width, height);
        }

        Button_Stop(null, null);
        Init();
    }

    private void Start()
    {
        Element mouseElement = new Element();
        mouseElement.Init(-1, 0, 0, targetWidth, targetHeight);
        mouseElement.color = ColorType.Yellow;
        mouseUIElement = (Instantiate(originUIElement.gameObject) as GameObject).GetComponent<UIElement>();
        mouseUIElement.Init(mouseElement, transform);
    }

    private void Update()
    {
        if (!isInitFinish || mouseUIElement == null)
            return;

        if (isRunning && mElementList != null)
        {
            deltaTime = Time.deltaTime;
            float x = 0;
            float y = 0;
            for (int i = 0; i < mElementList.Count; ++i)
            {
                if (mElementList[i].isMoving)
                    mElementList[i].Update(deltaTime);
                else
                {
                    // 开始移动
                    x = UnityEngine.Random.Range((mElementList[i].width - rootWidth) * 0.5f, (rootWidth - mElementList[i].width) * 0.5f);
                    y = UnityEngine.Random.Range((mElementList[i].height - rootHeight) * 0.5f, (rootHeight - mElementList[i].height) * 0.5f);
                    mElementList[i].Move(mElementList[i].x, mElementList[i].y, x, y, Vector2.Distance(new Vector2(mElementList[i].x, mElementList[i].y), new Vector2(x, y)) * moveSpeed);
                }
            }
            ReGenericQTree();
            GenericQTreeView();
            for (int i = 0; i < mUIElementList.Count; ++i)
            {
                mUIElementList[i].RefreshPosition();
            }
        }

        mouseUIElement.SetPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        FindTargetAroundObjs();
    }

    public void Init()
    {
        if (delayInitCoroutine != null)
            StopCoroutine(delayInitCoroutine);

        for (int i = 0; i < mUIQTreeList.Count; ++i)
        {
            if (mUIQTreeList[i] != null)
                Destroy(mUIQTreeList[i].gameObject);
        }
        mUIQTreeList.Clear();
        for (int i = 0; i < mUIElementList.Count; ++i)
        {
            if (mUIElementList[i] != null)
                Destroy(mUIElementList[i].gameObject);
        }
        mUIElementList.Clear();

        if(root != null)
            root.Clear();
        root = null;

        delayInitCoroutine = StartCoroutine(DelayInit());
    }

    private IEnumerator DelayInit()
    {
        isInitFinish = false;


        root = QTreeManager.Insatnce.CreateQTreeRoot<Element>(1);
        root.InitRect(0, 0, rootWidth, rootHeight);
        if(rootElement == null)
            rootElement = new Element();
        rootElement.Init(-100, root.x, root.y, root.width, root.height);

        float elementWidth = rootWidth * factor / QTreeManager.MAXDEPTH;
        float elementHeigth = rootHeight * factor / QTreeManager.MAXDEPTH;
        float x = 0;
        float y = 0;
        Element element = null;
        mElementList.Clear();

        for (int i = 0; i < maxCount; ++i)
        {
            element = new Element();
            x = UnityEngine.Random.Range((elementWidth - rootWidth) * 0.5f, (rootWidth - elementWidth) * 0.5f);
            y = UnityEngine.Random.Range((elementHeigth - rootHeight) * 0.5f, (rootHeight - elementHeigth) * 0.5f);
            element.Init(i, x, y, elementWidth, elementHeigth);
            mElementList.Add(element);
        }

        float time = Time.realtimeSinceStartup;
        //所有元素插入到树
        for (int i = 0; i < mElementList.Count ; ++i)
        {
            QTreeManager.Insatnce.InsertQTree<Element>(root, mElementList[i]);
        }
        label_InsertTime.text = ((Time.realtimeSinceStartup - time) * 1000).ToString("f3") + " ms";
        label_TotalCount.text = mElementList != null ? mElementList.Count.ToString() : "0";

        List<QTree<Element>> treeList = new List<QTree<Element>>();
        QTreeManager.Insatnce.QueryQTreeReturnRiseList<Element>(root, ref treeList);

        int count = 0;
        UIQTree curTimeUIQTree = null;
        for (int i = 0; i < treeList.Count; ++i)
        {
            curTimeUIQTree = (Instantiate(originUIQTree.gameObject) as GameObject).GetComponent<UIQTree>();
            curTimeUIQTree.Init(treeList[i], mUIQTreeParent);
            mUIQTreeList.Add(curTimeUIQTree);
            //Debug.Log($"i = {i}, depth = {treeList[i].depth}");
            if (++count % delayCount == 0)
                yield return null;
        }

        UIElement curTimeUIElement = null;
        for (int i = 0; i < mElementList.Count ; ++i)
        {
            curTimeUIElement = (Instantiate(originUIElement.gameObject) as GameObject).GetComponent<UIElement>();
            curTimeUIElement.Init(mElementList[i], mUIElementParent);
            mUIElementList.Add(curTimeUIElement);
            if (++count % delayCount == 0)
                yield return null;
        }

        delayInitCoroutine = null;
        isInitFinish = true;
    }

    private void AddElement(int count)
    {
        float elementWidth = rootWidth * factor / QTreeManager.MAXDEPTH;
        float elementHeigth = rootHeight * factor / QTreeManager.MAXDEPTH;
        float x = 0;
        float y = 0;
        Element element = null;
        List<Element> list = new List<Element>();
        
        for (int i = 0; i < count; ++i)
        {
            element = new Element();
            x = UnityEngine.Random.Range((elementWidth - rootWidth) * 0.5f, (rootWidth - elementWidth) * 0.5f);
            y = UnityEngine.Random.Range((elementHeigth - rootHeight) * 0.5f, (rootHeight - elementHeigth) * 0.5f);
            element.Init(mUIElementList.Count + i, x, y, elementWidth, elementHeigth);
            list.Add(element);
            mElementList.Add(element);
            QTreeManager.Insatnce.InsertQTree<Element>(root, element);
        }

        GenericQTreeView();

        UIElement curTimeUIElement = null;
        for (int i = 0; i < list.Count; ++i)
        {
            curTimeUIElement = (Instantiate(originUIElement.gameObject) as GameObject).GetComponent<UIElement>();
            curTimeUIElement.Init(list[i], mUIElementParent);
            mUIElementList.Add(curTimeUIElement);
        }

        ResetElementColor();
    }

    /// <summary>
    /// 重新生成四叉树
    /// </summary>
    private void ReGenericQTree()
    {
        root.Clear();
        root = QTreeManager.Insatnce.CreateQTreeRoot<Element>(1);
        root.InitRect(0, 0, rootWidth, rootHeight);
        rootElement.Init(-100, root.x, root.y, root.width, root.height);
        float time = Time.realtimeSinceStartup;
        for (int i = 0; i < mElementList.Count; ++i)
        {
            QTreeManager.Insatnce.InsertQTree<Element>(root, mElementList[i]);
        }
        label_InsertTime.text = ((Time.realtimeSinceStartup - time) * 1000).ToString("f3") + " ms";
        label_TotalCount.text = mElementList != null ? mElementList.Count.ToString() : "0";
    }

    /// <summary>
    /// 生成四叉树的可视层
    /// </summary>
    private void GenericQTreeView()
    {
        for (int i = 0; i < mUIQTreeList.Count; ++i)
        {
            if (mUIQTreeList[i] != null)
                Destroy(mUIQTreeList[i].gameObject);
        }
        mUIQTreeList.Clear();

        List<QTree<Element>> treeList = new List<QTree<Element>>();
        QTreeManager.Insatnce.QueryQTreeReturnRiseList<Element>(root, ref treeList);

        UIQTree curTimeUIQTree = null;
        for (int i = 0; i < treeList.Count; ++i)
        {
            curTimeUIQTree = (Instantiate(originUIQTree.gameObject) as GameObject).GetComponent<UIQTree>();
            curTimeUIQTree.Init(treeList[i], mUIQTreeParent);
            mUIQTreeList.Add(curTimeUIQTree);
        }
    }

    private void FindTargetAroundObjs()
    {
        float time = Time.realtimeSinceStartup;
        List<Element> objs = QTreeManager.Insatnce.FindTargetAroundObjs<Element>(root, mouseUIElement.M_Element);
        if (Time.frameCount % 10 == 0)
        {
            label_Time.text = ((Time.realtimeSinceStartup - time) * 1000).ToString("f3") + " ms";
            label_ObjCount.text = objs != null ? objs.Count.ToString() : "0";
            label_TotalCount.text = mElementList != null ? mElementList.Count.ToString() : "0";
        }
        if (objs != null && objs.Count > 0)
        {
            //Debug.Log("-----------------------------------------------------");
            //for (int i = 0; i < objs.Count; ++i)
            //{
            //    Debug.Log($"i = {i}, id = {objs[i].id}");
            //}
            //Debug.Log("-----------------------------------------------------");
            for (int i = 0; i < mElementList.Count; ++i)
            {
                mElementList[i].color = objs.IndexOf(mElementList[i]) > -1 ? ColorType.Green : ColorType.Default;
            }
            CalculateCollision(mouseUIElement.M_Element, objs);
            for (int i = 0; i < mUIElementList.Count; ++i)
            {
                mUIElementList[i].RefreshColor();
            }
        }
        else
            ResetElementColor();

        //鼠标超过了全体检测范围，即超过了根节点范围
        mouseUIElement.SetVisable(CheckoutIsCollision(mouseUIElement.M_Element, rootElement));
    }

    private void ResetElementColor()
    {
        for (int i = 0; i < mElementList.Count; ++i)
        {
            mElementList[i].color = ColorType.Default;
        }
        for (int i = 0; i < mUIElementList.Count; ++i)
        {
            mUIElementList[i].RefreshColor();
        }
    }

    // 碰撞计算
    private void CalculateCollision(Element target, List<Element> objs)
    {
        for(int i = 0 ; i < objs.Count ; ++i)
        {
            objs[i].color = CheckoutIsCollision(target, objs[i]) ? ColorType.Red : objs[i].color;
        }
    }

    private bool CheckoutIsCollision(Element target, Element obj)
    {
        float halfWidth = obj.width * 0.5f;
        float halfHeight = obj.height * 0.5f;
        float min_x = target.x - target.width * 0.5f;
        float min_y = target.y - target.height * 0.5f;
        float max_x = target.x + target.width * 0.5f;
        float max_y = target.y + target.height * 0.5f;
        
        if (min_x > obj.x + halfWidth || max_x < obj.x - halfWidth
            || min_y > obj.y + halfHeight || max_y < obj.y - halfHeight)
            return false;
        else
            return true;
    }

    private void Button_Reset(UnityEngine.EventSystems.PointerEventData eventData, GameObject obj)
    {
        Init();
    }

    private void Button_Play(UnityEngine.EventSystems.PointerEventData eventData, GameObject obj)
    {
        if (delayInitCoroutine != null)
            return;
        button_Play.SetActive(false);
        button_Stop.SetActive(true);
        isRunning = true;
    }

    private void Button_Stop(UnityEngine.EventSystems.PointerEventData eventData, GameObject obj)
    {
        if (delayInitCoroutine != null)
            return;
        button_Play.SetActive(true);
        button_Stop.SetActive(false);
        isRunning = false;
    }

    private void Button_Add(UnityEngine.EventSystems.PointerEventData eventData, GameObject obj)
    {
        if (delayInitCoroutine != null)
            return;
        AddElement(1);
    }

    private void Button_AddTen(UnityEngine.EventSystems.PointerEventData eventData, GameObject obj)
    {
        if (delayInitCoroutine != null)
            return;
        AddElement(10);
    }

    public Color GetColorByDepth(int depth)
    {
        return colors[depth - 1];
    }

    public Color GetColorByElement(ColorType color)
    {
        if (color == ColorType.Yellow)
            return YELLOWCOLOR;
        else if (color == ColorType.Green)
            return GREENCOLOR;
        else if (color == ColorType.Red)
            return REDCOLOR;
        else
            return DEFAULTCOLOR;
    }

}
