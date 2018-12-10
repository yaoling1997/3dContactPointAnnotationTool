using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjManager : MonoBehaviour//管理对象，避免找不到active为false的对象的尴尬
{
    public GameObject panelStatus;
    public GameObject prefabScrollViewItem;//scrollViewItem预制件
    public GameObject contactPoints;//接触点们
    public GameObject scrollViewContactPointsContent;//接触点scrollView的content
    public Shader shaderStandard;
    public Shader shaderWireframe;
    public int contactPointId;
    public GameObject contactPoints2d;
    // Use this for initialization
    void Start () {
        contactPointId = 0;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void CreateContactPointSphere(Vector3 center, float x, float y,float z)//x半径,y半径,z半径,创建球状接触点
    {
        contactPointId++;
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.position = center;//设置球的中心位置
        go.transform.localScale = new Vector3(2*x, 2*y, 2*z);//设置球的半径大小,scale是半径的两倍
        go.GetComponent<MeshRenderer>().material.color = Color.red;
        go.transform.SetParent(contactPoints.transform);
        go.name = go.name + contactPointId;
        go.tag = Macro.UNSELECTED;
        //var scrollViewItem = UnityEditor.PrefabUtility.InstantiatePrefab(prefabScrollViewItem) as GameObject;
        var scrollViewItem = Instantiate(prefabScrollViewItem, new Vector3(0, 0, 0), Quaternion.identity);
        scrollViewItem.GetComponent<ScrollViewItemController>().Init(go, scrollViewContactPointsContent, Color.red);
    }
    public void CreateContactPointSphere(Vector3 center, float radius)
    {
        CreateContactPointSphere(center,radius, radius, radius);
    }
    public void CreateContactPointSphere(Vector3 center, Vector3 v)
    {
        CreateContactPointSphere(center, v.x, v.y, v.z);
    }

}
