﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using Hont;
using System.IO;

public class MenuBarController : MonoBehaviour {
    private ObjManager objManager;
    private GameObject model3d;
    private GameObject scrollViewModelsContent;

    public Toggle WindowModelsToggle;
    public Toggle WindowContactPointsToggle;
    public Toggle WindowStatusToggle;
    public Toggle WindowReferenceImageToggle;

    // Use this for initialization
    void Start () {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        model3d = objManager.model3d;
        scrollViewModelsContent = objManager.scrollViewModelsContent;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void GetImage(string path)
    {
        WWW www = new WWW("file://" + path);
        //yield return new WaitForSeconds(1);
        if (www != null && string.IsNullOrEmpty(www.error))
        {
            Texture2D texture = new Texture2D(www.texture.width, www.texture.height);
            Debug.Log("width:" + texture.width);
            Debug.Log("height:" + texture.height);
            texture.SetPixels(www.texture.GetPixels());
            texture.Apply(true);
            texture.filterMode = FilterMode.Trilinear;
            //var image= objManager.contactPoints2d;//contactPoints2d就是那张图像，只是所有2d接触点都挂在这个图像上
            //image.GetComponent<ImageController>().SetImage(texture);
            var imageBackground = objManager.imageBackground;
            imageBackground.GetComponent<RectTransform>().sizeDelta = new Vector2(texture.width, texture.height);
            var image = imageBackground.GetComponent<Image>();
            image.sprite= Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            var color = image.color;
            color.a = 1;
            image.color = color;
            objManager.panelBackgroundImageControllerScript.Init(texture);
        }
        else
        {
            Debug.Log("no such image!");
        }
    }

    private void GetObj(string path)
    {
        WWW www = new WWW("file://" + path);
        //yield return new WaitForSeconds(1);//改成0s可能造成UI不稳定，不知道为啥
        if (www != null && string.IsNullOrEmpty(www.error))
        {
            var re = ObjFormatAnalyzerFactory.AnalyzeToGameObject(path);
            var prefabScrollViewItem = objManager.prefabScrollViewItem;
            foreach (var item in re)
            {
                model3d.GetComponent<Model3dController>().AddSon(item);//将解析出来的obj的父亲设置为model3d                
                item.AddComponent<Model3dItemController>();//添加该脚本
                //var scrollViewItem= UnityEditor.PrefabUtility.InstantiatePrefab(prefabScrollViewItem) as GameObject;                
                var scrollViewItem = Instantiate(prefabScrollViewItem, new Vector3(0, 0, 0), Quaternion.identity);
                scrollViewItem.GetComponent<ScrollViewItemController>().Init(item, scrollViewModelsContent);
            }
        }
        else
        {
            Debug.Log("no such model!");
        }
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
    private Bounds GetBoundsOfVector3Array(Vector3[] v)//根据vector2的数组获得他们的包围盒bounds
    {
        var oo = Macro.oo;
        var MinP = new Vector2(oo, oo);
        var MaxP = new Vector2(-oo, -oo);
        foreach (var i in v)
        {
            MinP = GetMinVector2(MinP, i);
            MaxP = GetMaxVector2(MaxP, i);
        }
        return new Bounds((MinP + MaxP) / 2, MaxP - MinP);
    }

    private void Export3dContactPoints(string path)
    {
        WWW www = new WWW("file://" + path);
        //yield return new WaitForSeconds(1);
        if (www != null && string.IsNullOrEmpty(www.error))
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
            var imageBackground = objManager.imageBackground;
            content += "2d:\r\n";
            foreach (var item in contactPoints.GetComponentsInChildren<Transform>())
            {
                if (item.name.Equals(contactPoints.name))
                    continue;
                var vertices=GetRealBoundsVertices(item.GetComponent<MeshFilter>());
                var bounds = GetBoundsOfVector3Array(vertices);
                //var r = item.sizeDelta.x / 2;
                //var p = item.localPosition;
                ////content += (p.x-r) + " ";
                ////content += (p.y-r) + " ";
                ////content += r+"\r\n";
                //p.x -= r;
                //p.y -= r;
                //var realX = realWidth / nowWidth * p.x + realWidth;//获得像素为单位的接触点x,y,r
                //var realY = realHeight / nowHeight * p.y + realHeight;
                //var realR = realWidth / nowWidth * r;
                //content += realX + " ";
                //content += realY + " ";
                //content += realR + "\r\n";

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
            GetImage(ofn.file);
        }

    }
    public void ButtonImportModelOnClick()//ImportModel按钮被点击
    {
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
            GetObj(ofn.file);
        }

    }
    public void ButtonExportContactPointsOnClick()//ExportContactPoints按钮被点击
    {
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
        if (LocalDialog.GetOpenFileName(ofn))
        {
            Debug.Log("file: " + ofn.file);
            //StartCoroutine(Export3dContactPoints(ofn.file));
            Export3dContactPoints(ofn.file);
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
