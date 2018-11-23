﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClearContactPointsOnClick : MonoBehaviour {
    public GameObject scrollViewContent;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void OnClick()
    {
        foreach(var item in scrollViewContent.GetComponentsInChildren<ButtonDeleteOnClick>())
        {
            item.OnClick();
        }
    }
}
