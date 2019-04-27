using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElement : MonoBehaviour
{
    private void Awake()
    {
        UIManager.Instance.AddElement(this);

        gameObject.SetActive(false);
    }

    public string GetIdentifier()
    {
        return name_identifier;
    }

    public void SetInfo(CanvasGroup cg, RectTransform rt, Canvas c)
    {      
       canvas_group = cg;
       rect_transform = rt;
       canvas = c;
    }

    public CanvasGroup GetCanvasGroup()
    {
        return canvas_group;
    }

    public Canvas GetCanvas()
    {
        return canvas;
    }

    public void ForceRectTransformUpdate()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect_transform);
    }

    [SerializeField]
    private string name_identifier = "no_name";

    private CanvasGroup canvas_group = null;
    private RectTransform rect_transform = null;
    private Canvas canvas = null;
}
