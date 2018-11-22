using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEditor;
public class ButtonGCPOnClick : MonoBehaviour {
    private const float oo = 1e18f;
    public GameObject model3d;//整个场景对象
    public GameObject contactPoints;//所有接触点
    public Button buttonGCP;
    public GameObject scrollViewContent;//scrollViewContactPoints的content    
    private GameObject objManager;
    
	// Use this for initialization
	void Start () {
        objManager = GameObject.Find("ObjManager");        
    }
	
	// Update is called once per frame
	void Update () {
	}
    private Vector3 GetMinVector3(Vector3 a,Vector3 b)//每一维取最小的
    {
        var re = new Vector3(0, 0, 0);
        re.x = Mathf.Min(a.x, b.x);
        re.y = Mathf.Min(a.y, b.y);
        re.z = Mathf.Min(a.z, b.z);
        return re;
    }
    private Vector3 GetMaxVector3(Vector3 a, Vector3 b)//每一维取最大的
    {
        var re = new Vector3(0, 0, 0);
        re.x = Mathf.Max(a.x, b.x);
        re.y = Mathf.Max(a.y, b.y);
        re.z = Mathf.Max(a.z, b.z);
        return re;
    }
    private Bounds GetBoundsOfTriangle(Vector3 []v)
    {
        var MinP = new Vector3(oo, oo, oo);
        var MaxP = new Vector3(-oo, -oo, -oo);
        foreach (var i in v){
            MinP = GetMinVector3(MinP, i);
            MaxP = GetMaxVector3(MaxP, i);
        }
        return new Bounds((MinP+MaxP)/2,MaxP-MinP);
    }
    private IEnumerator SolveContactPoints()//对选中的模型计算接触点
    {
        yield return new WaitForSeconds(0);        
        buttonGCP.interactable = false;//计算的时候不能再点击按钮
        var selectedObjTriangleBounds = new List<Bounds>();//选中对象们每个三角面片的Bounds
        var unselectedObjTriangleBounds = new List<Bounds>();//未选中对象们每个三角面片的Bounds
        int selectedObjNum = 0;
        int unselectedObjNum = 0;
        foreach (var item in model3d.GetComponentsInChildren<MeshFilter>())//获得所有物体每个三角面片包围盒
        {
            var vertices = GetRealVertices(item);
            if (item.tag.Equals(Macro.SELECTED))
            {
                selectedObjNum++;
                for (int i = 0; i < item.mesh.triangles.Length; i += 3)
                {
                    Vector3 []tmp = { vertices[i], vertices[i + 1], vertices[i + 2] };
                    selectedObjTriangleBounds.Add(GetBoundsOfTriangle(tmp));
                }
            }
            else if (item.tag.Equals(Macro.UNSELECTED))
            {
                unselectedObjNum++;
                for (int i = 0; i < item.mesh.triangles.Length; i += 3)
                {
                    Vector3[] tmp = { vertices[i], vertices[i + 1], vertices[i + 2] };
                    unselectedObjTriangleBounds.Add(GetBoundsOfTriangle(tmp));
                }
            }
        }
        yield return new WaitForSeconds(0);
        Debug.Log("selectedObjNum: " + selectedObjNum);
        Debug.Log("unselectedObjNum: " + unselectedObjNum);
        int contactPointNum = 0;
        foreach(var sb in selectedObjTriangleBounds)
        {
            foreach(var usb in unselectedObjTriangleBounds)
            {
                if (dealBoundsIntersection(sb, usb))
                {
                    contactPointNum++;
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
    private bool dealBoundsIntersection(Bounds a,Bounds b)
    {
        var minP = GetMaxVector3(a.min, b.min);
        var maxP = GetMinVector3(a.max, b.max);
        if (!Vector3xydy(minP, maxP))
            return false;
        objManager.GetComponent<ObjManager>().CreateContactPointSphere((minP+maxP)/2,(maxP-minP)/2);
        return true;
    }
    private bool Vector3xydy(Vector3 a,Vector3 b)//a<=b
    {
        return a.x <= b.x && a.y <= b.y && a.z <= b.z;
    }
}
