using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelBackgroundImageController : MonoBehaviour {
    public InputField inputFieldAlpha;
    public InputField inputFieldScale;

    private ObjManager objManager;
    private GameObject imageBackground;
    

    // Use this for initialization
    void Start () {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        imageBackground = objManager.imageBackground;        
    }
	
	// Update is called once per frame
	void Update () {
		
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
        float v = float.Parse(inputFieldAlpha.text);
        var image = imageBackground.GetComponent<Image>();
        var color = image.color;
        color.a = v;
        image.color = color;
    }
    public void ScaleOnValueChanged()
    {
        float v = float.Parse(inputFieldScale.text);
        var rt = imageBackground.GetComponent<RectTransform>();
        var texture = imageBackground.GetComponent<Image>().mainTexture;
        Debug.Log("width:"+texture.width+" height:"+texture.height);
        rt.sizeDelta = new Vector2(texture.width*v, texture.height*v);
    }
    public void ButtonOverlayOnClick()
    {

    }
    public void ButtonClearOnClick()
    {

    }
}

