using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonIncDecController : MonoBehaviour {
    public InputField inputField;
    public float speed = 0.1f;
    private bool ifPointerDown;//鼠标是否按下
    void Awake()
    {
        ifPointerDown = false;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (ifPointerDown)//鼠标按在按钮上
        {
            float delta = speed;
            //Debug.Log("name:"+transform.name);
            if (transform.name.Equals("ButtonDec"))
                delta = -speed;
            if (inputField.interactable)
                inputField.text = (float.Parse(inputField.text) + delta).ToString();
        }
    }
    public void PointerDown()
    {
        ifPointerDown = true;
    }
    public void PointerUp()
    {
        ifPointerDown = false;
    }
}
