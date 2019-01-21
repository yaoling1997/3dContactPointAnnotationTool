using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldOnValueChanged : MonoBehaviour {
    //public Text text;
    // Use this for initialization
    private ObjManager objManager;
    private InputField inputField;
	void Start () {
        objManager= GameObject.Find("ObjManager").GetComponent<ObjManager>();
        inputField = gameObject.GetComponent<InputField>();
        inputField.text = 0.ToString();
        inputField.onValueChanged.AddListener(delegate { OnValueChanged(); });
	}
	
	// Update is called once per frame
	void Update () {        

    }
    public void OnValueChanged()
    {
        float v = float.Parse(gameObject.GetComponent<InputField>().text);
        var obj = objManager.panelStatus.GetComponentInChildren<PanelStatusController>().selectedObj;//获得选中对象
        if (obj == null)//对象为空
            return;
        var p = obj.transform.position;
        var r = obj.transform.eulerAngles;
        var s = obj.transform.localScale;
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
