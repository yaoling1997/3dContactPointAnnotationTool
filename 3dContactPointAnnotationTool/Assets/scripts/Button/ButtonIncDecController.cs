using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonIncDecController : MonoBehaviour {
    public InputField inputField;
    public float speed = 0.1f;
    private ObjManager objManager;
    private bool ifPointerDown;//鼠标是否按下
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
        objManager.editorGizmoSystem.TranslationGizmo.StorePreTransform();//随便用哪个非null的gizmo都可以，因为都是存的position,rotation和localscale的信息，但是要保证前后一致
    }
    public void PointerUp()
    {
        ifPointerDown = false;
        objManager.editorGizmoSystem.TranslationGizmo.StoreObjectsTransform();
    }
}
