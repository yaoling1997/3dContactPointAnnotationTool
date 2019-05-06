using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawMapController : MonoBehaviour {
    private ObjManager objManager;
    private ImageDrawMapController imageDrawMapController;
    public Button buttonIfActive;
    public Button buttonAreaId;
    public Text textIoU;
    void Awake()
    {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        imageDrawMapController = objManager.imageDrawMapController;
        UpdateButtonIfActive();
        UpdateButtonAreaId();
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void UpdateButtonIfActive() {
        if (imageDrawMapController.ifActive)
        {
            buttonIfActive.GetComponentInChildren<Text>().text = "Active";
        }
        else {
            buttonIfActive.GetComponentInChildren<Text>().text = "InActive";
        }
    }
    public void UpdateButtonAreaId()
    {
        buttonAreaId.GetComponentInChildren<Text>().text="id="+ imageDrawMapController.areaId;
    }
    public void ButtonIfActiveOnClick() {
        imageDrawMapController.ifActive = !imageDrawMapController.ifActive;
        UpdateButtonIfActive();
    }
    public void ButtonFillOnClick()
    {
        if (imageDrawMapController.ifActive)
            imageDrawMapController.FillDrawMap();
    }
    public void ButtonClearOnClick()
    {
        if (imageDrawMapController.ifActive)
            imageDrawMapController.ClearNowDrawMap();
    }
    public void ButtonAreaIdOnClick()
    {
        if (imageDrawMapController.ifActive) {
            imageDrawMapController.ChangeChosenId();
            UpdateButtonAreaId();
        }            
    }
    public void ButtonGetIoUOnClick()
    {
        if (imageDrawMapController.ifActive)
        {
            var ans = imageDrawMapController.GetIoU();
            Debug.Log("ans: "+ans);
            textIoU.text = "IoU=" + ans.ToString("0.0000");//System.Math.Round(ans, 5).ToString();
        }
    }
    public void BuutonProjectSelectOnClick() {
        if (imageDrawMapController.ifActive) {
            imageDrawMapController.ProjectSelect();
        }
    }
}
