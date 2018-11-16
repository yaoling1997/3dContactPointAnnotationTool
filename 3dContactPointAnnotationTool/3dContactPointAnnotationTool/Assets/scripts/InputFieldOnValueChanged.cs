using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldOnValueChanged : MonoBehaviour {
    public Text text;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void OnValueChanged()
    {
        Debug.Log("OnValueChanged");
        float v = float.Parse(text.text);
        var obj= gameObject.transform.parent.GetComponent<StatusPanelController>().selectedObj;
        if (obj == null)//对象为空
            return;
        var p = obj.transform.position;
        if (gameObject.name.Equals("InputFieldX"))
        {
            p.x = v;
            obj.transform.position = p;
        }
        else if (gameObject.name.Equals("InputFieldY"))
        {
            p.y = v;
            obj.transform.position = p;
        }
        else if (gameObject.name.Equals("InputFieldZ"))
        {
            p.z = v;
            obj.transform.position = p;
        }
    }
}
