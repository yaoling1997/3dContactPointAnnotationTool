using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelImageController : MonoBehaviour {
    private Vector2 size;//初始size

    // Use this for initialization
    void Start () {
        size = GetComponent<RectTransform>().sizeDelta;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void setScale(float scale)
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(size.x * scale, size.y * scale);
        transform.Find("Image").GetComponent<ImageController>().SetScale(scale);
        updateSize();
    }
    public void updateSize()
    {        
        var tmpy = Mathf.Max(
            transform.Find("Image").GetComponent<RectTransform>().sizeDelta.y, 
            transform.Find("PanelController").GetComponent<RectTransform>().sizeDelta.y);
        var v = new Vector2(transform.Find("Image").GetComponent<RectTransform>().sizeDelta.x
            + transform.Find("PanelController").GetComponent<RectTransform>().sizeDelta.x
            +2,//间隔
            tmpy+transform.Find("Text").GetComponent<RectTransform>().sizeDelta.y);
        GetComponent<RectTransform>().sizeDelta = v;
    }
}
