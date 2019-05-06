using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTEditor;
using Hont;
using System;
using System.IO;

public class ObjManager : MonoBehaviour//管理对象，避免找不到active为false的对象的尴尬
{
    public const float oo= 1e18f;
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
    public GameObject panelCameraController;//控制相机的panel
    public GameObject panelSMPLController;//控制SMPL的panel
    public GameObject panelConsole;//控制Console显示的panel
    public GameObject panelDrawMapController;//控制DrawMapController显示的panel
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
    public Slider sliderTriangles;//修改三角面片数的slider
    public Text textConsoleContent;//console的内容
    public int contactPointId;//3d接触点标号
    public int humanModelId;//人体模型标号
    public Sprite spriteTriangleUp;//向上三角形sprite
    public Sprite spriteTriangleRight;//向右三角形sprite
    public Sprite spriteTriangleDown;//向下三角形sprite
    
    public Button buttonGCP;

    public RuntimeEditorApplication runtimeEditorApplication;
    public EditorGizmoSystem editorGizmoSystem;
    public EditorObjectSelection editorObjectSelection;
    public EditorUndoRedoSystem editorUndoRedoSystem;
    public GameObject imageBackground;//背景图片,人和物体的交互图片作为背景
    public PanelBackgroundImageController panelBackgroundImageControllerScript;//控制背景图片的panel的script
    public PanelItemWarehouseController panelItemWareHouseController;//控制ItemWareHouse的panel的script
    public PanelModel_PointsInformationController panelModel_PointsInformationController;
    public PanelSettingController panelSettingController;
    public ImageDrawMapController imageDrawMapController;//drawMap的控制器，用来计算准确率

    public string imagePath;//加载图片的路径

    public DontDestroyController dontDestroyController;    
    void Awake() {
        contactPointId = 0;
        humanModelId = 0;
        imagePath = null;
        dontDestroyController = GameObject.Find("DontDestroy").GetComponent<DontDestroyController>();
    }
    // Use this for initialization
    void Start () {        
        if (dontDestroyController.itemWarehousePath != null) {
            panelItemWareHouseController.AddItemWarehouse(dontDestroyController.itemWarehousePath);
        }
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
            imageDrawMapController.InitDrawMap(texture.width,texture.height);
            //Log("Load Image succeed!");

        }
        else
        {
            Debug.Log("no such image!");
        }
    }
    public void SetTransform(GameObject o,Vector3 position,Vector3 eulerAngles,Vector3 scale) {
        o.transform.position = position;
        o.transform.eulerAngles = eulerAngles;
        o.transform.localScale = scale;
    }
    public void LoadObj(string path,List<Vector3> postion, List<Vector3> eulerAngles, List<Vector3> scale)
    {
        //yield return new WaitForSeconds(1);//改成0s可能造成UI不稳定，不知道为啥
        if (File.Exists(path))
        {
            var tmp = path.Split(new char[] { '.','/','\\'});
            var name = tmp[tmp.Length - 2];
            var obj = new GameObject(name);
            model3d.GetComponent<Model3dController>().AddSon(obj);//将obj的父亲设置为model3d 
            obj.tag = Macro.UNSELECTED;//将tag设置为未选中
            //obj.GetComponent<SkinnedMeshRenderer>().material = panelModel_PointsInformationController.GetMaterial();
            obj.AddComponent<Model3dItemController>();//添加该脚本
            obj.AddComponent<ItemController>().SetModelType(ItemController.ModelType.OBJ_MODEL_ROOT).path = path;//添加该脚本
            if (postion!=null&&eulerAngles!=null&&scale!=null)
                SetTransform(obj,postion[0],eulerAngles[0],scale[0]);
            var rootSvi=Instantiate(prefabScrollViewItem, new Vector3(0, 0, 0), Quaternion.identity);
            rootSvi.GetComponent<ScrollViewItemController>().Init(obj, scrollViewModelsContent, name);
            int i = 1;
            var re = ObjFormatAnalyzerFactory.AnalyzeToGameObject(path, true);
            foreach (var item in re)
            {
                //model3d.GetComponent<Model3dController>().AddSon(item);//将解析出来的obj的父亲设置为model3d 
                //item.transform.position = postion;
                //item.transform.eulerAngles = eulerAngles;
                //item.transform.localScale = scale;
                item.AddComponent<Model3dItemController>();//添加该脚本
                item.AddComponent<ItemController>().SetModelType(ItemController.ModelType.OBJ_MODEL);//设置类型和obj文件路径   
                item.transform.SetParent(obj.transform);
                if (postion != null && eulerAngles != null && scale != null) {
                    SetTransform(item, postion[i], eulerAngles[i], scale[i]);
                    i++;
                }
                var scrollViewItem = Instantiate(prefabScrollViewItem, new Vector3(0, 0, 0), Quaternion.identity);
                scrollViewItem.GetComponent<ScrollViewItemController>().Init(item, scrollViewModelsContent);
                rootSvi.GetComponent<ScrollViewItemController>().AddSon(scrollViewItem);
            }
        }
        else
        {
            Debug.Log("no such model!");
        }
    }

    public void LoadObj(string path)
    {
        LoadObj(path,null,null,null);
    }    
    public GameObject LoadObjToShowItemView(string path)
    {
        //yield return new WaitForSeconds(1);//改成0s可能造成UI不稳定，不知道为啥
        if (File.Exists(path))
        {
            var objs = ObjFormatAnalyzerFactory.AnalyzeToGameObject(path, false);
            if (objs.Count == 0)
                return null;
            var re= new GameObject();
            foreach (var item in objs) {
                item.transform.SetParent(re.transform);
                item.layer = Macro.SHOW_ITEM;
            }            
            return re;
        }
        else
        {
            Debug.Log("no such model!");
        }
        return null;
    }
    public static Vector3 GetMinVector3(Vector3 a, Vector3 b)//每一维取最小的
    {
        var re = new Vector3(0, 0, 0);
        re.x = Mathf.Min(a.x, b.x);
        re.y = Mathf.Min(a.y, b.y);
        re.z = Mathf.Min(a.z, b.z);
        return re;
    }
    public static Vector3 GetMaxVector3(Vector3 a, Vector3 b)//每一维取最大的
    {
        var re = new Vector3(0, 0, 0);
        re.x = Mathf.Max(a.x, b.x);
        re.y = Mathf.Max(a.y, b.y);
        re.z = Mathf.Max(a.z, b.z);
        return re;
    }
    public static Bounds GetBoundsOfVector3Array(Vector3[] v)//根据vector3的数组获得他们的包围盒bounds
    {
        var MinP = new Vector3(oo, oo, oo);
        var MaxP = new Vector3(-oo, -oo, -oo);
        foreach (var i in v)
        {
            MinP = GetMinVector3(MinP, i);
            MaxP = GetMaxVector3(MaxP, i);
        }
        return new Bounds((MinP + MaxP) / 2, MaxP - MinP);
    }
    public static float StringToFloat(string s) {
        float re = 0;
        try
        {
            re = float.Parse(s);
        }
        catch {
            return 0;
        }
        return re;
    }
    public static int StringToInt(string s)
    {
        int re = 0;
        try
        {
            re = int.Parse(s);
        }
        catch
        {
            return 0;
        }
        return re;
    }
    public static Texture2D GetModifiedTexture2D(Texture2D source,int newWidth,int newHeight,float alpha) {
        var re = new Texture2D(newWidth,newHeight,source.format,false);
        for (int i = 0; i < newWidth; i++)
            for (int j = 0; j < newHeight; j++) {
                var nc = source.GetPixelBilinear((float)i/(float)newWidth,(float)j/(float)newHeight);
                nc.a = alpha;
                re.SetPixel(i, j, nc);
            }
        re.Apply();
        return re;
    }
    public static Vector3[] GetRealVertices(SkinnedMeshRenderer skinnedMeshRenderer)//将点变换到真实的点（应用位移、旋转、缩放变换）
    {
        Mesh mesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(mesh);
        var len = mesh.vertices.Length;
        var vertices = new Vector3[len];

        var transform = skinnedMeshRenderer.transform;
        Quaternion rotation = Quaternion.Euler(transform.eulerAngles);
        Matrix4x4 m = Matrix4x4.TRS(transform.position, rotation, transform.lossyScale);

        for (int i = 0; i < len; i++)
        {
            vertices[i] = m.MultiplyPoint3x4(mesh.vertices[i]);
        }
        return vertices;
    }
    public void Log(string s) {//将s显示到console中
        textConsoleContent.text = textConsoleContent.text + " "+DateTime.Now.ToString("T")+"\n"+" "+s+"\n";        
    }
}
