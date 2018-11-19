﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public GameObject model3d;
    public float scrollSpeed = 1;
    public float rotateSpeed = 1;
    public float minDist = 1;
    public float maxDist = 3000;
    private bool isRotating = false;//是否正在旋转
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {                
        ScrollView();
        RotateView();        
        transform.LookAt(model3d.transform);//使相机对准3dModel
    }

    private void ScrollView() {//鼠标滚轮实现放大缩小视角功能
        Vector3 offset = transform.position - model3d.transform.position;
        float dist = offset.magnitude;//获取相机与3d模型的距离
        dist = dist - Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        dist = Mathf.Clamp(dist,minDist,maxDist);
        offset = offset.normalized * dist;
        transform.position = model3d.transform.position + offset;
    }

    private void RotateView() {//鼠标右键实现旋转视角功能        
        if (Input.GetMouseButtonDown(1))//0左键、1右键、2中键
            isRotating = true;
        else if (Input.GetMouseButtonUp(1))
            isRotating = false;
        //Debug.Log(isRotating);
        if (isRotating) {
            transform.RotateAround(model3d.transform.position,transform.up,rotateSpeed*Input.GetAxis("Mouse X"));
            transform.RotateAround(model3d.transform.position, transform.right, -rotateSpeed * Input.GetAxis("Mouse Y"));
        }        
    }

}