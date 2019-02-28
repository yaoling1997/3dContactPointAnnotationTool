using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PanelCameraControllerDragChangeValue : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public float speed = 0.1f;//改变值的速率

    private ObjManager objManager;
    bool isPrimary = true;//是否是主屏
    int primaryScreenWidth = 0;//主屏宽
    int primaryScreenHeight = 0;//主屏高
    int secondScreenWidth = 0;//副屏宽
    int secondScreenHeight = 0;//副屏高

    void Awake()
    {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        SetScreenSize();
    }
    public void OnPointerDown(PointerEventData data)
    {
        //rectTransformPanel.SetAsLastSibling();//把该组件放到UI最前面
    }

    public void OnDrag(PointerEventData data)
    {
        //Debug.Log("on drag");
        var axisX = Input.GetAxis("Mouse X");
        var axisY = Input.GetAxis("Mouse Y");
        //Debug.Log("axisX: "+axisX);
        //Debug.Log("axisY: " + axisY);
        var t = objManager.mainCamera.transform;
        if (name.Equals("PanelControllForwardBack")) {
            t.position = t.position + t.forward* -axisX * speed;
        }
        else if (name.Equals("PanelControllLeftRight"))
        {
            t.position = t.position + t.right * axisX * speed;
        }
        else if (name.Equals("PanelControllUpDown"))
        {
            t.position = t.position + t.up * -axisX * speed;
        }

        Debug.Log("ScreenSize: " + new Vector2(primaryScreenWidth, primaryScreenHeight));
        Debug.Log("mouse pos: " + Input.mousePosition);
        SetScreenState();//
        UpdateCursorPos();
    }
    [System.Runtime.InteropServices.DllImport("user32.dll")] //引入dll
    public static extern int SetCursorPos(int x, int y);

    private void SetScreenSize()//设置屏幕大小
    {
        primaryScreenWidth = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
        primaryScreenHeight = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
        if (System.Windows.Forms.Screen.AllScreens.Length > 1)//不止一个屏幕
        {
            secondScreenWidth = System.Windows.Forms.Screen.AllScreens[1].WorkingArea.Width;
            secondScreenHeight = System.Windows.Forms.Screen.AllScreens[1].WorkingArea.Height;
        }
    }
    private void SetScreenState()//判断光标是否在主屏上
    {
        isPrimary = true;
        //获取当前光标位置
        if (System.Windows.Forms.Cursor.Position.X > primaryScreenWidth)
        {
            isPrimary = false;
        }
    }
    private void UpdateCursorPos()//更新光标位置,x→,y↓
    {
        int posX = System.Windows.Forms.Cursor.Position.X;
        int posY = System.Windows.Forms.Cursor.Position.Y;
        if (isPrimary)
        {//光标在主屏上
            if (posY <= 10) //top
                ResetCursorPos(true);
            if (posY >= primaryScreenHeight - 10)//bottom
                ResetCursorPos(false, true);
            if (posX <= 10)//left
                ResetCursorPos(false, false, true);//right
            if (posX >= primaryScreenWidth - 10)
                ResetCursorPos(false, false, false, true);
        }
        else//副屛上
        {
            if (posY <= 10) //top
                ResetCursorPos(true);
            if (posY >= secondScreenWidth - 10)//bottom
                ResetCursorPos(false, true);
            if (posX <= primaryScreenWidth + 10)//left
                ResetCursorPos(false, false, true);//right
            if (posX >= primaryScreenWidth + secondScreenWidth - 10)
                ResetCursorPos(false, false, false, true);

        }
    }
    private void ResetCursorPos(bool isTop = false, bool isBottom = false, bool isLeft = false, bool isRight = false)//光标移动到边界，将光标放到对应的另一边
    {
        int posX = System.Windows.Forms.Cursor.Position.X;
        int posY = System.Windows.Forms.Cursor.Position.Y;//获取当前光标位置

        if (isPrimary)
        {
            if (isTop)
                SetCursorPos(posX, primaryScreenHeight - 11);
            if (isBottom)
                SetCursorPos(posX, 11);
            if (isLeft)
                SetCursorPos(primaryScreenWidth - 11, posY);
            if (isRight)
                SetCursorPos(11, posY);
        }
        else
        {
            if (isTop)
                SetCursorPos(posX, secondScreenHeight - 11);
            if (isBottom)
                SetCursorPos(posX, 11);
            if (isLeft)
                SetCursorPos(primaryScreenWidth + secondScreenWidth - 11, posY);
            if (isRight)
                SetCursorPos(primaryScreenWidth + 11, posY);
        }
    }
}
