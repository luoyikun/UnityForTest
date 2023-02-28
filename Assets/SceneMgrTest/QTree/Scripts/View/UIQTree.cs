using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQTree : MonoBehaviour
{
    // Field
    [SerializeField]
    private Image bg;

    // Method
    public void Init(QuadTree.QTree<Element> tree, Transform parent)
    {
        gameObject.SetActive(true);
        name = $"UIQTree_{tree.depth}";
        transform.SetParent(parent);
        transform.localPosition = new Vector3(tree.x * Main.ZOOMFACTOR, tree.y * Main.ZOOMFACTOR, 0);
        transform.localScale = Vector3.one;
        bg.rectTransform.sizeDelta = new Vector2(tree.width * Main.ZOOMFACTOR, tree.height * Main.ZOOMFACTOR);
        bg.color = Main.Self.GetColorByDepth(tree.depth);
    }

}
