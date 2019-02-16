using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollViewItemsItemController : MonoBehaviour {
    public const float MIN_CLICK_TIME_GAP = 0.2f;
    public string objAbsolutePath;//obj文件的绝对路径

    private ObjManager objManager;
    private PanelItemWarehouseController panelItemWarehouseController;
    private float timeLastClick;//最后一次被点击的时间
    // Use this for initialization
    void Awake () {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        panelItemWarehouseController = objManager.panelItemWareHouseController;
        timeLastClick = 0;
        objAbsolutePath = "";        
    }

    // Update is called once per frame
    void Update () {
		
	}
    public void OnClick()//被点击
    {
        panelItemWarehouseController.SwitchItemsItem(gameObject);
        var timeNow = Time.realtimeSinceStartup;
        if (timeNow-timeLastClick< MIN_CLICK_TIME_GAP)//判定为双击
        {
            objManager.LoadObj(objAbsolutePath);
        }
        timeLastClick = timeNow;
    }
}
