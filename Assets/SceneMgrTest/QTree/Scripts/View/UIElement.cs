using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElement : MonoBehaviour
{
    // Field
    [SerializeField]
    private Image bg;

    private Element element;

    // Property
    public Element M_Element
    {
        get
        {
            return element;
        }
    }

    // Method
    public void Init(Element element, Transform parent)
    {
        this.element = element;
        gameObject.SetActive(true);
        name = $"UIElement_{element.id}";
        transform.SetParent(parent);
        transform.localPosition = new Vector3(element.x * Main.ZOOMFACTOR, element.y * Main.ZOOMFACTOR, 0);
        transform.localScale = Vector3.one;
        bg.rectTransform.sizeDelta = new Vector2(element.width * Main.ZOOMFACTOR, element.height * Main.ZOOMFACTOR);
        RefreshColor();
    }

    public void SetVisable(bool visable)
    {
        gameObject.SetActive(visable);
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
        element.x = transform.localPosition.x / Main.ZOOMFACTOR;
        element.y = transform.localPosition.y / Main.ZOOMFACTOR;
    }

    public void RefreshPosition()
    {
        transform.localPosition = new Vector3(element.x * Main.ZOOMFACTOR, element.y * Main.ZOOMFACTOR, 0);
    }

    public void RefreshColor()
    {
        bg.color = Main.Self.GetColorByElement(element.color);
    }

}
