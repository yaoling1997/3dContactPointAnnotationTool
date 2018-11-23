﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEditor;
public class ButtonGCPOnClick : MonoBehaviour {
    struct ListNode
    {
        public Bounds objBounds;
        public List<Bounds> triangleBoundsList;
        public ListNode(Bounds bounds,List<Bounds> list)
        {
            triangleBoundsList = list;
            objBounds = bounds;
        }
    }
    private const float oo = 1e18f;
    public GameObject model3d;//整个场景对象
    public GameObject contactPoints;//所有接触点
    public Button buttonGCP;
    public GameObject scrollViewContent;//scrollViewContactPoints的content    
    public Slider sliderGCP;//显示GCP进度的slider
    public Text textTimeCost;//显示计算过程最终花费的时间
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
    private Bounds GetBoundsOfVector3Array(Vector3 []v)//根据vector3的数组获得他们的包围盒bounds
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
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();//记录开始时间
        sw.Start();
        textTimeCost.text = "";//重新显示时间
        sliderGCP.value = 0;//进度条归零
        var selectedObjList = new List<ListNode>();//选中对象们
        var unselectedObjList = new List<ListNode>();//未选中对象们
        int totalSelectedObjTriangleNum = 0;//总的选中物体三角面片数,用来控制进度条显示
        yield return new WaitForSeconds(0);
        foreach (var item in model3d.GetComponentsInChildren<MeshFilter>())//获得所有物体每个三角面片包围盒
        {
            var vertices = GetRealVertices(item);
            var triangles = item.mesh.triangles;
            var boundsList = new List<Bounds>();
            var objBounds = GetBoundsOfVector3Array(vertices);
            if (item.tag.Equals(Macro.SELECTED))
            {                
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    Vector3 []tmp = { vertices[triangles[i]], vertices[triangles[i + 1]], vertices[triangles[i + 2]] };
                    boundsList.Add(GetBoundsOfVector3Array(tmp));
                }
                selectedObjList.Add(new ListNode(objBounds, boundsList));
                totalSelectedObjTriangleNum += boundsList.Count;
            }
            else if (item.tag.Equals(Macro.UNSELECTED))
            {
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    Vector3[] tmp = { vertices[triangles[i]], vertices[triangles[i + 1]], vertices[triangles[i + 2]] };
                    boundsList.Add(GetBoundsOfVector3Array(tmp));
                }
                unselectedObjList.Add(new ListNode(objBounds, boundsList));
            }
        }
        yield return new WaitForSeconds(0);
        Debug.Log("selectedObjNum: " + selectedObjList.Count);
        Debug.Log("unselectedObjNum: " + unselectedObjList.Count);
        int contactPointNum = 0;
        float sliderValueFz=0;//slider value的分子
        foreach (var so in selectedObjList)
        {
            foreach (var uso in unselectedObjList) {
                if (!dealBoundsIntersection(so.objBounds, uso.objBounds,false))//两物体的包围盒不相交
                {
                    sliderValueFz += so.triangleBoundsList.Count;
                    continue;
                }
                foreach(var sotb in so.triangleBoundsList)
                {
                    sliderValueFz++;
                    if (!dealBoundsIntersection(sotb, uso.objBounds,false))//选中物体三角面片包围盒不与未选中物体包围盒相交
                        continue;
                    foreach(var usotb in uso.triangleBoundsList)
                    {
                        if (dealBoundsIntersection(sotb, usotb,true))
                        {
                            contactPointNum++;
                        }
                    }
                    var tmpV = sliderValueFz / (unselectedObjList.Count * totalSelectedObjTriangleNum);
                    if (tmpV- sliderGCP.value >= 0.05) {
                        sliderGCP.value = tmpV;//更新进度条的值
                        yield return new WaitForSeconds(0);
                    }
                }
            }
        }
        sliderGCP.value = 1;//计算完毕，进度条的值置为1
        Debug.Log("contactPointNum: "+ contactPointNum);        
        buttonGCP.interactable = true;//计算完成可以点击按钮        
        sw.Stop();
        ShowTime(sw.Elapsed.TotalSeconds);
    }
    private void ShowTime(double timeCost)//计算完毕，显示计算总耗时
    {
        int minuteCost = (int)(timeCost / 60);
        int secondCost = (int)(timeCost - minuteCost * 60);
        textTimeCost.text = "";
        if (minuteCost > 0)
            textTimeCost.text += minuteCost + "′";
        textTimeCost.text +=  secondCost + "″";
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
    private bool dealBoundsIntersection(Bounds a,Bounds b,bool ifCreatePoint)//是否创建对应接触点
    {
        var minP = GetMaxVector3(a.min, b.min);
        var maxP = GetMinVector3(a.max, b.max);
        if (!Vector3xydy(minP, maxP))
            return false;
        if (ifCreatePoint)
            objManager.GetComponent<ObjManager>().CreateContactPointSphere((minP+maxP)/2,(maxP-minP)/2);
        return true;
    }
    private bool Vector3xydy(Vector3 a,Vector3 b)//a<=b
    {
        return a.x <= b.x && a.y <= b.y && a.z <= b.z;
    }
}
