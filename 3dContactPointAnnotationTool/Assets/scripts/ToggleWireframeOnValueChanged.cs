using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleWireframeOnValueChanged : MonoBehaviour {
    private GameObject model3d;
    private ObjManager objManager;
	// Use this for initialization
	void Start () {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        model3d = objManager.model3d;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ToggleValueChanged() {        
        var isOn = this.gameObject.GetComponent<Toggle>().isOn;
        Debug.Log("ToggleValueChanged:" + isOn);
        foreach (var item in model3d.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            var color = item.material.color;
            if (isOn)
            {
                item.material = new Material(objManager.materialWireframe);
            }
            else
            {
                item.material = new Material(objManager.materialStandard);
            }
            item.material.color = color;
        }
    }
}
