using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPanelControllerOnClick : MonoBehaviour {
    public GameObject panel;//对应的面板或物体
    private bool active;//对应的面板是否被激活
    private Vector2 localPosition;
    private RectTransform panelRectTransform;
    private void Start()
    {
        active = true;
        panel.SetActive(active);
        GetComponent<Image>().color = Color.cyan;
        panelRectTransform = panel.GetComponent<RectTransform>();
        if (panelRectTransform != null) {
            var lp = panelRectTransform.localPosition;
            localPosition = new Vector2(lp.x,lp.y);
        }
    }
    public void OnClick()
    {
        active=!active;
        if (active)
        {
            GetComponent<Image>().color = Color.cyan;
        }
        else {
            GetComponent<Image>().color = Color.white;
            if (panelRectTransform != null) {
                panel.GetComponent<RectTransform>().localPosition = localPosition;
            }
        }
        panel.SetActive(active);        
    }
}
