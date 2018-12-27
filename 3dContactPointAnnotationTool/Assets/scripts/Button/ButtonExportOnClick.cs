using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ButtonExportOnClick : MonoBehaviour {
    public InputField InputField;//导出路径
    public Image image;//人和物的图像
    private ObjManager ObjManager;
	// Use this for initialization
	void Start () {
        ObjManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private IEnumerator Export3dContactPoints(string path)
    {
        WWW www = new WWW("file://" + path);
        yield return new WaitForSeconds(1);
        if (www != null && string.IsNullOrEmpty(www.error))
        {
            string content = "";
            var contactPoints = ObjManager.contactPoints;
            var imageRt = image.GetComponent<RectTransform>().sizeDelta;
            var realWidth = image.mainTexture.width;//真实图像像素宽
            var realHeight = image.mainTexture.height;//真实图像像素高
            var nowWidth = imageRt.x;//工具中显示的图像的宽
            var nowHeight = imageRt.y;//工具中显示的图像的高
            Debug.Log("width:" + realWidth);
            Debug.Log("height:" + realHeight);
            content += "3d:\r\n";
            foreach(var item in contactPoints.GetComponentsInChildren<Transform>())
            {
                if (item.name.Equals(contactPoints.name))
                    continue;
                var p = item.position;
                var s = item.localScale;
                content += p.x + " ";
                content += p.y + " ";
                content += p.z + " ";
                content += s.x/2 + " ";
                content += s.y/2 + " ";
                content += s.z/2 + "\r\n";
                //content += System.Math.Round(p.x,4) + " ";
                //content += System.Math.Round(p.y,4) + " ";
                //content += System.Math.Round(p.z,4) + " ";
                //content += System.Math.Round(s.x,4) + " ";
                //content += System.Math.Round(s.y,4) + " ";
                //content += System.Math.Round(s.z,4) + "\r\n";
            }
            var contactPoints2d= ObjManager.contactPoints2d;
            content += "2d:\r\n";
            foreach(var item in contactPoints2d.GetComponentsInChildren<RectTransform>())
            {
                if (item.name.Equals(contactPoints2d.name))
                    continue;
                var r = item.sizeDelta.x/2;
                var p = item.localPosition;
                //content += (p.x-r) + " ";
                //content += (p.y-r) + " ";
                //content += r+"\r\n";
                p.x -= r;
                p.y -= r;
                var realX = realWidth / nowWidth * p.x+ realWidth;//获得像素为单位的接触点x,y,r
                var realY = realHeight / nowHeight * p.y + realHeight;
                var realR = realWidth / nowWidth * r;
                content += realX + " ";
                content += realY + " ";
                content += realR + "\r\n";

            }
            File.WriteAllText(path,content);
        }
        else
        {
            Debug.Log("no such txt!");
        }
    }

    public void OnClick()
    {
        StartCoroutine(Export3dContactPoints(InputField.text));
    }
}
