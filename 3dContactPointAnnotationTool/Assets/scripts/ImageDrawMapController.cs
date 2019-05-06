using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ImageDrawMapController : MonoBehaviour, IDragHandler
{
    private ObjManager objManager;
    private Image imageDraw;//用来画2d接触区域的画布
    private int[][,] area;//区域
    //private int[,] contactPointArea;//计算出来的2d接触点覆盖区域
    private Texture2D t2d;
    private int width;
    private int height;
    private List<Vector2> move;
    private readonly int radius = 2;
    private Color[] colorSet = new Color[] {Color.red,Color.blue };//不同区域染色用的颜色
    public int areaId;//现在选的是哪一个area的下标
    public bool ifActive;//是否启用该功能
    private void Awake()
    {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        imageDraw = GetComponent<Image>();
        move = new List<Vector2>();
        for (int i = -radius; i <= radius; i++)
            for (int j = -radius; j <= radius; j++) {
                var p = new Vector2(i,j);
                if (p.magnitude <= radius) {
                    move.Add(p);
                }
            }
        InitDrawMap(1, 1);
        areaId = 0;
        ifActive = false;
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void InitDrawMap(int width,int height) {//导入图片时重置drawMap分辨率
        t2d = new Texture2D(width, height);
        this.width = width;
        this.height = height;
        imageDraw.sprite = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), Vector2.zero);
        area = new int[2][,];
        area[0] = new int[width,height];
        area[1] = new int[width, height];
        //contactPointArea = new int[width, height];
        ClearDrawMap();
    }
    public void ClearDrawMap() {//清空显示的texture2d和数组
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                area[0][i, j] = area[1][i, j]= 0;
                //contactPointArea[i, j] = 0;
                t2d.SetPixel(i, j, Color.clear);
            }
        t2d.Apply();
    }
    public void ClearNowDrawMap() {//清空当前选择的map
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                area[areaId][i, j] = 0;
                //contactPointArea[i, j] = 0;
                if (area[1 - areaId][i, j] == 1)
                    t2d.SetPixel(i,j,colorSet[1-areaId]);
                else
                    t2d.SetPixel(i, j, Color.clear);
            }
        t2d.Apply();
    }
    public void FillDrawMap() {//画好后填充包围部分,x右,y上
        var nowArea = area[areaId];
        var vis = new int[width,height];
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++) {
                vis[i, j] = nowArea[i,j]==1?1:0;                
            }
        var Q = new Queue<Vector2>();
        for (int i = 0; i < width; i++) {
            if (nowArea[i, 0] != 1) {
                Q.Enqueue(new Vector2(i,0));
                vis[i, 0] = -1;
            }
            if (nowArea[i, height-1] != 1)
            {
                Q.Enqueue(new Vector2(i, height-1));
                vis[i, height-1] = -1;
            }
        }
        for (int i = 0; i < height; i++)
        {
            if (nowArea[0, i] != 1)
            {
                Q.Enqueue(new Vector2(0, i));
                vis[0, i] = -1;
            }
            if (nowArea[width-1, i] != 1)
            {
                Q.Enqueue(new Vector2(width-1, i));
                vis[width-1, i] = -1;
            }
        }
        var move = new Vector2[] {
            new Vector2(-1,0),
            new Vector2(1,0),
            new Vector2(0,-1),
            new Vector2(0,1)
        };
        while (Q.Count > 0) {
            var u = Q.Dequeue();
            for (int i = 0; i < move.Length; i++) {
                int x = (int)(u.x + move[i].x);
                int y = (int)(u.y + move[i].y);
                if (IfInMap(x, y) && vis[x, y] == 0) {
                    vis[x, y] = -1;//该像素在外面
                    Q.Enqueue(new Vector2(x,y));
                }
            }
        }
        //更新gtArea
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                if (vis[i, j] != -1) {
                    nowArea[i, j] = 1;
                    t2d.SetPixel(i, j, colorSet[areaId]);//更新texture2d            
                }        
        t2d.Apply();
    }
    private bool IfInMap(int x, int y) {//在map的范围里
        return 0 <= x && x < width && 0 <= y && y < height;
    }

    public void OnDrag(PointerEventData data)
    {
        if (!ifActive)
            return;
        var rt = GetComponent<RectTransform>();
        Vector2 p;
        //Debug.Log("rt: "+rt);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, data.position, data.pressEventCamera, out p);
        UpdateDrawMap(UIPosToPixel(p));
        //Debug.Log("p:" + p);
    }
    private Vector2 UIPosToPixel(Vector2 p) {
        var rt = GetComponent<RectTransform>().rect;
        p += new Vector2(rt.width, rt.height) / 2;
        //Debug.Log("rect.size: "+new Vector2(rt.width,rt.height));
        var re = new Vector2((int)(p.x/rt.width*width),(int)(p.y/rt.height*height));        
        return re;
    }
    private void UpdateDrawMap(Vector2 p) {//带入像素坐标更新drawMap,x右,y上,起点(0,0)
        //Debug.Log("p: "+p);
        for (int i = 0; i < move.Count; i++) {
            int x = (int)(p.x +move[i].x);
            int y = (int)(p.y + move[i].y);
            if (IfInMap(x, y)) {
                t2d.SetPixel(x, y, colorSet[areaId]);
                area[areaId][x,y]=1;
            }
        }
        t2d.Apply();
    }
    public void ChangeChosenId() {
        areaId = 1 - areaId;
    }
    public float GetIoU() {//计算IoU并显示
        float intersection = 0;
        float union = 0;
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++) {
                t2d.SetPixel(i,j,Color.clear);
                if (area[0][i, j] + area[1][i, j] >= 1) {
                    union++;
                    t2d.SetPixel(i, j, Color.red);
                }
                if (area[0][i, j] + area[1][i, j] == 2) {
                    intersection++;
                    t2d.SetPixel(i, j, Color.blue);
                }
            }
        t2d.Apply();
        if (union == 0)
            return 0;
        return intersection / union;
    }
    public void ProjectSelect() {//投影选中的模型
        var o = objManager.panelStatusController.selectedObj;
        if (o == null)
            return;
        foreach (var item in o.GetComponentsInChildren<SkinnedMeshRenderer>()) {
            var vertices = ObjManager.GetRealVertices(item);
            var triangles = item.sharedMesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3) {
                var tmp = new Vector2[] {
                    WorldPointToPixel(vertices[triangles[i]]),
                    WorldPointToPixel(vertices[triangles[i+1]]),
                    WorldPointToPixel(vertices[triangles[i+2]])
                };
                var pList = new List<Vector2>();
                for (int j = 0; j < tmp.Length; j++) {
                    var u = tmp[j];
                    var v = tmp[(j + 1)%tmp.Length];
                    float dx = 0;
                    float dy = 0;
                    int totalPixel = 0;
                    if (Mathf.Abs(v.x - u.x) > Mathf.Abs(v.y - u.y))
                    {
                        dx = u.x > v.x ? -1 : 1;
                        dy = (v.y - u.y) / (v.x - u.x) * dx;
                        totalPixel = (int)Mathf.Abs(v.x-u.x)+1;
                    }
                    else if (v.y == u.y)
                    {
                        dx = dy = 1;
                        totalPixel = 1;
                    }
                    else {
                        dy = u.y > v.y ? -1 : 1;
                        dx = (v.x - u.x) / (v.y - u.y)*dy;
                        totalPixel = (int)Mathf.Abs(v.y - u.y) + 1;
                    }
                    var d = new Vector2(dx,dy);
                    int cnt = 0;
                    while (cnt < totalPixel) {
                        cnt++;
                        pList.Add(new Vector2(u.x,u.y));
                        u += d;
                    }
                }
                foreach (var v in pList) {
                    int x = (int)v.x;
                    int y = (int)v.y;
                    if (IfInMap(x,y)) {
                        area[areaId][x, y] = 1;
                        t2d.SetPixel(x, y, colorSet[areaId]);
                    }
                }
            }            
        }
        FillDrawMap();
    }
    public Vector2 WorldPointToPixel(Vector3 p) {//将世界坐标转为pixel        
        Vector2 lp;
        p=objManager.mainCamera.WorldToScreenPoint(p);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), p, objManager.cameraBackground, out lp);
        return UIPosToPixel(lp);
    }
}
