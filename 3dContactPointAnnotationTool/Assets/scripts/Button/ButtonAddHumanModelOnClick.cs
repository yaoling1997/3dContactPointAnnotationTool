﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAddHumanModelOnClick : MonoBehaviour {
    public GameObject scrollViewModelsContent;//scrollViewModels的content
    private ObjManager objManager;
	// Use this for initialization
	void Start () {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void OnClick()//点击添加一个人物模型
    {
        objManager.humanModelId++;
        var item = Instantiate(objManager.humanModel, new Vector3(0, 0, 0), Quaternion.identity);//实例化一个人物模型
        item.name = "SMPL"+objManager.humanModelId;
        var prefabScrollViewItem = objManager.prefabScrollViewItem;
        var model3d = objManager.model3d;
        model3d.GetComponent<Model3dController>().AddSon(item);//将解析出来的obj的父亲设置为model3d
        item.tag = Macro.UNSELECTED;//将tag设置为未选中
        item.AddComponent<Model3dItemController>();//添加该脚本
        var scrollViewItem = Instantiate(prefabScrollViewItem, new Vector3(0, 0, 0), Quaternion.identity);
        scrollViewItem.GetComponent<ScrollViewItemController>().Init(item, scrollViewModelsContent);
        var par = scrollViewItem;
        foreach (var o in item.GetComponentsInChildren<Transform>())
        {
            if (o.gameObject == par.GetComponent<ScrollViewItemController>().model)
                continue;
            Debug.Log(o.name);
            var svi = Instantiate(prefabScrollViewItem, new Vector3(0, 0, 0), Quaternion.identity);
            svi.GetComponent<ScrollViewItemController>().Init(o.gameObject, scrollViewModelsContent);
            while (par.GetComponent<ScrollViewItemController>().model != o.parent.gameObject)
            {
                Debug.Log("o name:" + o.gameObject.name);
                par = par.GetComponent<ScrollViewItemController>().par;
            }
            par.GetComponent<ScrollViewItemController>().AddSon(svi);
            par = svi;
        }
    }
}