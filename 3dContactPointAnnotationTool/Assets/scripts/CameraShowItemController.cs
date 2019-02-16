using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShowItemController : MonoBehaviour {
    private float rotateAnglePerSec=30;//每秒旋转角度
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(Vector3.zero, Vector3.up, rotateAnglePerSec*Time.deltaTime);        
	}
}
