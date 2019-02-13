using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelModel_PointsInformationController : MonoBehaviour {
    public Button buttonMaterial;//控制材质选择的按钮
    private ObjManager objManager;
    private GameObject model3d;
    private const int materialNum = 3;//一共有几种material可供选择
    private int materialId;//当前显示的是哪一个material(0 standard,1 wireframe,2 UIdefault)
    private List<Material> materialList;
	// Use this for initialization
	void Start () {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        model3d = objManager.model3d;
        materialId = 0;
        materialList = new List<Material>();
        materialList.Add(objManager.materialStandard);
        materialList.Add(objManager.materialWireframe);
        materialList.Add(objManager.materialUIdefault);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void ButtonMaterialOnClick()
    {
        materialId = (materialId + 1) % 3;
        buttonMaterial.GetComponentInChildren<Text>().text = materialList[materialId].name;
        foreach (var item in model3d.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            var color = item.material.color;
            color.a = materialList[materialId].color.a;//透明度为新材质的透明度，只是保持原来的颜色不变
            item.material = new Material(materialList[materialId]);
            item.material.color = color;
        }
    }
    public Material GetMaterial()//返回当前选择的材质
    {
        return materialList[materialId];
    }
}
