using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelBackgroundImageController : MonoBehaviour {
    public InputField inputFieldAlpha;
    public InputField inputFieldScale;

    private ObjManager objManager;
    private GameObject imageBackground;
    private bool changeAlpha;//alpha是否正在被改变
    private bool changeScale;//scale是否正在被改变
    void Awake() {
        changeAlpha = false;
        changeScale = false;
    }
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
        if (changeAlpha)//避免修改text的时候该函数被反复调用
            return;
        changeAlpha = true;
        float v = float.Parse(inputFieldAlpha.text);
        v = Mathf.Clamp(v,0, 1);
        var image = imageBackground.GetComponent<Image>();
        var color = image.color;
        color.a = v;
        image.color = color;
        inputFieldAlpha.text = v.ToString();//修正inputField的text的值
        changeAlpha = false;
    }
    public void ScaleOnValueChanged()
    {
        if (changeScale)//避免修改text的时候该函数被反复调用
            return;
        changeScale = true;
        float v = float.Parse(inputFieldScale.text);
        v = Mathf.Max(v,0);
        var rt = imageBackground.GetComponent<RectTransform>();
        var texture = imageBackground.GetComponent<Image>().mainTexture;
        //Debug.Log("width:"+texture.width+" height:"+texture.height);
        rt.sizeDelta = new Vector2(texture.width*v, texture.height*v);
        inputFieldScale.text = v.ToString();//修正inputField的text的值
        changeScale = false;
    }
}

