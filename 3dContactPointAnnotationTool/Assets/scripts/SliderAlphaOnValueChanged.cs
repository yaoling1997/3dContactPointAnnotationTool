using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderAlphaOnValueChanged : MonoBehaviour {
    public Image image;
    public void OnValueChanged()
    {        
        var color = image.color;
        color.a = GetComponent<Slider>().value;
        image.color = color;
    }
}
