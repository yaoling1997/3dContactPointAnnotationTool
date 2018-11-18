using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelStatusController : MonoBehaviour {
    public GameObject selectedObj;//当前选中的对象
    // Use this for initialization
    void Awake () {
        selectedObj = null;
    }
	
	// Update is called once per frame
	void Update () {
    }
    public void SetSelectedObj(GameObject obj)
    {
        selectedObj = obj;
        foreach(var item in gameObject.GetComponentsInChildren<InputField>())
        {
            item.interactable = true;
            if (item.transform.parent.name.Equals("PanelStatusItemPosX"))
            {                
                item.text = obj.transform.position.x.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemPosY"))
            {
                item.text = obj.transform.position.y.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemPosZ"))
            {
                item.text = obj.transform.position.z.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemRotX"))
            {
                item.text = obj.transform.eulerAngles.x.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemRotY"))
            {
                item.text = obj.transform.eulerAngles.y.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemRotZ"))
            {
                item.text = obj.transform.eulerAngles.z.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemScaleX"))
            {
                item.text = obj.transform.localScale.x.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemScaleY"))
            {
                item.text = obj.transform.localScale.y.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemScaleZ"))
            {
                item.text = obj.transform.localScale.z.ToString();
            }else
            {
                Debug.Log("statusPanelController:can not find correct parent!");
            }            
        }
    }
}
