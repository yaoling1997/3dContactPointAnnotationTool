using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBackgroundController : MonoBehaviour {
    private ObjManager objManager;
    private Camera mainCamera;
    // Use this for initialization
    void Start () {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        mainCamera = objManager.mainCamera;
    }
	
	// Update is called once per frame
	void Update () {
        //使渲染背景图的相机和主相机的位置角度一致
        transform.position = mainCamera.transform.position;
        transform.rotation = mainCamera.transform.rotation;
    }
}
