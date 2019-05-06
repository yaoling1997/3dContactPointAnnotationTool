using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using Hont;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Text;
//using System.Windows.Forms;

public class MenuBarController : MonoBehaviour {
    private ObjManager objManager;    
    private GameObject imageBackground;
    private Canvas canvasBackground;
    private Camera mainCamera;
    private Vector3 mainCameraPosition;
    private Quaternion mainCameraRotation;
    private string saveProjectPath;//存储工程的路径    

    public Toggle WindowXZGridToggle;
    public Toggle WindowCameraControllerToggle;
    public Toggle WindowSMPLControllerToggle;
    public Toggle WindowConsoleToggle;
    public Toggle WindowDrawMapControllerToggle;

    public GameObject filePanel;//File面板
    public GameObject editPanel;//Edit面板

    public List<GameObject> menuBarButtons;    
    public EventSystem eventSystem;

    private int vertexOffset = 0;
    private int normalOffset = 0;
    private int uvOffset = 0;

    void Awake () {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        imageBackground = objManager.imageBackground;
        canvasBackground = objManager.canvasBackground;
        mainCamera = objManager.mainCamera;
        mainCameraPosition = mainCamera.transform.position;//记录初始的主相机位置和旋转参数
        mainCameraRotation = mainCamera.transform.rotation;
        saveProjectPath = null;
        WindowXZGridToggle.isOn = objManager.runtimeEditorApplication.XZGrid.IsVisible;
        WindowCameraControllerToggle.isOn = objManager.panelCameraController.activeSelf;
        WindowSMPLControllerToggle.isOn = objManager.panelSMPLController.activeSelf;
        WindowConsoleToggle.isOn = objManager.panelConsole.activeSelf;
        WindowDrawMapControllerToggle.isOn= objManager.panelDrawMapController.activeSelf;
    }
	
	// Update is called once per frame
	void Update () {
    }

    private Vector3[] GetRealBoundsVertices(MeshFilter meshFilter)//将点变换到真实的点（应用位移、旋转、缩放变换）
    {
        Mesh mesh = meshFilter.mesh;        
        var vertices = new Vector3[8];
        var transform = meshFilter.transform;
        var bounds = meshFilter.mesh.bounds;
        var x = bounds.extents.x;
        var y = bounds.extents.y;
        var z = bounds.extents.z;
        Quaternion rotation = Quaternion.Euler(transform.eulerAngles);
        Matrix4x4 m = Matrix4x4.TRS(transform.position, rotation, transform.lossyScale);
        vertices[0] = m.MultiplyPoint3x4(bounds.center + new Vector3(x, y, z));
        vertices[1] = m.MultiplyPoint3x4(bounds.center + new Vector3(-x, y, z));
        vertices[2] = m.MultiplyPoint3x4(bounds.center + new Vector3(x, -y, z));
        vertices[3] = m.MultiplyPoint3x4(bounds.center + new Vector3(x, y, -z));
        vertices[4] = m.MultiplyPoint3x4(bounds.center + new Vector3(-x, -y, z));
        vertices[5] = m.MultiplyPoint3x4(bounds.center + new Vector3(-x, y, -z));
        vertices[6] = m.MultiplyPoint3x4(bounds.center + new Vector3(x, -y, -z));
        vertices[7] = m.MultiplyPoint3x4(bounds.center + new Vector3(-x, -y, -z));
        return vertices;
    }
    private Vector2 GetMinVector2(Vector2 a, Vector2 b)//每一维取最小的
    {
        var re = new Vector2(0, 0);
        re.x = Mathf.Min(a.x, b.x);
        re.y = Mathf.Min(a.y, b.y);
        return re;
    }
    private Vector2 GetMaxVector2(Vector2 a, Vector2 b)//每一维取最大的
    {
        var re = new Vector2(0, 0);
        re.x = Mathf.Max(a.x, b.x);
        re.y = Mathf.Max(a.y, b.y);
        return re;
    }
    private void Get2dContactPoint(Vector3[] v,out Vector2 center,out Vector2 d)//根据vector3的数组获得他们的二维接触点(x,y坐标,x,y缩放大小)
    {
        var oo = Macro.oo;
        var MinP = new Vector2(oo, oo);
        var MaxP = new Vector2(-oo, -oo);
        var mainCamera = objManager.mainCamera;
        var cameraBackground = objManager.cameraBackground;
        var rt = imageBackground.GetComponent<RectTransform>();
        foreach (var i in v)
        {
            Debug.Log("v:" + v);
            var p = mainCamera.WorldToScreenPoint(i);//世界坐标点转屏幕坐标点
            Vector2 lp;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, p, cameraBackground, out lp);//屏幕坐标点转局部坐标点
            MinP = GetMinVector2(MinP, lp);
            MaxP = GetMaxVector2(MaxP, lp);
        }
        //Debug.Log("MinP:"+MinP);
        //Debug.Log("MaxP:" + MaxP);
        center = (MinP + MaxP) / 2 + rt.sizeDelta / 2;
        d = MaxP - MinP;
    }
    public string FloatToString(float v, int precision) {
        return System.Math.Round(v,precision).ToString();
    }
    public string Vector2ToString(Vector2 v, int precision)
    {//将Vector3转化为string基于精度
        var re = string.Format("({0}, {1})", FloatToString(v.x, precision), FloatToString(v.y, precision));
        return re;
    }
    public string Vector3ToString(Vector3 v,int precision) {//将Vector3转化为string基于精度
        var re = string.Format("({0}, {1}, {2})", FloatToString(v.x, precision), FloatToString(v.y, precision), FloatToString(v.z, precision));
        return re;
    }
    private void ExportCamera(string path) {//导出相机参数
        if (File.Exists(path)) {
            Save.SaveCamera mainCamera;
            Save.SaveCameraInfo(out mainCamera);
            var exportPrecision=objManager.panelSettingController.exportPrecision;
            var content = "positon: " + Vector3ToString(mainCamera.position.ToVector3(),exportPrecision)+ ", ";
            content += "eulerAngles: "+ Vector3ToString(mainCamera.eulerAngles.ToVector3(), exportPrecision) + ", ";
            content += "fieldOfView: " + FloatToString(mainCamera.fieldOfView,exportPrecision) + "\r\n";
            File.WriteAllText(path,content);
        }
        else
        {
            Debug.Log("no such ExportCamera txt!");
        }
    }
    private void ExportModels(string pathObjModel,string pathSMPL)//导出obj模型和smpl模型的参数
    {
        if (File.Exists(pathObjModel)&&File.Exists(pathSMPL))
        {
            List<Save.SaveObjModel> objModelList;
            List<Save.SaveSMPL> SMPLList;
            Save.SaveModelsInfo(out objModelList,out SMPLList);
            var exportPrecision = objManager.panelSettingController.exportPrecision;
            string content = "";
            foreach (var item in objModelList) {
                content += "path: "+item.path+ "\r\n";
                var len = item.position.Count;
                for (int i = 0; i < len; i++) {
                    content += "position: " + Vector3ToString(item.position[i].ToVector3(), exportPrecision) + "\r\n";
                    content += "eulerAngles: " + Vector3ToString(item.eulerAngles[i].ToVector3(), exportPrecision) + "\r\n";
                    content += "scale: " + Vector3ToString(item.scale[i].ToVector3(), exportPrecision) + "\r\n";
                }
            }
            File.WriteAllText(pathObjModel, content);
            content = "";
            foreach (var item in SMPLList) {
                content += "position: " + Vector3ToString(item.position.ToVector3(), exportPrecision) + ", ";
                content += "eulerAngles: " + Vector3ToString(item.eulerAngles.ToVector3(), exportPrecision) + ", ";
                content += "poseParam(localEulerAngles): ";
                foreach (var p in item.poseParam)
                    content += FloatToString(p,exportPrecision) + " ";
                content += ", shapeParam: ";
                for (int i = 0; i < item.shapeParam.Count; i++) {
                    content += FloatToString(item.shapeParam[i], exportPrecision) + (i< item.shapeParam.Count-1?" ":"\r\n");
                }
            }
            File.WriteAllText(pathSMPL, content);
        }
        else
        {
            Debug.Log("no such ExportModels txt!");
        }
    }
    private void ExportContactPoints(string path)//导出接触点参数
    {
        if (File.Exists(path))
        {
            string content = "";
            var contactPoints = objManager.contactPoints;
            var image = objManager.imageBackground.GetComponent<Image>();
            var imageRt = image.GetComponent<RectTransform>().sizeDelta;
            var realWidth = image.mainTexture.width;//真实图像像素宽
            var realHeight = image.mainTexture.height;//真实图像像素高
            var nowWidth = imageRt.x;//工具中显示的图像的宽
            var nowHeight = imageRt.y;//工具中显示的图像的高
            var exportPrecision = objManager.panelSettingController.exportPrecision;
            Debug.Log("width:" + realWidth);
            Debug.Log("height:" + realHeight);
            content += "3d:\r\n";
            foreach (var item in contactPoints.GetComponentsInChildren<Transform>())
            {
                if (item.name.Equals(contactPoints.name))
                    continue;
                var p = item.position;
                var e = item.eulerAngles;
                var s = item.lossyScale;
                content += "position: " +Vector3ToString(p,exportPrecision)+", ";//支点
                content += "eulerAngles: " + Vector3ToString(e, exportPrecision) + ", ";//欧拉角
                content += "scale: " + Vector3ToString(s, exportPrecision) + "\r\n";//局部轴的缩放
            }
            if (objManager.imagePath != null) {//图片加载了才导出2d接触点
                content += "2d:\r\n";
                foreach (var item in contactPoints.GetComponentsInChildren<Transform>())
                {
                    if (item.name.Equals(contactPoints.name))
                        continue;
                    var vertices=GetRealBoundsVertices(item.GetComponent<MeshFilter>());
                    Vector2 p, d;
                    Get2dContactPoint(vertices,out p,out d);
                    //Debug.Log("p:"+p);
                    //Debug.Log("d:"+d);
                    var realPx = realWidth / nowWidth * p.x;//获得像素为单位的接触点x,y,r
                    var realPy = realHeight / nowHeight * p.y;
                    var realDx = realWidth / nowWidth * d.x;
                    var realDy = realHeight / nowHeight * d.y;
                    //Debug.Log("realp:" + new Vector2(realPx, realPy));
                    //Debug.Log("reald:" + new Vector2(realDx, realDy));

                    content += "positon: "+Vector2ToString(new Vector2(realPx,realPy),exportPrecision) + ", ";
                    content += "scale: "+ Vector2ToString(new Vector2(realDx, realDy),exportPrecision) + "\r\n";
                }
            }
            File.WriteAllText(path, content);
        }
        else
        {
            Debug.Log("no such ExportContactPoints txt!");
        }
    }
    private void ExportBackgroundImage(string path)//导出参考图像参数
    {
        if (File.Exists(path))
        {
            Save.SaveImage imageInfo;
            Save.SaveBackgroundImageInfo(out imageInfo);
            var texture = Instantiate(objManager.imageBackground.GetComponent<Image>().mainTexture) as Texture2D;
            var s = ObjManager.StringToFloat(imageInfo.scale);
            var a= ObjManager.StringToFloat(imageInfo.alpha);
            var nt = ObjManager.GetModifiedTexture2D(texture, (int)(s*texture.width), (int)(s * texture.height), a);
            var bytes= nt.EncodeToPNG();
            File.WriteAllBytes(path,bytes);            
        }
        else
        {
            Debug.Log("no such ExportBackgroundImage file!");
        }
    }
    private void Export(string path) {//导出到path文件夹
        if (string.IsNullOrEmpty(path))
            return ;
        string fullRootPath = Path.GetFullPath(path);
        if (string.IsNullOrEmpty(fullRootPath))
            return ;
        if (!Directory.Exists(path)) {
            Debug.Log("path is not a directory!");
            return;
        }
        path += @"\Export";
        Directory.CreateDirectory(path);
        var cameraFile = path + @"\camera.txt";
        var SMPLFile = path + @"\SMPL.txt";
        var objModelFile = path + @"\objModel.txt";
        var contactPointsFile = path + @"\contactPoints.txt";
        var backgroundImageFile = path + @"\referenceImage.png";
        File.Create(cameraFile).Dispose();
        File.Create(SMPLFile).Dispose();
        File.Create(objModelFile).Dispose();
        File.Create(contactPointsFile).Dispose();
        File.Create(backgroundImageFile).Dispose();
        ExportCamera(cameraFile);
        ExportModels(objModelFile,SMPLFile);
        ExportContactPoints(contactPointsFile);//导出接触点
        ExportBackgroundImage(backgroundImageFile);
    }
    public string SmrToObjString(SkinnedMeshRenderer smr) {
        StringBuilder re = new StringBuilder();
        var mesh = new Mesh();
        smr.BakeMesh(mesh);
        //var vertices = ObjManager.GetRealVertices(smr);
        foreach (var V in mesh.vertices) {
            var v = smr.transform.TransformPoint(V);
            re.Append(string.Format("v {0} {1} {2}\r\n", v.x, v.y, v.z));
        }
        re.Append("\r\n");
        foreach (var VN in mesh.normals) {
            var vn = smr.transform.TransformDirection(VN);
            re.Append(string.Format("vn {0} {1} {2}\r\n",vn.x, vn.y, vn.z));
        }
        re.Append("\r\n");
        foreach (var vt in mesh.uv)
        {
            re.Append(string.Format("vt {0} {1}\r\n", vt.x, vt.y));
        }
        re.Append("\r\n");
        re.Append("o " +smr.name+"\r\n");
        re.Append("\r\n");
        var triangles = mesh.triangles;
        string uv0 = "";
        string uv1 = "";
        string uv2 = "";
        string normal0 = "";
        string normal1 = "";
        string normal2 = "";
        Debug.Log("mesh.uv.length: "+mesh.uv.Length);
        Debug.Log("mesh.normals.length: " + mesh.normals.Length);
        for (int i = 0; i < triangles.Length; i += 3) {
            if (mesh.uv.Length != 0)
            {
                uv0 = (triangles[i] + uvOffset + 1).ToString();
                uv1 = (triangles[i + 1] + uvOffset + 1).ToString();
                uv2 = (triangles[i + 2] + uvOffset + 1).ToString();
            }
            else {
                uv0 = "";
                uv1 = "";
                uv2 = "";
            }
            if (mesh.normals.Length != 0)
            {
                normal0 = (triangles[i] + normalOffset + 1).ToString();
                normal1 = (triangles[i + 1] + normalOffset + 1).ToString();
                normal2 = (triangles[i + 2] + normalOffset + 1).ToString();
            }
            else {
                normal0 = "";
                normal1 = "";
                normal2 = "";
            }
            re.Append(string.Format("f {0}/{1}/{2} ", triangles[i + 1] + vertexOffset + 1, uv1, normal1));
            re.Append(string.Format(" {0}/{1}/{2} ", triangles[i] + vertexOffset + 1, uv0, normal0));
            re.Append(string.Format(" {0}/{1}/{2}\r\n", triangles[i + 2] + vertexOffset + 1, uv2, normal2));
            //re.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\r\n", triangles[i] + 1 + vertexOffset, triangles[i + 1] + 1 + normalOffset, triangles[i + 2] + 1 + uvOffset));
        }
        vertexOffset += mesh.vertices.Length;
        normalOffset += mesh.normals.Length;
        uvOffset += mesh.uv.Length;        
        return re.ToString();
    }
    public void ExportSceneToObj(string path) {
        File.Create(path);
        if (File.Exists(path))
        {
            vertexOffset = 0;
            normalOffset = 0;
            uvOffset = 0;
            var content = "";
            foreach (var smr in objManager.model3d.GetComponentsInChildren<SkinnedMeshRenderer>()) {
                content += SmrToObjString(smr);
            }
            File.WriteAllText(path,content);
        }
    }
    public void ButtonImportImageOnClick()//ImportImage按钮被点击
    {
        CloseFilePanel();
        OpenFileName ofn = new OpenFileName();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = "图片文件(*.jpg*.jpeg*.png)\0*.jpg;*.png;*.jpeg";
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
        ofn.title = "导入图像";
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
        if (LocalDialog.GetOpenFileName(ofn))
        {
            Debug.Log("file: " + ofn.file);
            objManager.LoadImage(ofn.file);
        }

    }
    public void ButtonImportModelOnClick()//ImportModel按钮被点击
    {
        CloseFilePanel();
        OpenFileName ofn = new OpenFileName();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = "3d文件(*.obj)\0*.obj";
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
        ofn.title = "导入3d模型";
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
        if (LocalDialog.GetOpenFileName(ofn))
        {
            Debug.Log("file: " + ofn.file);
            objManager.LoadObj(ofn.file);
        }
    }
    public void ButtonExportOnClick()//Export按钮被点击
    {
        CloseFilePanel();
        System.Windows.Forms.FolderBrowserDialog fb = new System.Windows.Forms.FolderBrowserDialog();   //创建控件并实例化
        fb.Description = "选择文件夹";
        fb.ShowNewFolderButton = false;   //创建文件夹按钮关闭
                                          //如果按下弹窗的OK按钮
        string path = "";
        if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)//接受路径
        {
            path = fb.SelectedPath;
            Debug.Log("path: " + path);
            Export(path);
        }
        else//用户取消
        {
            Debug.Log("cancel!");
            return;
        }
    }
    public void ButtonExportScene()//导出场景到obj
    {
        CloseFilePanel();
        OpenFileName ofn = new OpenFileName();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = "Obj文件(*.obj)\0*.obj";
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
        ofn.title = "导出场景到obj";
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
        if (LocalDialog.GetSaveFileName(ofn))
        {
            if (!File.Exists(ofn.file))
            {
                ofn.file += ".obj";
                //File.Create(ofn.file);
            }
            Debug.Log("file: " + ofn.file);
            ExportSceneToObj(ofn.file);
        }
    }
    public void ButtonOpenProjectOnClick()//OpenProject按钮被点击
    {
        CloseFilePanel();
        OpenFileName ofn = new OpenFileName();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = "项目文件(*.proj)\0*.proj";
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
        ofn.title = "打开项目文件";
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
        if (LocalDialog.GetOpenFileName(ofn))
        {
            Debug.Log("file: " + ofn.file);
            saveProjectPath = ofn.file;
            Save.LoadByBin(saveProjectPath);
        }        
    }
    public void ButtonSaveProjectOnClick()//SaveProject按钮被点击
    {
        CloseFilePanel();
        if (saveProjectPath == null) { 
            ButtonSaveProjectAsOnClick();
            return;
        }
        Save.SaveByBin(saveProjectPath);
    }
    public void ButtonSaveProjectAsOnClick()//SaveProjectAs按钮被点击
    {
        CloseFilePanel();
        OpenFileName ofn = new OpenFileName();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = "项目文件(*.proj)\0*.proj";
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
        ofn.title = "另存为";
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
        if (LocalDialog.GetSaveFileName(ofn))
        {
            if (!File.Exists(ofn.file)) {
                ofn.file += ".proj";
                //File.Create(ofn.file);
            }
            Debug.Log("file: " + ofn.file);
            //StartCoroutine(Export3dContactPoints(ofn.file));
            saveProjectPath = ofn.file;//更新保存项目的存储路径
            Save.SaveByBin(saveProjectPath);
        }        
    }
    public void ButtonExitOnClick()//Exit按钮被点击
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void CloseFilePanel()//关闭FilePanel
    {
        filePanel.SetActive(false);
    }
    public void CloseEditPanel()//关闭EditPanel
    {
        editPanel.SetActive(false);
    }
    public void ButtonFixImage()//UnbindImage按钮被点击
    {
        canvasBackground.renderMode = RenderMode.ScreenSpaceCamera;
        CloseEditPanel();
    }
    public void ButtonUnfixImage()//BindImage按钮被点击
    {
        canvasBackground.renderMode = RenderMode.WorldSpace;//背景图不再固定
        CloseEditPanel();
    }
    public void ButtonStoreCameraValues()//StoreCameraValues按钮被点击,存储当前主相机位置和旋转
    {
        mainCameraPosition = mainCamera.transform.position;
        mainCameraRotation = mainCamera.transform.rotation;
        CloseEditPanel();
    }
    public void ButtonRestoreCameraValues()//RestoreCameraValues按钮被点击,恢复主相机位置和旋转
    {
        mainCamera.transform.position = mainCameraPosition;
        mainCamera.transform.rotation = mainCameraRotation;
        CloseEditPanel();
    }
    public void ButtonResetOnClick()//Reset按钮被点击,将所有内容恢复到一开始的状态
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);        
        CloseEditPanel();
    }

    public void ButtonXZGridOnClick()//XZGrid按钮被点击
    {
        var XZGrid = objManager.runtimeEditorApplication.XZGrid;
        var active = !XZGrid.IsVisible;
        XZGrid.IsVisible=active;
        WindowXZGridToggle.isOn = active;
    }
    public void ButtonCameraControllerOnClick()//CameraController按钮被点击
    {
        var panelCameraController = objManager.panelCameraController;
        var active = !panelCameraController.activeSelf;
        panelCameraController.SetActive(active);
        WindowCameraControllerToggle.isOn = active;
    }
    public void ButtonSMPLControllerOnClick()//SMPLController按钮被点击
    {
        var panelSMPLController = objManager.panelSMPLController;
        var active = !panelSMPLController.activeSelf;
        panelSMPLController.SetActive(active);
        WindowSMPLControllerToggle.isOn = active;
    }
    public void ButtonConsoleOnClick()//Console按钮被点击
    {
        var panelConsole = objManager.panelConsole;
        var active = !panelConsole.activeSelf;
        panelConsole.SetActive(active);
        WindowConsoleToggle.isOn = active;
    }
    public void ButtonDrawMapControllerOnClick()//DrawMapController按钮被点击
    {
        var panelDrawMapController = objManager.panelDrawMapController;
        var active = !panelDrawMapController.activeSelf;
        panelDrawMapController.SetActive(active);
        WindowDrawMapControllerToggle.isOn = active;
    }
    public bool IfMenuBarButtonSelected() {//是否选中了menuBar上的按钮
        var s = eventSystem.currentSelectedGameObject;
        return s != null&& menuBarButtons.Contains(s);
    }
}
