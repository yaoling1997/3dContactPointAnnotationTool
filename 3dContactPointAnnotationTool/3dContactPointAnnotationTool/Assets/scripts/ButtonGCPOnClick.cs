using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGCPOnClick : MonoBehaviour {
    public GameObject model3d;//整个场景对象
    public Button buttonGCP;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	}

    private IEnumerator SolveContactPoints()//对选中的模型计算接触点
    {
        yield return new WaitForSeconds(0);        
        buttonGCP.interactable = false;//计算的时候不能再点击按钮
        var selectedObjList = new List<MeshFilter>();
        var unselectedObjList = new List<MeshFilter>();
        foreach (var item in model3d.GetComponentsInChildren<MeshFilter>())
        {
            if (item.tag.Equals(Macro.SELECTED))
            {
                selectedObjList.Add(item);
            }
            else if (item.tag.Equals(Macro.UNSELECTED))
            {
                unselectedObjList.Add(item);
            }
        }
        yield return new WaitForSeconds(0);
        Debug.Log("size of selectedObjList: "+selectedObjList.Count);
        Debug.Log("size of unselectedObjList: " + unselectedObjList.Count);
        int contactPointNum = 0;
        foreach (var so in selectedObjList)
        {
            var mesh = so.mesh;
            var len = mesh.triangles.Length;

            //for (int i = 0; i < len; i += 3)
            //{
            //    var a = mesh.vertices[mesh.triangles[i]];
            //    var b = mesh.vertices[mesh.triangles[i + 1]];
            //    var c = mesh.vertices[mesh.triangles[i + 2]];
            //    var fn = new Vector3(0, 0, 0);
            //    var n = Vector3.Cross(b - a, c - a).normalized;
            //    var ct = (a + b + c) / 3;
            //    Debug.DrawLine(ct, ct + n / 10, Color.red, 10000);
            //    Debug.DrawLine(ct, ct + fn / 10, Color.yellow, 10000);
            //    CreateContactPointSphere(ct, 0.01f);
            //}
            //break;
            for (int i = 0; i < len; i += 3)
            {
                float[] weight = { 1, 1, 1 };
                float totalWeight = 0;
                bool xj = false;
                for (int j = 0; j < 3; j++)
                {
                    foreach (var uso in unselectedObjList)
                    {                        
                        if (isPointInMesh(mesh.vertices[mesh.triangles[i+j]],uso.mesh))
                        {
                            xj = true;
                            //weight[j] += 2;
                        }
                        yield return new WaitForSeconds(0);
                    }
                }
                if (xj)
                {
                    contactPointNum++;
                    var center = new Vector3(0, 0, 0);
                    float radius = 0;
                    for (int j = 0; j < 3; j++)
                    {
                        CreateContactPointSphere(mesh.vertices[mesh.triangles[i + j]], 0.01f);
                        center += weight[j] * mesh.vertices[mesh.triangles[i + j]];
                        totalWeight += weight[j];
                    }
                    center /= totalWeight;
                    for (int j = 0; j < 3; j++)
                    {
                        radius += weight[j] * (mesh.vertices[mesh.triangles[i + j]] - center).magnitude;
                    }
                    radius /= totalWeight;
                    CreateContactPointSphere(center, radius);
                    yield return new WaitForSeconds(0.1f);//求出一个接触点就显示一下
                }
            }
        }
        Debug.Log("contactPointNum: "+ contactPointNum);        
        buttonGCP.interactable = true;//计算完成可以点击按钮
    }
    private void CreateContactPointSphere(Vector3 center,float radius) {//创建球状接触点
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.position = center;//设置球的中心位置
        go.transform.localScale = new Vector3(radius, radius, radius);//设置球的半径大小
        go.GetComponent<MeshRenderer>().material.color = Color.red;
    }
    public void Click()//按钮点击会调用
    {
        StartCoroutine(SolveContactPoints());
    }
    private bool ifTriXj(Vector3 a, Vector3 b, Vector3 Vc,Vector3 O,Vector3 sx) {//从O处
        return false;
    }
    private bool isPointInMesh(Vector3 p,Mesh mesh)//判断一个点是否在一个mesh内部
    {
        var triangles = mesh.triangles;
        var vertices = mesh.vertices;
        var normals = mesh.normals;
        var len = triangles.Length;
        bool re = false;
        var sx = new Vector3(0,1,0);
        for (int i = 0; i < len; i += 3)
        {
            var a = vertices[triangles[i]];
            var b = vertices[triangles[i+1]];
            var c = vertices[triangles[i+2]];
            if (ifTriXj(a, b, c, p,sx))
                re = !re;
        }
        return re;
    }
}
