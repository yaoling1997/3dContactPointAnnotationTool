using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderScaleOnValueChanged : MonoBehaviour {
    public GameObject panelImage;
    public void OnValueChanged()
    {
        var value= GetComponent<Slider>().value*3;//最多放大3倍
        panelImage.GetComponent<PanelImageController>().setScale(value+1);
    }
}
