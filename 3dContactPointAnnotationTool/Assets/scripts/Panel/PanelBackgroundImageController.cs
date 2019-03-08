using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelBackgroundImageController : MonoBehaviour {
    public InputField inputFieldAlpha;
    public InputField inputFieldScale;

    private ObjManager objManager;
    private GameObject imageBackground;
    void Awake() {
    }
    // Use this for initialization
    void Start () {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        imageBackground = objManager.imageBackground;        
    }
	
	// Update is called once per frame
	void Update () {
        if (objManager.imagePath!=null)
            UpdateInputFieldValues();
    }
    public void UpdateInputFieldValues() {
        if (!inputFieldAlpha.isFocused) {
            inputFieldAlpha.text = imageBackground.GetComponent<Image>().color.a.ToString();//修正inputField的text的值
        }
        if (!inputFieldScale.isFocused) {
            inputFieldScale.text = (imageBackground.GetComponent<RectTransform>().sizeDelta.x / imageBackground.GetComponent<Image>().mainTexture.width).ToString();//修正inputField的text的值
        }
    }
    public void Init()//导入图片调用初始化函数
    {        
        inputFieldAlpha.interactable = true;
        inputFieldScale.interactable = true;
        inputFieldAlpha.text = 1.ToString();
        inputFieldScale.text = 1.ToString();        
    }
    public void AlphaOnValueChanged()
    {
        float v = ObjManager.StringToFloat(inputFieldAlpha.text);
        v = Mathf.Clamp(v,0, 1);
        var image = imageBackground.GetComponent<Image>();
        var color = image.color;
        color.a = v;
        image.color = color;        
    }
    public void ScaleOnValueChanged()
    {
        float v = ObjManager.StringToFloat(inputFieldScale.text);
        v = Mathf.Max(v,0);
        var rt = imageBackground.GetComponent<RectTransform>();
        var texture = imageBackground.GetComponent<Image>().mainTexture;
        //Debug.Log("width:"+texture.width+" height:"+texture.height);
        rt.sizeDelta = new Vector2(texture.width*v, texture.height*v);        
    }
}

