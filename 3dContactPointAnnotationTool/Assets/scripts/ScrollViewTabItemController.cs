using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollViewTabItemController : MonoBehaviour {
    private ObjManager objManager;
    private PanelItemWarehouseController panelItemWarehouseController;
	// Use this for initialization
	void Start () {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        panelItemWarehouseController = objManager.panelItemWareHouseController;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void OnClick()//被点击
    {
        panelItemWarehouseController.SwitchTab(gameObject);
    }
}
