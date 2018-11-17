using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragPanel : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private Vector2 pointerOffset;
    private RectTransform rectTransformCanvas;
    private RectTransform rectTransformPanel;
    void Awake()
    {
        rectTransformCanvas = GetComponentInParent<Canvas>().transform as RectTransform;
        rectTransformPanel = transform.parent as RectTransform;
    }
    public void OnPointerDown(PointerEventData data)
    {
        rectTransformPanel.SetAsLastSibling();//把该组件放到UI最前面
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransformPanel,data.position,data.pressEventCamera,out pointerOffset);        
    }

    public void OnDrag(PointerEventData data)
    {
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransformCanvas,data.position,data.pressEventCamera,out localPointerPosition))
        {
            rectTransformPanel.localPosition = localPointerPosition - pointerOffset;
        }
    }

}
