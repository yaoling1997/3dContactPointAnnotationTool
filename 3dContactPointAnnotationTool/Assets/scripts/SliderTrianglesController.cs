using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderTrianglesController : MonoBehaviour {
    public Text textTriangles;
    public PanelStatusController panelStatusController;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void OnValueChanged()
    {
        var triangleNum = ValueToTriangleNum(GetComponent<Slider>().value);
        textTriangles.text = "Triangles: x" + System.Math.Round(triangleNum, 2);
        var obj = panelStatusController.selectedObj;
        if (obj != null && obj.GetComponent<Model3dItemController>()!=null) {
            obj.GetComponent<Model3dItemController>().SetTriangleMultiNum(triangleNum);
        }
    }
    public float ValueToTriangleNum(float v)//slider的value转化为三角形倍数
    {
        return 1 + v / 2;
    }
    public float TriangleNumToValue(float n)//三角形倍数转化为slider的value
    {
        return (n-1)*2;
    }

}
