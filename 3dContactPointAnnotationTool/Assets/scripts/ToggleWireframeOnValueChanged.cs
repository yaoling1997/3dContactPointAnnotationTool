using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleWireframeOnValueChanged : MonoBehaviour {
    public GameObject model3d;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ToggleValueChanged() {        
        var isOn = this.gameObject.GetComponent<Toggle>().isOn;
        Debug.Log("ToggleValueChanged:" + isOn);
        foreach (var item in model3d.GetComponentsInChildren<MeshRenderer>()) {
            var color = item.material.color;
            if (isOn) {
                item.material = new Material(Shader.Find("UCLA Game Lab/Wireframe/Single-Sided"));
            }
            else {
                item.material = new Material(Shader.Find("Standard"));
            }
            item.material.color = color;
        }
    }
}
