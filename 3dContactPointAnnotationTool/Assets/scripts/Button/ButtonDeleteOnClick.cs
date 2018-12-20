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
        GetComponentInParent<ScrollViewItemController>().Delete();//删除一系列的scrollView及model
    }
}
