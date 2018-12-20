using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hont;

public class ButtonOkOnClick : MonoBehaviour {
    public InputField inputFieldImagePath;
    public InputField inputField3dModel;
    public GameObject scrollViewModelsContent;//scrollViewModels的content
    public GameObject image;//panelImage的image
    public GameObject model3d;    
    public Toggle toggleWireframe;
    public Button buttonGCP;

    private ObjManager objManager;

    private void Start()
    {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
    }
    public void Click() {
        Debug.Log("onClick");
        StartCoroutine(GetImage(inputFieldImagePath.text));
        StartCoroutine(GetObj(inputField3dModel.text));
    }
    private IEnumerator GetImage(string path) {
        //path = @"C:\Users\acer-pc\Desktop\2.jpg";//
        //Debug.Log(path);
        WWW www = new WWW("file://" + path);
        yield return new WaitForSeconds(1);
        if (www != null && string.IsNullOrEmpty(www.error))
        {
            Texture2D texture = new Texture2D(www.texture.width, www.texture.height);
            texture.SetPixels(www.texture.GetPixels());
            texture.Apply(true);
            texture.filterMode = FilterMode.Trilinear;
            image.GetComponent<ImageController>().SetImage(texture);
        }
        else {
            Debug.Log("no such image!");
        }
    }
    private IEnumerator GetObj(string path) {
        //path = @"C:\Users\acer-pc\Desktop\sample.obj";//
        //path = @"C:\Users\A\Desktop\sample.obj";//

        WWW www = new WWW("file://" + path);
        yield return new WaitForSeconds(1);
        if (www != null && string.IsNullOrEmpty(www.error))
        {
            var re = ObjFormatAnalyzerFactory.AnalyzeToGameObject(path);
            var prefabScrollViewItem = objManager.prefabScrollViewItem;
            foreach (var item in re) {
                model3d.GetComponent<Model3dController>().AddSon(item);//将解析出来的obj的父亲设置为model3d
                item.tag = Macro.UNSELECTED;//将tag设置为未选中
                item.AddComponent<Model3dItemController>();//添加该脚本
                //var scrollViewItem= UnityEditor.PrefabUtility.InstantiatePrefab(prefabScrollViewItem) as GameObject;                
                var scrollViewItem = Instantiate(prefabScrollViewItem,new Vector3(0,0,0),Quaternion.identity);
                scrollViewItem.GetComponent<ScrollViewItemController>().Init(item, scrollViewModelsContent);
            }
        }
        else
        {
            Debug.Log("no such model!");
        }
    }
}
