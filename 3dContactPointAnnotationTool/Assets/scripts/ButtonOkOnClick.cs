using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Hont;

public class ButtonOkOnClick : MonoBehaviour {
    public InputField inputFieldImagePath;
    public InputField inputField3dModel;
    public GameObject scrollViewContent;//scrollViewModels的content
    public Image referenceImage;  
    public GameObject model3d;
    public GameObject prefabScrollViewItem;
    public Toggle toggleWireframe;
    public Button buttonGCP;

    public void Click() {
        Debug.Log("onClick");
        StartCoroutine(GetImage(inputFieldImagePath.text));
        StartCoroutine(GetObj(inputField3dModel.text));
        //Debug.Log("s off :"+"s off".Contains("f "));
    }
    private IEnumerator GetImage(string path) {
        //Debug.Log(path);
        WWW www = new WWW("file://" + path);
        //Debug.Log(www.url);
        yield return new WaitForSeconds(1);
        if (www != null && string.IsNullOrEmpty(www.error))
        {
            Texture2D texture = new Texture2D(www.texture.width, www.texture.height);
            texture.SetPixels(www.texture.GetPixels());
            texture.Apply(true);
            texture.filterMode = FilterMode.Trilinear;
            referenceImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
        else {
            Debug.Log("no such image!");
        }
    }
    private IEnumerator GetObj(string path) {
        path = @"C:\Users\acer-pc\Desktop\sample.obj";//
        //path = @"C:\Users\A\Desktop\sample.obj";//

        WWW www = new WWW("file://" + path);
        yield return new WaitForSeconds(1);
        if (www != null && string.IsNullOrEmpty(www.error))
        {
            var re = ObjFormatAnalyzerFactory.AnalyzeToGameObject(path);            
            foreach (var item in re) {
                item.transform.SetParent(model3d.transform);//将解析出来的obj的父亲设置为model3d
                item.tag = Macro.UNSELECTED;//将tag设置为未选中
                var scrollViewItem=PrefabUtility.InstantiatePrefab(prefabScrollViewItem) as GameObject;                
                scrollViewItem.GetComponentInChildren<Text>().text=item.name;
                scrollViewItem.GetComponent<ScrollViewItemOnClick>().model = item;//将模型赋值给item的脚本
                scrollViewItem.transform.SetParent(scrollViewContent.transform);//将scrollViewItem添加到scrollView里                                
            }
            toggleWireframe.interactable = true;
            buttonGCP.interactable = true;
        }
        else
        {
            Debug.Log("no such model!");
        }
    }
}
