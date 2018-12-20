using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model3dController : MonoBehaviour {
    private ObjManager objManager;
    private int sonNum;//儿子数
    
	// Use this for initialization
	void Start () {
        sonNum = 0;
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void AddSon(GameObject item)//将给定GameObject设为其儿子
    {
        item.transform.SetParent(transform);
        sonNum++;
        if (sonNum == 1)
        {
            objManager.buttonGCP.interactable = true;
            objManager.toggleWireframe.interactable = true;
        }
    }
    public void RemoveSon()
    {
        sonNum--;
        if (sonNum == 0)//没有模型了
        {
            objManager.buttonGCP.interactable = false;
            objManager.toggleWireframe.interactable = false;
        }
    }
}
