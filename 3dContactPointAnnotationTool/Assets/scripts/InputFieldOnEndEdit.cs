using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldOnEndEdit : MonoBehaviour {
    //public Text text;
	// Use this for initialization
	void Start () {
        gameObject.GetComponent<InputField>().onEndEdit.AddListener(delegate { OnEndEdit(); });
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void OnEndEdit()
    {
        Debug.Log("OnEndEdit");
        Debug.Log(gameObject.GetComponent<InputField>().text);
        float v = float.Parse(gameObject.GetComponent<InputField>().text);
        var obj= GameObject.Find("ObjManager").GetComponent<ObjManager>().panelStatus.GetComponent<PanelStatusController>().selectedObj;
        if (obj == null)//对象为空
            return;
        var p = obj.transform.position;
        var r = obj.transform.eulerAngles;
        var s = obj.transform.localScale;
        Debug.Log("gameObject.transform.name: "+gameObject.transform.name);
        Debug.Log("transform.name: " + transform.name);
        if (transform.parent.name.Equals("PanelStatusItemPosX"))
        {
            p.x = v;
            obj.transform.position = p;
        }
        else if (transform.parent.name.Equals("PanelStatusItemPosY"))
        {
            p.y = v;
            obj.transform.position = p;
        }
        else if (transform.parent.name.Equals("PanelStatusItemPosZ"))
        {
            p.z = v;
            obj.transform.position = p;
        }
        else if (transform.parent.name.Equals("PanelStatusItemRotX"))
        {
            r.x = v;
            obj.transform.eulerAngles = r;
        }
        else if (transform.parent.name.Equals("PanelStatusItemRotY"))
        {
            r.y = v;
            obj.transform.eulerAngles = r;
        }
        else if (transform.parent.name.Equals("PanelStatusItemRotZ"))
        {
            r.z = v;
            obj.transform.eulerAngles = r;
        }
        else if (transform.parent.name.Equals("PanelStatusItemScaleX"))
        {
            s.x = v;
            obj.transform.localScale = s;
        }
        else if (transform.parent.name.Equals("PanelStatusItemScaleY"))
        {
            s.y = v;
            obj.transform.localScale = s;
        }
        else if (transform.parent.name.Equals("PanelStatusItemScaleZ"))
        {
            s.z = v;
            obj.transform.localScale = s;
        }
        else
        {
            Debug.Log("statusPanelController:can not find correct parent!");
        }
    }
}
