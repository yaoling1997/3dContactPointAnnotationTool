using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPanelControllerOnClick : MonoBehaviour {
    public GameObject panel;//对应的面板
    private bool active;//对应的面板是否被激活
    private void Start()
    {
        active = true;
        panel.SetActive(active);
        GetComponent<Image>().color = Color.cyan;
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
        }
        panel.SetActive(active);
    }
}
