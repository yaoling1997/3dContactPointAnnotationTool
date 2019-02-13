using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTEditor;

public class ObjManager : MonoBehaviour//管理对象，避免找不到active为false的对象的尴尬
{
    public Canvas canvas;//UI的canvas    
    public Canvas canvasBackground;//背景的canvas    
    public Camera mainCamera;//主相机
    public Camera cameraBackground;//主相机
    public GameObject panelModels;
    public GameObject panelContactPoints;
    public GameObject panelStatus;
    public ReferenceImageController referenceImageController;//参考图片的控制器
    public PanelStatusController panelStatusController;//panelStatus的controller
    public GameObject prefabScrollViewItem;//scrollViewItem预制件
    public GameObject prefabScrollViewTabItem;//scrollViewTabItem预制件
    public GameObject model3d;//3d模型们
    public GameObject contactPoints;//接触点们
    public GameObject scrollViewModelsContent;//模型scrollView的content
    public GameObject scrollViewContactPointsContent;//接触点scrollView的content
    public GameObject menuBar;//菜单栏
    public GameObject toolBar;//工具栏    
    public GameObject humanModel;//SMPL人体模型    
    public Material materialStandard;//标准 shader的材质
    public Material materialUIdefault;//UIdefault shader的材质
    public Material materialWireframe;//网格 shader的材质
    public int contactPointId;//3d接触点标号
    public int humanModelId;//人体模型标号
    public Sprite spriteTriangleUp;//向上三角形sprite
    public Sprite spriteTriangleRight;//向右三角形sprite
    public Sprite spriteTriangleDown;//向下三角形sprite
    
    public Button buttonGCP;    

    public EditorObjectSelection editorObjectSelection;
    public GameObject imageBackground;//背景图片,人和物体的交互图片作为背景
    public PanelBackgroundImageController panelBackgroundImageControllerScript;//控制背景图片的panel的script

    public PanelModel_PointsInformationController panelModel_PointsInformationController;

    // Use this for initialization
    void Start () {
        contactPointId = 0;
        humanModelId = 0;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void CreateContactPointSphere(Vector3 center, float x, float y,float z)//x半径,y半径,z半径,创建球状接触点
    {
        contactPointId++;
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        go.layer = Macro.CONTACT_POINTS_ITEM;//设置为ContactPointsItem层，实现光照等效果

        go.transform.position = center;//设置球的中心位置
        go.transform.localScale = new Vector3(2*x, 2*y, 2*z);//设置球的半径大小,scale是半径的两倍
        var mr = go.GetComponent<MeshRenderer>();//获取球的meshRenderer
        mr.material.color = Color.red;//设置为红球
        //不产生阴影也不接收阴影
        mr.receiveShadows = false;
        mr.shadowCastingMode =UnityEngine.Rendering.ShadowCastingMode.Off;

        go.transform.SetParent(contactPoints.transform);
        go.name = go.name + contactPointId;
        go.tag = Macro.UNSELECTED;
        var itemController= go.AddComponent<ItemController>();
        itemController.trianglesEditable = false;
        //var scrollViewItem = UnityEditor.PrefabUtility.InstantiatePrefab(prefabScrollViewItem) as GameObject;
        var scrollViewItem = Instantiate(prefabScrollViewItem, new Vector3(0, 0, 0), Quaternion.identity);
        scrollViewItem.GetComponent<ScrollViewItemController>().Init(go, scrollViewContactPointsContent, Color.red);
    }
    public void CreateContactPointSphere(Vector3 center, float radius)
    {
        CreateContactPointSphere(center,radius, radius, radius);
    }
    public void CreateContactPointSphere(Vector3 center, Vector3 v)
    {
        CreateContactPointSphere(center, v.x, v.y, v.z);
    }
    public void UnselectAll()
    {
        foreach(var item in panelModels.GetComponentsInChildren<ScrollViewItemController>())
        {
            item.SetUnselected();
        }
        foreach (var item in panelContactPoints.GetComponentsInChildren<ScrollViewItemController>())
        {
            item.SetUnselected();
        }
    }
}
