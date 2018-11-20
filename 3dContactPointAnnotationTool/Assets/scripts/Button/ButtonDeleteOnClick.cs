using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDeleteOnClick : MonoBehaviour {    
        
    // Use this for initialization
    void Start () {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void OnClick()
    {
        Destroy(GetComponentInParent<ScrollViewItemController>().model);//删除item对应模型
        Destroy(transform.parent.gameObject);//删除ScrollViewItem
    }
}
