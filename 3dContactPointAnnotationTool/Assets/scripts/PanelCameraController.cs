using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelCameraController : MonoBehaviour {
    public float speed = 0.1f;
    private ObjManager objManager;
    private bool ifPointerDown;//鼠标是否按下
    enum Orient {Forward,Back,Left,Right,Up,Down };
    private Orient moveStatus;//移动状态
    void Awake()
    {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        ifPointerDown = false;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (ifPointerDown)//鼠标按在按钮上
        {
            //Debug.Log("name:"+transform.name);
            var t = objManager.mainCamera.transform;
            switch (moveStatus) {
                case Orient.Forward:
                    t.position = t.position + t.forward * speed;
                    break;
                case Orient.Back:
                    t.position = t.position + t.forward * -speed;
                    break;
                case Orient.Left:
                    t.position = t.position + t.right * -speed;
                    break;
                case Orient.Right:
                    t.position = t.position + t.right * speed;
                    break;
                case Orient.Up:
                    t.position = t.position + t.up * speed;
                    break;
                case Orient.Down:
                    t.position = t.position + t.up * -speed;
                    break;
            }
        }
    }
    public void PointerForwardDown()
    {
        moveStatus = Orient.Forward;
        ifPointerDown = true;
    }
    public void PointerBackDown()
    {
        moveStatus = Orient.Back;
        ifPointerDown = true;
    }
    public void PointerLeftDown()
    {
        moveStatus = Orient.Left;
        ifPointerDown = true;
    }
    public void PointerRightDown()
    {
        moveStatus = Orient.Right;
        ifPointerDown = true;
    }
    public void PointerUpDown()
    {
        moveStatus = Orient.Up;
        ifPointerDown = true;
    }
    public void PointerDownDown()
    {
        moveStatus = Orient.Down;
        ifPointerDown = true;
    }
    public void PointerUp()
    {
        ifPointerDown = false;
    }
}