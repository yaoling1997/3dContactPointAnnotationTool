using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ButtonExportOnClick : MonoBehaviour {
    public InputField InputField;
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
        //path = @"C:\Users\acer-pc\Desktop\2.jpg";//
        //Debug.Log(path);
        WWW www = new WWW("file://" + path);
        yield return new WaitForSeconds(1);
        if (www != null && string.IsNullOrEmpty(www.error))
        {
            string content = "";
            var contactPoints = ObjManager.contactPoints;
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
                content += (p.x-r) + " ";
                content += (p.y-r) + " ";
                content += r+"\r\n";
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
