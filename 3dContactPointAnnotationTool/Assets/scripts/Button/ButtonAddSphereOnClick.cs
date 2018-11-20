using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAddSphereOnClick : MonoBehaviour {
    private GameObject objManager;
	// Use this for initialization
	void Start () {
        objManager = GameObject.Find("ObjManager");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClick()
    {
        objManager.GetComponent<ObjManager>().CreateContactPointSphere(new Vector3(0,0,0),0.5f);        
    }
}
