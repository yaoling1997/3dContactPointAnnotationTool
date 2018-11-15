using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewItemOnClick : MonoBehaviour {
    private int status;//0表示未选中，1表示选中
    private Button scrollViewItem;
    public GameObject model3d;
	// Use this for initialization
	void Start () {
        status = 0;
        scrollViewItem = transform.gameObject.GetComponent<Button>();
        scrollViewItem.onClick.AddListener(Click);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Click() {
        status ^= 1;
        if (status == 1) { 
            scrollViewItem.GetComponent<Image>().color = Color.cyan;
            model3d.GetComponent<MeshRenderer>().material.color = Color.cyan;
        }
        else { 
            scrollViewItem.GetComponent<Image>().color = Color.white;
            model3d.GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }
}
