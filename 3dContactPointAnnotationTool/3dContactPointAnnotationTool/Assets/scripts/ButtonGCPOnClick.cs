using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGCPOnClick : MonoBehaviour {
    public GameObject model3d;//整个场景对象
    public Button buttonGCP;
    private const float oo = 1e18f;
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
                Debug.Log(i/3+"/"+len/3);
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
                        //yield return new WaitForSeconds(0);
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
    private bool isPointInMesh(Vector3 p,Mesh mesh)//判断一个点是否在一个mesh内部
    {
        if (!mesh.bounds.Contains(p))//包围盒不含该点
            return false;
        var triangles = mesh.triangles;
        var vertices = mesh.vertices;
        var normals = mesh.normals;
        var len = triangles.Length;
        for (int i = 0; i < len; i += 3)
        {
            var vMin = new Vector3(oo, oo, oo);
            var vMax = new Vector3(-oo, -oo, -oo);
            for (int j = i; j < i + 3; j++)
            {
                var v = vertices[triangles[j]];
                vMin.x = Mathf.Min(vMin.x, v.x);
                vMin.y = Mathf.Min(vMin.y, v.y);
                vMin.z = Mathf.Min(vMin.z, v.z);
                vMax.x = Mathf.Max(vMax.x, v.x);
                vMax.y = Mathf.Max(vMax.y, v.y);
                vMax.z = Mathf.Max(vMax.z, v.z);
            }
            if (Vector3xydy(vMin, p) && Vector3xydy(p, vMax))
                return true;
        }
        return false;
    }
    private bool Vector3xydy(Vector3 a,Vector3 b)//a<=b
    {
        return a.x <= b.x && a.y <= b.y && a.z <= b.z;
    }
}
