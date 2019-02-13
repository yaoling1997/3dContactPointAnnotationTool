using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentController : MonoBehaviour {
    
	// Use this for initialization
	void Start () {        

    }
	
	// Update is called once per frame
	void Update () {
    }
    public void Add(GameObject item)//将Item添加到scrollView里
    {
        item.transform.SetParent(transform);
        //var size=GetComponent<RectTransform>().sizeDelta;
        //GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, size.y + item.GetComponent<RectTransform>().sizeDelta.y);//更新content的高度
    }
}
