using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelBackgroundImageController : MonoBehaviour {
    public InputField inputFieldAlpha;
    public InputField inputFieldScale;
    public Button buttonOverlay;
    public Button buttonClear;

    private ObjManager objManager;
    private GameObject imageBackground;
    private Texture2D imageTexture;
    private GameObject contactPoints;//3d接触点
    private Camera mainCamera;//主相机
    // Use this for initialization
    void Start () {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        imageBackground = objManager.imageBackground;
        contactPoints = objManager.contactPoints;
        mainCamera = objManager.mainCamera;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Init(Texture2D texture)//导入图片调用初始化函数
    {
        imageTexture = texture;
        inputFieldAlpha.interactable = true;
        inputFieldScale.interactable = true;
        buttonOverlay.interactable = true;
        buttonClear.interactable = true;
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
        rt.sizeDelta = new Vector2(imageTexture.width*v,imageTexture.height*v);
    }
    public void ButtonOverlayOnClick()
    {

    }
    public void ButtonClearOnClick()
    {

    }
}

