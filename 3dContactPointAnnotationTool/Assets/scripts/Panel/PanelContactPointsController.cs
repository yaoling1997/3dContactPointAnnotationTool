using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelContactPointsController : MonoBehaviour {
    private ObjManager objManager;
    public GameObject scrollViewContent;
    void Awake () {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void ButtonClearOnClick()//clear按钮被点击
    {
        foreach (var item in scrollViewContent.GetComponentsInChildren<ScrollViewItemController>())
        {
            item.ButtonDeleteOnClick();
        }
    }
    public void ButtonAddSphereOnClick()
    {
        objManager.CreateContactPointSphere(new Vector3(0, 0, 0), 0.5f);//创建一个原点为中心，半径0.5的球        
    }
}
