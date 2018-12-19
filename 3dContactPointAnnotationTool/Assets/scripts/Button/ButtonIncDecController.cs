using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonIncDecController : MonoBehaviour {
    public InputField inputField;
    public float speed = 0.1f;
    void Awake()
    {
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}
    public void OnClick()
    {
        float delta = speed;
        //Debug.Log("name:"+transform.name);
        if (transform.name.Equals("ButtonDec"))
            delta = -speed;
        if (inputField.interactable)
            inputField.text = (float.Parse(inputField.text) + delta).ToString();
    }
}
