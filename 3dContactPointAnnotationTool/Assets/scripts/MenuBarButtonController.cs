using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuBarButtonController : MonoBehaviour {
    private ObjManager objManager;
    private MenuBarController menuBarController;

    public GameObject panel;
	void Awake () {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        menuBarController = objManager.menuBar.GetComponent<MenuBarController>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void OnPointerEnter() {//鼠标指针进入
        if (menuBarController.IfMenuBarButtonSelected()) {
            menuBarController.eventSystem.SetSelectedGameObject(gameObject);
            panel.SetActive(true);
        }
    }
}
