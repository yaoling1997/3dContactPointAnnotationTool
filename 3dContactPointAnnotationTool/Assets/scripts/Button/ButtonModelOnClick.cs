using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonModelOnClick : MonoBehaviour {
    private int status;//0表示未选中，1表示选中
    private Button buttonModel;//选中模型的按钮
    public Color selectedColor;//选中时模型的颜色
    public Color unselectedColor;//未选中时模型的颜色
    private GameObject panelStatus;
    private ObjManager objManager;
    // Use this for initialization
    void Awake () {
        status = 0;
        selectedColor = Color.cyan;//默认选中颜色
        unselectedColor = Color.white;//默认未选中颜色
        buttonModel = gameObject.GetComponent<Button>();
        buttonModel.onClick.AddListener(Click);
	}
    private void Start()
    {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        panelStatus = objManager.panelStatus;
    }
    // Update is called once per frame
    void Update () {
		
	}
    public void ChangeStatus()//反转状态
    {
        var model= GetComponentInParent<ScrollViewItemController>().model; 
        var s = buttonModel.transform.parent.parent.GetComponent<ContentController>();
        status ^= 1;
        if (status == 1)
        {
            s.selectedItem = buttonModel;
            buttonModel.GetComponent<Image>().color = Color.cyan;
            model.GetComponent<SkinnedMeshRenderer>().material.color = selectedColor;
            model.tag = Macro.SELECTED;//设置为已选中
        }
        else
        {
            buttonModel.GetComponent<Image>().color = Color.white;
            model.GetComponent<SkinnedMeshRenderer>().material.color = unselectedColor;
            model.tag = Macro.UNSELECTED;//设置为未选中
        }
    }
    public void Click() {
        panelStatus.GetComponent<PanelStatusController>().SetSelectedObj(GetComponentInParent<ScrollViewItemController>().model);//将该scrollVIew项的model赋值给panelStatus，显示该model信息
        var s = buttonModel.transform.parent.parent.GetComponent<ContentController>();//获得当前item绑定content的脚本
        if (s.selectedItem!=null&&s.selectedItem!= buttonModel)
            s.selectedItem.GetComponent<ButtonModelOnClick>().ChangeStatus();
        else
            s.selectedItem = null;
        ChangeStatus();
    }
}
