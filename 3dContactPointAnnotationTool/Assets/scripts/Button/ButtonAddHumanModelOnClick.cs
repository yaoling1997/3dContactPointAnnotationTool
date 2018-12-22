using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        item.GetComponent<Model3dItemController>().trianglesEditable = false;//禁用三角形编辑
        item.GetComponent<Model3dItemController>().scaleEditable = false;//禁用scale编辑

        var scrollViewItem = Instantiate(prefabScrollViewItem, new Vector3(0, 0, 0), Quaternion.identity);
        scrollViewItem.GetComponent<ScrollViewItemController>().Init(item, scrollViewModelsContent);
        var par = scrollViewItem;
        foreach (var o in item.GetComponentsInChildren<Transform>())
        {
            if (o.gameObject == par.GetComponent<ScrollViewItemController>().model)
                continue;

            var svi = Instantiate(prefabScrollViewItem, new Vector3(0, 0, 0), Quaternion.identity);//创建对应的scrollViewItem
            svi.GetComponent<ScrollViewItemController>().Init(o.gameObject, scrollViewModelsContent);
            svi.transform.Find("ButtonDelete").gameObject.SetActive(false);//儿子们的delete按钮都不显示，不能使用
            while (par.GetComponent<ScrollViewItemController>().model != o.parent.gameObject)
            {
                par = par.GetComponent<ScrollViewItemController>().par;
            }
            par.GetComponent<ScrollViewItemController>().AddSon(svi);
            par = svi;

            Debug.Log(o.name);
            var model3dItemController = o.gameObject.AddComponent<Model3dItemController>();
            if (!o.name.Equals("m_avg_root"))//不是m_avg_root
            {                
                model3dItemController.trianglesEditable = false;//禁用三角形编辑
                model3dItemController.positionEditable = false;//禁用坐标编辑
                model3dItemController.scaleEditable = false;//禁用scale编辑
            }
        }
    }
}
