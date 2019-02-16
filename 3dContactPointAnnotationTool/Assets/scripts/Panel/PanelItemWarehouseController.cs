using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Windows.Forms;
public class PanelItemWarehouseController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void ButtonAddItemWarehouseOnClick()//添加ItemWarehouse按钮被点击
    {
        FolderBrowserDialog fb = new FolderBrowserDialog();   //创建控件并实例化
        fb.Description = "选择文件夹";
        //fb.RootFolder = Environment.SpecialFolder.MyComputer;  //设置默认路径
        fb.ShowNewFolderButton = false;   //创建文件夹按钮关闭
                                          //如果按下弹窗的OK按钮
        string CompentPath="";
        string UnityPath;
        if (fb.ShowDialog() == DialogResult.OK)
        { 
    //接受路径
            CompentPath = fb.SelectedPath;
        }
        //将路径中的 \ 替换成 /            由于unity路径的规范必须转
        UnityPath = CompentPath.Replace(@"\", "/");
        print(UnityPath);
    }
}
