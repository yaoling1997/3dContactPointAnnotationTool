using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewItemController : MonoBehaviour {
    public GameObject model;//scrollViewItem对应的model
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Init(GameObject item,GameObject scrollViewModelsContent)//初始化scrollViewItem
    {
        var buttonModel = transform.Find("ButtonModel");        
        buttonModel.GetComponentInChildren<Text>().text = item.name;
        model = item;//将模型赋值给item的脚本        
        scrollViewModelsContent.GetComponent<ContentController>().add(gameObject);//将scrollViewItem添加到scrollView里                                
    }
    public void Init(GameObject item, GameObject scrollViewModelsContent,Color color)//初始化scrollViewItem
    {
        Init(item, scrollViewModelsContent);
        var buttonModel = transform.Find("ButtonModel");
        buttonModel.GetComponent<ButtonModelOnClick>().unselectedColor = color;
    }

}
