using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using Hont;
using System.IO;
using UnityEngine.SceneManagement;

public class MenuBarController : MonoBehaviour {
    private ObjManager objManager;    
    private GameObject imageBackground;
    private Canvas canvasBackground;
    private Camera mainCamera;
    private Vector3 mainCameraPosition;
    private Quaternion mainCameraRotation;
    private string saveProjectPath;//存储工程的路径

    public Toggle WindowModelsToggle;
    public Toggle WindowContactPointsToggle;
    public Toggle WindowStatusToggle;

    public GameObject filePanel;//File面板
    public GameObject editPanel;//Edit面板
    
    void Awake () {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        imageBackground = objManager.imageBackground;
        canvasBackground = objManager.canvasBackground;
        mainCamera = objManager.mainCamera;
        mainCameraPosition = mainCamera.transform.position;//记录初始的主相机位置和旋转参数
        mainCameraRotation = mainCamera.transform.rotation;
        saveProjectPath = null;
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
        Matrix4x4 m = Matrix4x4.TRS(transform.position, rotation, transform.localScale);
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
        Debug.Log("MinP:"+MinP);
        Debug.Log("MaxP:" + MaxP);
        center = (MinP + MaxP) / 2 + rt.sizeDelta / 2;
        d = MaxP - MinP;
    }
    private void Export3dContactPoints(string path)
    {
        //yield return new WaitForSeconds(1);
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
            Debug.Log("width:" + realWidth);
            Debug.Log("height:" + realHeight);
            content += "3d:\r\n";
            foreach (var item in contactPoints.GetComponentsInChildren<Transform>())
            {
                if (item.name.Equals(contactPoints.name))
                    continue;
                var p = item.position;
                var e = item.eulerAngles;
                var s = item.localScale;
                content += p.x + " ";
                content += p.y + " ";
                content += p.z + " ";//支点
                content += e.x + " ";
                content += e.y + " ";
                content += e.z + " ";//欧拉角
                content += s.x + " ";
                content += s.y + " ";
                content += s.z + "\r\n";//局部轴的缩放
                //保留4位小数
                //content += System.Math.Round(p.x,4) + " ";
                //content += System.Math.Round(p.y,4) + " ";
                //content += System.Math.Round(p.z,4) + " ";
                //content += System.Math.Round(s.x,4) + " ";
                //content += System.Math.Round(s.y,4) + " ";
                //content += System.Math.Round(s.z,4) + "\r\n";
            }            
            content += "2d:\r\n";
            foreach (var item in contactPoints.GetComponentsInChildren<Transform>())
            {
                if (item.name.Equals(contactPoints.name))
                    continue;
                var vertices=GetRealBoundsVertices(item.GetComponent<MeshFilter>());
                Vector2 p, d;
                Get2dContactPoint(vertices,out p,out d);
                Debug.Log("p:"+p);
                Debug.Log("d:"+d);
                var realPx = realWidth / nowWidth * p.x;//获得像素为单位的接触点x,y,r
                var realPy = realHeight / nowHeight * p.y;
                var realDx = realWidth / nowWidth * d.x;
                var realDy = realHeight / nowHeight * d.y;
                Debug.Log("realp:" + new Vector2(realPx, realPy));
                Debug.Log("reald:" + new Vector2(realDx, realDy));

                content += realPx + " ";
                content += realPy + " ";
                content += realDx + " ";
                content += realDy + "\r\n";
            }            
            File.WriteAllText(path, content);
        }
        else
        {
            Debug.Log("no such txt!");
        }
    }

    public void ButtonImportImageOnClick()//ImportImage按钮被点击
    {
        CloseFilePanel();
        OpenFileName ofn = new OpenFileName();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = "图片文件(*.jpg*.png)\0*.jpg;*.png";
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
            //StartCoroutine(GetImage(ofn.file));
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
            //StartCoroutine(GetObj(ofn.file));
            objManager.LoadObj(ofn.file);
        }
    }
    public void ButtonExportContactPointsOnClick()//ExportContactPoints按钮被点击
    {
        CloseFilePanel();
        OpenFileName ofn = new OpenFileName();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = "文本文件(*.txt)\0*.txt";
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
        ofn.title = "导出接触点";
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
        if (LocalDialog.GetSaveFileName(ofn))
        {
            if (!File.Exists(ofn.file)) {
                ofn.file += ".txt";
                File.Create(ofn.file);
            }
            Debug.Log("file: " + ofn.file);
            //StartCoroutine(Export3dContactPoints(ofn.file));
            Export3dContactPoints(ofn.file);
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
            //StartCoroutine(GetObj(ofn.file));
            objManager.LoadObj(ofn.file);
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

    public void ButtonModelsOnClick()//Models按钮被点击
    {
        var panelModels = objManager.panelModels;        
        var active = !panelModels.activeSelf;
        panelModels.SetActive(active);
        WindowModelsToggle.isOn = active;
    }
    public void ButtonContactPointsOnClick()//ContactPoints按钮被点击
    {
        var panelContactPoints = objManager.panelContactPoints;        
        var active = !panelContactPoints.activeSelf;
        panelContactPoints.SetActive(active);
        WindowContactPointsToggle.isOn = active;
    }
    public void ButtonStatusOnClick()//Status按钮被点击
    {
        var panelStatus = objManager.panelStatus;
        var active = !panelStatus.activeSelf;
        panelStatus.SetActive(active);
        WindowStatusToggle.isOn = active;
    }
}
