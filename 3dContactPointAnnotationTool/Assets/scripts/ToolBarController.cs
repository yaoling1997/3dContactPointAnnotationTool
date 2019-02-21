using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolBarController : MonoBehaviour {
    public Button[] buttons;//工具栏上的button
    public Button buttonTransform;//global or local
    public float zoomSpeed = 1;
    public float orbitSpeed = 1;
    public float panViewSpeed = 1;
    public float minDist = 1;
    public float maxDist = 3000;
    public bool isZooming = false;//是否正在缩放
    public bool isOrbiting = false;//是否正在旋转
    public bool isPanning = false;//是否正在平移视图
    public bool isFocusing = false;//是否正在最大化显示选中视图
    public bool isMoving = false;//是否正在移动对象
    public bool isRotating = false;//是否正在旋转对象
    public bool isScaling = false;//是否正在缩放对象
    public bool isVolumeScaling = false;//是否正在长方体缩放对象
    public bool isGlobal = false;//是否是global

    //private ObjManager objManager;
    private Color buttonSelectedColor;
    //用户鼠标的状态,-1未选中工具,0 zoom,1 orbit,2 PanView,4 move,5 rotate,6 scale,7 volumeScale
    private int mouseStatus;
    private Color initButtonColor;//初始的按钮颜色
    private string globalString = "Global";
    private string localString = "Local";

    void Awake () {
        mouseStatus = -1;
        initButtonColor = buttons[0].GetComponent<Image>().color;
        buttonSelectedColor = Color.black;
    }

    void Start()
    {
        //objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
    }

    void Update()
    {
        Zoom();
        Orbit();
        PanView();
        //mainCamera.transform.LookAt(model3d.transform);//使相机对准3dModel
    }

    private void Zoom()
    {//鼠标左键实现放大缩小视角功能
        if (Input.GetMouseButton(0) && mouseStatus == 0)//0左键、1右键、2中键
            isZooming = true;
        else 
            isZooming = false;
    }

    private void Orbit()
    {//鼠标左键实现旋转视角功能        
        if (Input.GetMouseButton(0)&&mouseStatus==1)//0左键、1右键、2中键
            isOrbiting = true;
        else 
            isOrbiting = false;
    }

    private void PanView()
    {//鼠标左键实现平移视图功能     
        if (Input.GetMouseButton(0) && mouseStatus == 2)//0左键、1右键、2中键
            isPanning = true;
        else 
            isPanning = false;
    }

    private void ChangeStatus()
    {
        Debug.Log("mouseStatus:"+mouseStatus);
        foreach(var item in buttons)
        {
            item.GetComponent<Image>().color = initButtonColor;
        }
        if (mouseStatus != -1)
            buttons[mouseStatus].GetComponent<Image>().color = buttonSelectedColor;
    }
    public void ButtonZoomOnClick()//Zoom按钮被点击
    {
        mouseStatus = mouseStatus == 0 ? -1 : 0;
        ChangeStatus();
    }
    public void ButtonOrbitOnClick()//Orbit按钮被点击
    {
        mouseStatus = mouseStatus == 1 ? -1 : 1;
        ChangeStatus();
    }
    public void ButtonPanViewOnClick()//PanView按钮被点击,平移视图
    {
        mouseStatus = mouseStatus == 2 ? -1 : 2;
        ChangeStatus();
    }
    public void ButtonFocusOnClick()//Focus按钮被点击,最大化显示选中对象
    {
        isFocusing = true;
    }
    public void ButtonMoveOnClick()//Move按钮被点击,平移物体
    {
        mouseStatus = mouseStatus == 4 ? -1 : 4;
        isMoving = mouseStatus == 4 ? true : false;
        ChangeStatus();        
    }
    public void ButtonRotateOnClick()//Rotate按钮被点击,旋转物体
    {
        mouseStatus = mouseStatus == 5 ? -1 : 5;
        isRotating = mouseStatus == 5 ? true : false;
        ChangeStatus();
    }
    public void ButtonScaleOnClick()//Scale按钮被点击,缩放物体
    {
        mouseStatus = mouseStatus == 6 ? -1 : 6;
        isScaling = mouseStatus == 6 ? true : false;
        ChangeStatus();
    }
    public void ButtonVolumeScaleOnClick()//VolumeScale按钮被点击,以长方体形式缩放物体
    {
        mouseStatus = mouseStatus == 7 ? -1 : 7;
        isVolumeScaling = mouseStatus == 7 ? true : false;
        ChangeStatus();        
    }

    public void ButtonTransformOnClick()//transform按钮被点击,更换transform gizmo的形式,global or local
    {
        SetGlobal(!isGlobal);
    }
    public void SetGlobal(bool bl)
    {
        isGlobal = bl;
        var textCp = buttonTransform.GetComponentInChildren<Text>();
        if (isGlobal)
            textCp.text = globalString;
        else
            textCp.text = localString;
    }
    public bool CanSelectObject()//是否可以选择对象,只有选中移动,旋转,缩放,长方体缩放工具才能选择对象
    {
        return 4 <= mouseStatus && mouseStatus <= 7;
    }
}
