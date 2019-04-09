using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEditor;
public class ButtonGCPOnClick : MonoBehaviour {
    public const int MAX_CONTACT_POINT_NUM= 100;//自动计算最多生成多少个接触点
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
    public class BoundsComparer : IComparer<Bounds>
    {
        public int Compare(Bounds x, Bounds y)
        {
            var X = x.center;
            var Y = y.center;
            if (X.Equals(Y))
                return 0;
            if (X.x < Y.x)
                return -1;
            else if (X.x == Y.x && X.y < Y.y)
                return -1;
            else if (X.x == Y.x && X.y == Y.y && X.z < Y.z)
                return -1;
            else return 1;
        }
    }
    public class Cost
    {
        public float cost;
        public int id;//第几个Cost
        public Cost(float cost, int id)
        {
            this.cost = cost;
            this.id = id;
        }
    }
    public class CostComparer : IComparer<Cost>
    {
        public int Compare(Cost x, Cost y)
        {
            if (x.cost == y.cost && x.id == y.id)
                return 0;
            if (x.cost > y.cost || (x.cost == y.cost && x.id < y.id))
                return 1;
            else
                return -1;
        }
    }

    private const float eps = 1e-7f;
    private float oo;
    public GameObject model3d;//整个场景对象
    public GameObject contactPoints;//所有接触点
    public Button buttonGCP;
    public GameObject scrollViewContent;//scrollViewContactPoints的content    
    public Slider sliderGCP;//显示GCP进度的slider
    public Text textTimeCost;//显示计算过程最终花费的时间
    private ObjManager objManager;
    //private List<Bounds> xjBoundsList;//不同物体相交的区域    

	// Use this for initialization
	void Start () {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        oo = Macro.oo;
    }
	
	// Update is called once per frame
	void Update () {
	}
    private int Dcmp(float x)
    {
        if (Mathf.Abs(x) < eps)
            return 0;
        return x < 0 ? -1 : 1;
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
        int totalSelectedObjVerticeNum = 0;//总的选中物体顶点数
        int totalUnselectedObjVerticeNum = 0;//总的未选中物体顶点数
        int totalSelectedObjTriangleNum = 0;//总的选中物体三角面片数,用来控制进度条显示
        int totalUnselectedObjTriangleNum = 0;//总的未选中物体三角面片数
        yield return new WaitForSeconds(0);
        foreach (var item in model3d.GetComponentsInChildren<SkinnedMeshRenderer>())//获得所有物体每个三角面片包围盒
        {
            var vertices = ObjManager.GetRealVertices(item);
            var triangles = item.sharedMesh.triangles;
            var boundsList = new List<Bounds>();
            var objBounds =  ObjManager.GetBoundsOfVector3Array(vertices);

            if (item.tag.Equals(Macro.SELECTED))
            {
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    Vector3[] tmp = { vertices[triangles[i]], vertices[triangles[i + 1]], vertices[triangles[i + 2]] };
                    boundsList.Add(ObjManager.GetBoundsOfVector3Array(tmp));
                }
                selectedObjList.Add(new ListNode(objBounds, boundsList));
                totalSelectedObjVerticeNum += vertices.Length;
                totalSelectedObjTriangleNum += triangles.Length;
            }
            else if (item.tag.Equals(Macro.UNSELECTED))
            {
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    Vector3[] tmp = { vertices[triangles[i]], vertices[triangles[i + 1]], vertices[triangles[i + 2]] };
                    boundsList.Add(ObjManager.GetBoundsOfVector3Array(tmp));
                }
                unselectedObjList.Add(new ListNode(objBounds, boundsList));
                totalUnselectedObjVerticeNum += vertices.Length;
                totalUnselectedObjTriangleNum += triangles.Length;
            }
        }
        yield return new WaitForSeconds(0);
        Debug.Log("selectedObjNum: " + selectedObjList.Count);
        Debug.Log("unselectedObjNum: " + unselectedObjList.Count);
        Debug.Log("totalSelectedObjVerticeNum: " + totalSelectedObjVerticeNum);
        Debug.Log("totalSelectedObjVerticeNum: " + totalUnselectedObjVerticeNum);
        Debug.Log("totalSelectedObjTriangleNum: " + totalSelectedObjTriangleNum);
        Debug.Log("totalUnselectedObjTriangleNum: " + totalUnselectedObjTriangleNum);
        float sliderValueFz = 0;//slider value的分子
        var xjBoundsList = new List<Bounds>();//不同物体相交的区域 

        foreach (var so in selectedObjList)//求交
        {
            foreach (var uso in unselectedObjList)
            {
                if (!DealBoundsIntersection(so.objBounds, uso.objBounds, false, xjBoundsList))//两物体的包围盒不相交
                {
                    sliderValueFz += so.triangleBoundsList.Count;
                    continue;
                }
                foreach (var sotb in so.triangleBoundsList)
                {
                    sliderValueFz++;
                    if (!DealBoundsIntersection(sotb, uso.objBounds, false, xjBoundsList))//选中物体三角面片包围盒不与未选中物体包围盒相交
                        continue;
                    foreach (var usotb in uso.triangleBoundsList)
                    {
                        if (DealBoundsIntersection(sotb, usotb, true, xjBoundsList))
                        {

                        }
                    }
                    var tmpV = sliderValueFz*3 / (unselectedObjList.Count * totalSelectedObjTriangleNum);
                    if (tmpV - sliderGCP.value >= 0.05)
                    {
                        sliderGCP.value = tmpV;//更新进度条的值
                        yield return new WaitForSeconds(0);
                    }
                }
            }
        }

        var contactPointNum = CreateContactPoints(xjBoundsList);//显示接触点
        sliderGCP.value = 1;//计算完毕，进度条的值置为1
        Debug.Log("contactPointNum: " + contactPointNum);
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
    public void Click()//按钮点击会调用
    {
        StartCoroutine(SolveContactPoints());
    }
    private bool DealBoundsIntersection(Bounds a,Bounds b,bool ifCreateXjBounds,List<Bounds> xjBoundsList)//是否创建对应相交长方体
    {
        var minP = ObjManager.GetMaxVector3(a.min, b.min);
        var maxP = ObjManager.GetMinVector3(a.max, b.max);
        if (!Vector3xydy(minP, maxP))
            return false;
        if (ifCreateXjBounds)
        {
            xjBoundsList.Add(new Bounds((minP + maxP) / 2, maxP - minP));//往xjBoundsList添加一个相交长方体
        }
        return true;
    }
    private List<Bounds> MergeSameCenterBounds(List<Bounds> boundsList) {
        var re = new List<Bounds>();
        boundsList.Sort(new BoundsComparer());
        bool isFirst = true;//第一个是初始bounds不要生成接触点
        var bounds = new Bounds(new Vector3(-oo, -oo, -oo), new Vector3(0, 0, 0));
        int cnt = 0;//接触点数量
        foreach (var item in boundsList)
        {
            //Debug.Log("center:"+item.center.x+ ","+item.center.y + "," + item.center.z);
            var tmp = item.center - bounds.center;
            if (Dcmp(tmp.x) == 0 && Dcmp(tmp.y) == 0 && Dcmp(tmp.z) == 0)
            {
                bounds.extents = ObjManager.GetMaxVector3(bounds.extents, item.extents);
            }
            else
            {
                if (!isFirst)
                {
                    re.Add(bounds);
                    cnt++;
                }
                isFirst = false;
                bounds = item;
            }
        }
        if (!isFirst)
        {
            re.Add(bounds);
            cnt++;
        }
        return re;
    }
    private float BoundsToBoundsCost(Bounds u,Bounds v) {
        return (u.center - v.center).magnitude - u.extents.magnitude - v.extents.magnitude;//包围盒外接球球心距离减去半径距离
    }
    private List<Bounds> MergeContactPointsToMaxNum(List<Bounds> boundsList) {//将接触点限制到最大数量内
        if (boundsList.Count <= MAX_CONTACT_POINT_NUM)//未达到数量直接返回
            return boundsList;
        var re = new List<Bounds>();
        var sd = new SortedDictionary<Cost, int[]>(new CostComparer());
        var f = new List<int>();//是否被合并的标志
        int id = 0;//新加入sd的cost的id,SortedDictionary的key不能有相同的元素
        for (int i = 0; i < boundsList.Count; i++) {
            f.Add(i);
            for (int j = i + 1; j < boundsList.Count; j++) {
                var cost = BoundsToBoundsCost(boundsList[i], boundsList[j]);
                sd.Add(new Cost(cost,++id),new int[] {i , j});
            }
        }
        int cnt = boundsList.Count;//还剩下多少接触点
        while (cnt > MAX_CONTACT_POINT_NUM) {
            var e = sd.GetEnumerator();
            e.MoveNext();
            var ec = e.Current;
            int[] pair = ec.Value;
            sd.Remove(ec.Key);
            if (f[pair[0]] != pair[0] || f[pair[1]] != pair[1])//该点已被合并
                continue;
            var u = boundsList[pair[0]];
            var v = boundsList[pair[1]];
            var vector3s = new Vector3[] { u.max,u.min,v.max,v.min };
            var newBounds = ObjManager.GetBoundsOfVector3Array(vector3s);
            boundsList.Add(newBounds);
            f[pair[0]] = f.Count;//合并老点
            f[pair[1]] = f.Count;
            for (int i = 0; i < f.Count; i++) {//计算与其它球的cost
                if (f[i] != i)
                    continue;
                var cost = BoundsToBoundsCost(boundsList[i],newBounds);//包围盒外接球球心距离减去半径距离
                sd.Add(new Cost(cost, ++id), new int[] { i, f.Count });
            }
            f.Add(f.Count);//创建新点            
            cnt--;
        }
        for (int i = 0; i < f.Count; i++) {
            if (i == f[i])
                re.Add(boundsList[i]);
        }
        return re;
    }
    private int CreateContactPoints(List<Bounds> xjBoundsList)//创建接触点，把中心相同的接触点合并
    {
        var boundsList = MergeSameCenterBounds(xjBoundsList);
        //boundsList = MergeContactPointsToMaxNum(boundsList);
        objManager.Log("contactPoint Num:"+ boundsList.Count);
        foreach (var item in boundsList) {
            objManager.CreateContactPointSphere(item.center,item.extents);
        }
        Debug.Log("old length: " + xjBoundsList.Count);
        Debug.Log("new length: " + boundsList.Count);
        return boundsList.Count;
    }
    public static bool Vector3xydy(Vector3 a,Vector3 b)//a<=b,x,y,z都小于等于
    {
        return a.x <= b.x && a.y <= b.y && a.z <= b.z;
    }
}
