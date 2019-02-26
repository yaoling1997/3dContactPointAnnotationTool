using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTEditor;
using Hont;
using System.IO;

public class ObjManager : MonoBehaviour//管理对象，避免找不到active为false的对象的尴尬
{
    public Canvas canvas;//UI的canvas    
    public Canvas canvasBackground;//背景的canvas    
    public Camera mainCamera;//主相机
    public Camera cameraBackground;//背景图片相机
    public Camera cameraShowItem;//showItem相机
    public GameObject panelModels;
    public GameObject panelContactPoints;
    public GameObject panelStatus;
    public ReferenceImageController referenceImageController;//参考图片的控制器
    public PanelStatusController panelStatusController;//panelStatus的controller
    public GameObject prefabScrollViewItem;//scrollViewItem预制件
    public GameObject prefabScrollViewTabItem;//scrollViewTabItem预制件
    public GameObject prefabScrollViewItemsItem;//scrollViewItemsItem预制件
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

    public RuntimeEditorApplication runtimeEditorApplication;
    public EditorObjectSelection editorObjectSelection;
    public GameObject imageBackground;//背景图片,人和物体的交互图片作为背景
    public PanelBackgroundImageController panelBackgroundImageControllerScript;//控制背景图片的panel的script
    public PanelItemWarehouseController panelItemWareHouseController;//控制ItemWareHouse的panel的script
    public PanelModel_PointsInformationController panelModel_PointsInformationController;

    public string imagePath;//加载图片的路径

    // Use this for initialization
    void Start () {
        contactPointId = 0;
        humanModelId = 0;
        imagePath = null;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void CreateContactPointSphere(Vector3 position, Vector3 eulerAngles, Vector3 scale) {
        contactPointId++;
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        go.layer = Macro.CONTACT_POINTS_ITEM;//设置为ContactPointsItem层，实现光照等效果

        go.transform.position = position;//设置球的中心位置
        go.transform.eulerAngles = eulerAngles;//设置欧拉角
        go.transform.localScale = scale;//设置球的半径大小
        var mr = go.GetComponent<MeshRenderer>();//获取球的meshRenderer
        mr.material.color = Color.red;//设置为红球
        //不产生阴影也不接收阴影
        mr.receiveShadows = false;
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        go.transform.SetParent(contactPoints.transform);
        go.name = go.name + contactPointId;
        go.tag = Macro.UNSELECTED;
        go.AddComponent<ItemController>().SetModelType(ItemController.ModelType.CONTACT_POINT);        
        var scrollViewItem = Instantiate(prefabScrollViewItem, new Vector3(0, 0, 0), Quaternion.identity);
        scrollViewItem.GetComponent<ScrollViewItemController>().Init(go, scrollViewContactPointsContent, Color.red);
    }
    public void CreateContactPointSphere(Vector3 center, float rx, float ry,float rz)//x半径,y半径,z半径,创建球状接触点
    {
        CreateContactPointSphere(center,Vector3.zero, new Vector3(2 * rx, 2 * ry, 2 * rz));//scale是半径的两倍
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
    public void LoadImage(string path)
    {
        WWW www = new WWW("file://" + path);
        //yield return new WaitForSeconds(1);
        if (www != null && string.IsNullOrEmpty(www.error)&&File.Exists(path))
        {
            imagePath = path;//存储该图片路径
            Texture2D texture = new Texture2D(www.texture.width, www.texture.height);
            Debug.Log("width:" + texture.width);
            Debug.Log("height:" + texture.height);
            texture.SetPixels(www.texture.GetPixels());
            texture.Apply(true);
            texture.filterMode = FilterMode.Trilinear;
            //var image= objManager.contactPoints2d;//contactPoints2d就是那张图像，只是所有2d接触点都挂在这个图像上
            //image.GetComponent<ImageController>().SetImage(texture);
            imageBackground.GetComponent<RectTransform>().sizeDelta = new Vector2(texture.width, texture.height);
            var image = imageBackground.GetComponent<Image>();
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            var color = image.color;
            color.a = 1;
            image.color = color;
            panelBackgroundImageControllerScript.Init();
            referenceImageController.Init(texture);
        }
        else
        {
            Debug.Log("no such image!");
        }
    }
    public void LoadObj(string path,Vector3 postion,Vector3 eulerAngles,Vector3 scale)
    {
        //yield return new WaitForSeconds(1);//改成0s可能造成UI不稳定，不知道为啥
        if (File.Exists(path))
        {
            var re = ObjFormatAnalyzerFactory.AnalyzeToGameObject(path, true);//默认只有一个obj对象
            foreach (var item in re)
            {
                model3d.GetComponent<Model3dController>().AddSon(item);//将解析出来的obj的父亲设置为model3d 
                item.transform.position = postion;
                item.transform.eulerAngles = eulerAngles;
                item.transform.localScale = scale;
                item.AddComponent<Model3dItemController>();//添加该脚本
                item.AddComponent<ItemController>().SetModelType(ItemController.ModelType.OBJ_MODEL).path = path;//设置类型和obj文件路径                
                var scrollViewItem = Instantiate(prefabScrollViewItem, new Vector3(0, 0, 0), Quaternion.identity);
                scrollViewItem.GetComponent<ScrollViewItemController>().Init(item, scrollViewModelsContent);
            }
        }
        else
        {
            Debug.Log("no such model!");
        }
    }

    public void LoadObj(string path)
    {
        LoadObj(path,Vector3.zero,Vector3.zero,new Vector3(1,1,1));
    }    
    public GameObject LoadObjToShowItemView(string path)
    {
        //yield return new WaitForSeconds(1);//改成0s可能造成UI不稳定，不知道为啥
        if (File.Exists(path))
        {
            var re = ObjFormatAnalyzerFactory.AnalyzeToGameObject(path, false);
            if (re.Count == 0)
                return null;
            re[0].layer = Macro.SHOW_ITEM;
            return re[0];
        }
        else
        {
            Debug.Log("no such model!");
        }
        return null;
    }

}
