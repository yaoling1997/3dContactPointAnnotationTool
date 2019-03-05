using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolBarController : MonoBehaviour {
    enum MouseStatus{nothing=-1, zoom, orbit,orbitSelected, panView, move, rotate, scale, volumeScale };
    public Button[] buttons;//工具栏上的修改mouseStatus的button
    public Button buttonTransform;//global or local
    public float zoomSpeed = 1;
    public float orbitSpeed = 1;
    public float panViewSpeed = 1;
    public float minDist = 1;
    public float maxDist = 3000;
    public bool isZooming = false;//是否正在缩放
    public bool isOrbiting = false;//是否正在旋转
    public bool isOrbitingSelected = false;//是否正在围绕选中物体旋转
    public bool isPanning = false;//是否正在平移视图
    public bool isFocusing = false;//是否正在最大化显示选中视图
    public bool isMoving = false;//是否正在移动对象
    public bool isRotating = false;//是否正在旋转对象
    public bool isScaling = false;//是否正在缩放对象
    public bool isVolumeScaling = false;//是否正在长方体缩放对象
    public bool isGlobal = false;//是否是global

    //private ObjManager objManager;
    public Color buttonSelectedColor;
    //用户鼠标的状态
    private MouseStatus mouseStatus;
    private Color initButtonColor;//初始的按钮颜色
    private string globalString = "Global";
    private string localString = "Local";

    void Awake () {
        mouseStatus = MouseStatus.nothing;
        initButtonColor = buttons[0].GetComponent<Image>().color;        
    }

    void Start()
    {
        //objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
    }

    void Update()
    {
        Zoom();
        Orbit();
        OrbitSelected();
        PanView();
        //mainCamera.transform.LookAt(model3d.transform);//使相机对准3dModel
    }

    private void Zoom()
    {//鼠标左键实现放大缩小视角功能
        if (Input.GetMouseButton(0) && mouseStatus == MouseStatus.zoom)//0左键、1右键、2中键
            isZooming = true;
        else 
            isZooming = false;
    }

    private void Orbit()
    {//鼠标左键实现旋转视角功能        
        if (Input.GetMouseButton(0)&&mouseStatus== MouseStatus.orbit)//0左键、1右键、2中键
            isOrbiting = true;
        else 
            isOrbiting = false;
    }
    private void OrbitSelected()
    {//鼠标左键实现旋转视角功能        
        if (Input.GetMouseButton(0) && mouseStatus == MouseStatus.orbitSelected)//0左键、1右键、2中键
            isOrbitingSelected = true;
        else
            isOrbitingSelected = false;
    }

    private void PanView()
    {//鼠标左键实现平移视图功能     
        if (Input.GetMouseButton(0) && mouseStatus == MouseStatus.panView)//0左键、1右键、2中键
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
        if (mouseStatus != MouseStatus.nothing)
            buttons[(int)mouseStatus].GetComponent<Image>().color = buttonSelectedColor;
    }
    public void ButtonZoomOnClick()//Zoom按钮被点击
    {
        mouseStatus = mouseStatus == MouseStatus.zoom ? MouseStatus.nothing : MouseStatus.zoom;
        ChangeStatus();
    }
    public void ButtonOrbitOnClick()//Orbit按钮被点击
    {
        mouseStatus = mouseStatus == MouseStatus.orbit ? MouseStatus.nothing : MouseStatus.orbit;
        ChangeStatus();
    }
    public void ButtonOrbitSelectedOnClick()//OrbitSelected按钮被点击
    {
        mouseStatus = mouseStatus == MouseStatus.orbitSelected ? MouseStatus.nothing : MouseStatus.orbitSelected;
        ChangeStatus();
    }
    public void ButtonPanViewOnClick()//PanView按钮被点击,平移视图
    {
        mouseStatus = mouseStatus == MouseStatus.panView ? MouseStatus.nothing : MouseStatus.panView;
        ChangeStatus();
    }
    public void ButtonFocusOnClick()//Focus按钮被点击,最大化显示选中对象
    {
        isFocusing = true;
    }
    public void ButtonMoveOnClick()//Move按钮被点击,平移物体
    {
        mouseStatus = mouseStatus == MouseStatus.move ? MouseStatus.nothing : MouseStatus.move;
        isMoving = mouseStatus == MouseStatus.move ? true : false;
        ChangeStatus();        
    }
    public void ButtonRotateOnClick()//Rotate按钮被点击,旋转物体
    {
        mouseStatus = mouseStatus == MouseStatus.rotate ? MouseStatus.nothing : MouseStatus.rotate;
        isRotating = mouseStatus == MouseStatus.rotate ? true : false;
        ChangeStatus();
    }
    public void ButtonScaleOnClick()//Scale按钮被点击,缩放物体
    {
        mouseStatus = mouseStatus == MouseStatus.scale ? MouseStatus.nothing : MouseStatus.scale;
        isScaling = mouseStatus == MouseStatus.scale ? true : false;
        ChangeStatus();
    }
    public void ButtonVolumeScaleOnClick()//VolumeScale按钮被点击,以长方体形式缩放物体
    {
        mouseStatus = mouseStatus == MouseStatus.volumeScale ? MouseStatus.nothing : MouseStatus.volumeScale;
        isVolumeScaling = mouseStatus == MouseStatus.volumeScale ? true : false;
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
        return MouseStatus.move <= mouseStatus && mouseStatus <= MouseStatus.volumeScale;
    }
}
