using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEditor;
public class ButtonGCPOnClick : MonoBehaviour {
    public GameObject model3d;//整个场景对象
    public GameObject contactPoints;//所有接触点
    public Button buttonGCP;
    public GameObject scrollViewContent;//scrollViewContactPoints的content    
    private GameObject objManager;

    private const float oo = 1e18f;
	// Use this for initialization
	void Start () {
        objManager = GameObject.Find("ObjManager");        
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
        var unselectedObjRealVerticesList = new List<Vector3[]>();
        foreach (var item in model3d.GetComponentsInChildren<MeshFilter>())
        {
            if (item.tag.Equals(Macro.SELECTED))
            {
                selectedObjList.Add(item);
            }
            else if (item.tag.Equals(Macro.UNSELECTED))
            {
                unselectedObjList.Add(item);
                unselectedObjRealVerticesList.Add(GetRealVertices(item));
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
            var vertices = GetRealVertices(so);

            for (int i = 0; i < len; i += 3)
            {
                Debug.Log(i/3+"/"+len/3);
                float[] weight = { 1, 1, 1 };
                float totalWeight = 0;
                bool xj = false;
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < unselectedObjList.Count; k++) {
                        var uso = unselectedObjList[k];
                        var usoVertices = unselectedObjRealVerticesList[k];//获取真实点
                        if (isPointInMesh(vertices[mesh.triangles[i+j]],uso, usoVertices))
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
                        center += weight[j] * vertices[mesh.triangles[i + j]];
                        totalWeight += weight[j];
                    }
                    center /= totalWeight;
                    for (int j = 0; j < 3; j++)
                    {
                        radius += weight[j] * (vertices[mesh.triangles[i + j]] - center).magnitude;
                    }
                    radius /= totalWeight;
                    objManager.GetComponent<ObjManager>().CreateContactPointSphere(center, radius);
                    yield return new WaitForSeconds(0.1f);//求出一个接触点就显示一下
                }
            }
        }
        Debug.Log("contactPointNum: "+ contactPointNum);        
        buttonGCP.interactable = true;//计算完成可以点击按钮
    }
    private Vector3[] GetRealVertices(MeshFilter meshFilter)//将点变换到真实的点（应用位移、旋转、缩放变换）
    {
        var len = meshFilter.mesh.vertices.Length;
        var vertices = new Vector3[len];
        var transform = meshFilter.transform;
        Quaternion rotation = Quaternion.Euler(transform.eulerAngles);
        Matrix4x4 m = Matrix4x4.TRS(transform.position,rotation,transform.localScale);
        for (int i = 0; i < len; i++)
        {
            vertices[i] = m.MultiplyPoint3x4(meshFilter.mesh.vertices[i]);
        }
        return vertices;
    }
    public void Click()//按钮点击会调用
    {
        StartCoroutine(SolveContactPoints());
    }
    private bool isPointInMesh(Vector3 p,MeshFilter meshFilter,Vector3 []vertices)//判断一个点是否在一个mesh内部
    {
        var mesh = meshFilter.mesh;
        if (!mesh.bounds.Contains(p))//包围盒不含该点
            return false;
        var triangles = mesh.triangles;
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
            if (Vector3xydy(vMin, p) && Vector3xydy(p, vMax))//在某个三角形的包围盒中
                return true;
        }

        return false;
    }
    private bool Vector3xydy(Vector3 a,Vector3 b)//a<=b
    {
        return a.x <= b.x && a.y <= b.y && a.z <= b.z;
    }
}
