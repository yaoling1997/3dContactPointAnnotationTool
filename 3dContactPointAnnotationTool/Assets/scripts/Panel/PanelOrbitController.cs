using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RTEditor;

public class PanelOrbitController : MonoBehaviour {
    public GameObject buttonOrbit;
    public GameObject buttonOrbitSelected;

    private ObjManager objManager;
    private ToolBarController toolBarController;
    private GameObject inactivateButton;//记录未激活的按钮
    void Awake() {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        toolBarController = objManager.toolBar.GetComponent<ToolBarController>();
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void PointerDown() {
        Debug.Log("buttonOrbit activeSelf: " + buttonOrbit.activeSelf);
        Debug.Log("buttonOrbitSelected activeSelf: " + buttonOrbitSelected.activeSelf);
        inactivateButton = buttonOrbit.activeSelf ? buttonOrbitSelected : buttonOrbit;
        inactivateButton.SetActive(true);
    }
    public void PointerUp()
    {
        Vector2 inputDevPos;
        if (!InputDevice.Instance.GetPosition(out inputDevPos)) return ;
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(inputDevPos.x, inputDevPos.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        foreach (var item in results) {
            if (item.gameObject.Equals(buttonOrbit))//鼠标抬起时指针在buttonOrbit上
            {
                toolBarController.ButtonOrbitOnClick();
                buttonOrbitSelected.SetActive(false);                
                return;
            } else if (item.gameObject.Equals(buttonOrbitSelected))//鼠标抬起时指针在buttonOrbitSelected上
            {
                toolBarController.ButtonOrbitSelectedOnClick();
                buttonOrbit.SetActive(false);
                return;
            }
        }
        inactivateButton.SetActive(false);
    }
}
