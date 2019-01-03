using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolBarController : MonoBehaviour {
    public Button[] buttons;//工具栏上的button
    public float zoomSpeed = 1;
    public float orbitSpeed = 1;
    public float minDist = 1;
    public float maxDist = 3000;
    private bool isZooming = false;//是否正在旋转
    private bool isOrbiting = false;//是否正在旋转

    private ObjManager objManager;
    private GameObject model3d;
    private GameObject mainCamera;
    private Color buttonSelectedColor;
    private int mouseStatus;//用户鼠标的状态,-1未选中工具,0 zoom,1 orbit
    private Color initButtonColor;//初始的按钮颜色

    void Awake () {
        mouseStatus = -1;
        initButtonColor = buttons[0].GetComponent<Image>().color;
        buttonSelectedColor = Color.black;
    }

    void Start()
    {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        model3d = objManager.model3d;
        mainCamera = objManager.mainCamera;
    }

    void Update()
    {
        Zoom();
        Orbit();
        //mainCamera.transform.LookAt(model3d.transform);//使相机对准3dModel
    }

    private void Zoom()
    {//鼠标滚轮实现放大缩小视角功能
        if (Input.GetMouseButtonDown(0) && mouseStatus == 0)//0左键、1右键、2中键
            isZooming = true;
        else if (Input.GetMouseButtonUp(0))
            isZooming = false;
        if (isZooming) {
            Vector3 offset = mainCamera.transform.position - model3d.transform.position;
            float dist = offset.magnitude;//获取相机与3d模型的距离
            dist = dist - (Input.GetAxis("Mouse X")+ Input.GetAxis("Mouse Y")) * zoomSpeed;
            dist = Mathf.Clamp(dist, minDist, maxDist);
            offset = offset.normalized * dist;
            mainCamera.transform.position = model3d.transform.position + offset;
        }
    }

    private void Orbit()
    {//鼠标右键实现旋转视角功能        
        if (Input.GetMouseButtonDown(0)&&mouseStatus==1)//0左键、1右键、2中键
            isOrbiting = true;
        else if (Input.GetMouseButtonUp(0))
            isOrbiting = false;
        //Debug.Log(isRotating);
        if (isOrbiting)
        {
            mainCamera.transform.RotateAround(model3d.transform.position, mainCamera.transform.up, orbitSpeed * Input.GetAxis("Mouse X"));
            mainCamera.transform.RotateAround(model3d.transform.position, mainCamera.transform.right, -orbitSpeed * Input.GetAxis("Mouse Y"));
        }
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
}
