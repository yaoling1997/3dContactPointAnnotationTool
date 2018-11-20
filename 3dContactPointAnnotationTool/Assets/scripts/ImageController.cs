using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageController : MonoBehaviour {
    public GameObject panelController;
    private Vector2 size;//初始size

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void SetImage(Texture2D texture)
    {
        var rectTransform = GetComponent<RectTransform>();
        var scale = Mathf.Min(rectTransform.sizeDelta.x/texture.width,rectTransform.sizeDelta.y/texture.height);
        size= new Vector2(texture.width * scale, texture.height * scale);
        rectTransform.sizeDelta = size;
        var imgTmp = GetComponent<Image>();
        imgTmp.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        var color = imgTmp.color;
        color.a = 1;
        imgTmp.color = color;

        var sliderAlpha= panelController.transform.Find("SliderAlpha").GetComponent<Slider>();//开启sliderAlpha
        sliderAlpha.interactable = true;
        sliderAlpha.value = 1;
        var sliderScale = panelController.transform.Find("SliderScale").GetComponent<Slider>();//开启sliderScale
        sliderScale.interactable = true;
        var buttonOverlay= panelController.transform.Find("ButtonOverlay").GetComponent<Button>();//开启buttonOverlay
        buttonOverlay.interactable = true;
        GetComponentInParent<PanelImageController>().updateSize();
    }
    public void SetScale(float scale)
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(size.x * scale, size.y * scale);
    }
}
