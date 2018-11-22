using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisController : MonoBehaviour {
    public Camera mainCamera;
    private Vector3 originEulerAngles;
    //private Vector3 originCameraPosition;
    //private Vector3 positionOffset;

	// Use this for initialization
	void Start () {
        //var canvasRectTransform = GameObject.Find("Canvas").GetComponent<RectTransform>();        
        originEulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
        //originCameraPosition = new Vector3(cameraTransform.position.x, cameraTransform.position.y, cameraTransform.position.z);
        //positionOffset = transform.position - cameraTransform.position;
	}
	
	// Update is called once per frame
	void Update () {
        //Quaternion rotation = Quaternion.Euler(cameraTransform.eulerAngles);
        //Matrix4x4 m = Matrix4x4.TRS(cameraTransform.position-originCameraPosition,rotation,cameraTransform.localScale);
        //transform.position = m.MultiplyPoint3x4(positionOffset)+originCameraPosition;
        //Debug.Log("1transform.eulerAngles: " + transform.eulerAngles);
        gameObject.transform.eulerAngles = originEulerAngles;
        //Debug.Log("cameraTransform.eulerAngles: " + mainCamera.transform.eulerAngles);
        //Debug.Log("originEulerAngles: " + originEulerAngles);
        //Debug.Log("2transform.eulerAngles: " + transform.eulerAngles);
    }
}
