using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyController : MonoBehaviour {
    public string itemWarehousePath;
    void Awake() {
        itemWarehousePath = null;
        //Debug.Log("DontDestroy.Awake()");
    }
	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
