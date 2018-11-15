using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCollision : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnCollisionEnter(Collision collision) {
        Debug.Log("OnCollisionEnter");
        int length = collision.contacts.Length;
        foreach (var item in collision.contacts) {
            Debug.Log("x:"+item.point.x+" y:"+ item.point.y+" z:"+ item.point.z);
        }
        Debug.Log("length: "+length);
    }

    void OnCollisionStay(Collision collision) {
        Debug.Log("OnCollisionStay");
    }
    void OnTriggerEnter(Collider collider) {     
        
        //Debug.Log("OnTriggerEnter");
    }
    void OnTriggerStay(Collider collider)
    {
        //Debug.Log("OnTriggerStay");
    }
}
