﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewItemOnClick : MonoBehaviour {
    private int status;//0表示未选中，1表示选中
    private Button scrollViewItem;
    public GameObject model;//按钮对应的模型或接触点
	// Use this for initialization
	void Start () {
        status = 0;
        scrollViewItem = transform.gameObject.GetComponent<Button>();
        scrollViewItem.onClick.AddListener(Click);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void ChangeStatus()//反转状态
    {
        var s = scrollViewItem.transform.parent.GetComponent<ContentSelectedItem>();
        status ^= 1;
        if (status == 1)
        {
            s.selectedItem = scrollViewItem;
            scrollViewItem.GetComponent<Image>().color = Color.cyan;
            model.GetComponent<MeshRenderer>().material.color = Color.cyan;
            model.tag = Macro.SELECTED;//设置为已选中
        }
        else
        {
            scrollViewItem.GetComponent<Image>().color = Color.white;
            model.GetComponent<MeshRenderer>().material.color = Color.white;
            model.tag = Macro.UNSELECTED;//设置为未选中
        }
    }
    public void Click() {
        var s = scrollViewItem.transform.parent.GetComponent<ContentSelectedItem>();//获得当前item绑定content的脚本
        if (s.selectedItem!=null&&s.selectedItem!= scrollViewItem)
            s.selectedItem.GetComponent<ScrollViewItemOnClick>().ChangeStatus();
        else
            s.selectedItem = null;
        ChangeStatus();
    }
}
