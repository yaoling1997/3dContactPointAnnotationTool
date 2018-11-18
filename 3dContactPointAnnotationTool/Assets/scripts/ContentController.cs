using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentController : MonoBehaviour {
    public Button selectedItem;
	// Use this for initialization
	void Start () {
        selectedItem = null;

    }
	
	// Update is called once per frame
	void Update () {
    }
    public void add(GameObject item)//将Item添加到scrollView里
    {
        item.transform.SetParent(transform);
        var size=GetComponent<RectTransform>().sizeDelta;
        GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, size.y + item.GetComponent<RectTransform>().sizeDelta.y);
    }
}
