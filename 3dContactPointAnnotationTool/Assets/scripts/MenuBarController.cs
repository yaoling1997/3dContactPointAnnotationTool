using System.Collections;
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
            var image= objManager.contactPoints2d;//contactPoints2d就是那张图像，只是所有2d接触点都挂在这个图像上
            image.GetComponent<ImageController>().SetImage(texture);
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
    private void Export3dContactPoints(string path)
    {
        WWW www = new WWW("file://" + path);
        //yield return new WaitForSeconds(1);
        if (www != null && string.IsNullOrEmpty(www.error))
        {
            string content = "";
            var contactPoints = objManager.contactPoints;
            var image = objManager.contactPoints2d.GetComponent<Image>();
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
                var s = item.localScale;
                content += p.x + " ";
                content += p.y + " ";
                content += p.z + " ";
                content += s.x / 2 + " ";
                content += s.y / 2 + " ";
                content += s.z / 2 + "\r\n";
                //content += System.Math.Round(p.x,4) + " ";
                //content += System.Math.Round(p.y,4) + " ";
                //content += System.Math.Round(p.z,4) + " ";
                //content += System.Math.Round(s.x,4) + " ";
                //content += System.Math.Round(s.y,4) + " ";
                //content += System.Math.Round(s.z,4) + "\r\n";
            }
            var contactPoints2d = objManager.contactPoints2d;
            content += "2d:\r\n";
            foreach (var item in contactPoints2d.GetComponentsInChildren<RectTransform>())
            {
                if (item.name.Equals(contactPoints2d.name))
                    continue;
                var r = item.sizeDelta.x / 2;
                var p = item.localPosition;
                //content += (p.x-r) + " ";
                //content += (p.y-r) + " ";
                //content += r+"\r\n";
                p.x -= r;
                p.y -= r;
                var realX = realWidth / nowWidth * p.x + realWidth;//获得像素为单位的接触点x,y,r
                var realY = realHeight / nowHeight * p.y + realHeight;
                var realR = realWidth / nowWidth * r;
                content += realX + " ";
                content += realY + " ";
                content += realR + "\r\n";

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
    public void ButtonReferenceImageOnClick()//ReferenceImage按钮被点击
    {
        var panelReferenceImage = objManager.panelReferenceImage;
        var active = !panelReferenceImage.activeSelf;
        panelReferenceImage.SetActive(active);
        WindowReferenceImageToggle.isOn = active;
    }
}
