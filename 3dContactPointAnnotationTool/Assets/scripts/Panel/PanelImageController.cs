using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelImageController : MonoBehaviour {
    private Vector2 size;//初始size
    private GameObject image;//image
    // Use this for initialization
    void Start () {
        size = GetComponent<RectTransform>().sizeDelta;
        image = transform.Find("Image").gameObject;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void SetScale(float scale)
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(size.x * scale, size.y * scale);
        image.GetComponent<ImageController>().SetScale(scale);
        UpdateSize();
    }
    public void UpdateSize()
    {        
        var tmpy = Mathf.Max(
            image.GetComponent<RectTransform>().sizeDelta.y, 
            transform.Find("PanelController").GetComponent<RectTransform>().sizeDelta.y);
        var v = new Vector2(transform.Find("Image").GetComponent<RectTransform>().sizeDelta.x
            + transform.Find("PanelController").GetComponent<RectTransform>().sizeDelta.x
            +2,//间隔
            tmpy+transform.Find("Text").GetComponent<RectTransform>().sizeDelta.y);
        GetComponent<RectTransform>().sizeDelta = v;
    }
    public void ButtonClearContactPoints2dOnClick()//清除2d接触点
    {
        foreach(var item in image.GetComponentsInChildren<Transform>())
        {
            if (item.name.Equals(image.name))
                continue;
            Destroy(item.gameObject);
        }
    }
}
