using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusPanelController : MonoBehaviour {
    public GameObject selectedObj;//当前选中的对象
    public InputField inputFieldX;//x
    public InputField inputFieldY;//y
    public InputField inputFieldZ;//z
    // Use this for initialization
    void Awake () {
        selectedObj = null;
    }
	
	// Update is called once per frame
	void Update () {
		if (selectedObj == null)
        {
            //Debug.Log("null");
            inputFieldX.interactable = false;
            inputFieldY.interactable = false;
            inputFieldZ.interactable = false;
        }
        else
        {
            //Debug.Log(selectedObj.name);
            inputFieldX.interactable = true;
            inputFieldY.interactable = true;
            inputFieldZ.interactable = true;
            inputFieldX.GetComponentInChildren<Text>().text = selectedObj.transform.position.x.ToString();
            inputFieldY.GetComponentInChildren<Text>().text = selectedObj.transform.position.y.ToString();
            inputFieldZ.GetComponentInChildren<Text>().text = selectedObj.transform.position.z.ToString();
        }
        
    }
}
