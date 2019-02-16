using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
public class PanelItemWarehouseController : MonoBehaviour {
    private ObjManager objManager;
    private GameObject prefabScrollViewTabItem;
    private GameObject prefabScrollViewItemsItem;
    public GameObject scrollViewTabContent;
    public GameObject scrollViewItemsContent;
    public ScrollRect scrollRect;//ScrollView的scrollRect组件

    private Dictionary<GameObject, GameObject> tabToContent;//通过tab找到对应的Content
    private GameObject selectedTab;//选中的tab
    private GameObject selectedItem;//选中的selectedScrollViewItemsItem
    // Use this for initialization
    void Start () {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        prefabScrollViewTabItem = objManager.prefabScrollViewTabItem;
        prefabScrollViewItemsItem = objManager.prefabScrollViewItemsItem;
        tabToContent = new Dictionary<GameObject, GameObject>();
        selectedTab = null;
        selectedItem = null;
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
        if (fb.ShowDialog() == DialogResult.OK)//接受路径
        {     
            CompentPath = fb.SelectedPath;
        }
        else//用户取消
        {
            Debug.Log("cancel!");
            return;
        }
        //将路径中的 \ 替换成 /            由于unity路径的规范必须转
        UnityPath = CompentPath.Replace(@"\", "/");
        var subDirectories=GetAllLocalSubDirs(UnityPath);
        if (subDirectories == null)//路径不正确或没有文件
            return;
        foreach(var dir in subDirectories)
        {
            var dirName = AbsolutePathToName(dir);
            var tab = Instantiate(prefabScrollViewTabItem);//利用模板生成一个tab
            tab.name = "Tab " + dirName;
            tab.GetComponentInChildren<Text>().text=dirName;//将名字赋值为子目录的名字
            tab.transform.SetParent(scrollViewTabContent.transform);//成为scrollViewTab的儿子
            var content = Instantiate(scrollViewItemsContent);//克隆空的scrollViewItemsContent
            content.transform.SetParent(scrollViewItemsContent.transform.parent);//设置父亲
            ApplyRT(scrollViewItemsContent.GetComponent<RectTransform>(), content.GetComponent<RectTransform>());
            tabToContent[tab] = content;//将tab和该content关联
            var objFiles=GetAllLocalSubFiles(dir);
            if (objFiles != null)//路径不正确或没有文件
            { 
                foreach (var file in objFiles)
                {
                    var fileName = NameOffSuffix(AbsolutePathToName(file));                
                    var scrollViewItemsItem = Instantiate(prefabScrollViewItemsItem);
                    scrollViewItemsItem.GetComponent<ScrollViewItemsItemController>().objAbsolutePath = file;
                    scrollViewItemsItem.name = "scrollViewItemsItem " + fileName;
                    scrollViewItemsItem.GetComponentInChildren<Text>().text =fileName;
                    scrollViewItemsItem.transform.SetParent(content.transform);                
                }
            }
            content.SetActive(false);//先不显示
        }
    }
    private void ApplyRT(RectTransform a,RectTransform b)//把a给b
    {
        b.localPosition = a.localPosition;
        b.sizeDelta = a.sizeDelta;
    }
    public string AbsolutePathToName(string s)//将绝对路径转为文件名,绝对路径末尾不能带'\'
    {
        var tmp = s.Split(new char[]{ '\\' ,'/'});
        return tmp[tmp.Length - 1];
    }
    public string NameOffSuffix(string s)//去掉后缀
    {
        var tmp = s.Split('.');
        var re = tmp.Length > 1 ? tmp[tmp.Length - 2] : tmp[tmp.Length - 1];
        return re;
    }
    public List<string> GetAllLocalSubDirs(string rootPath)//获得当前文件夹下所有文件夹的绝对路径
    {
        if (string.IsNullOrEmpty(rootPath))
            return null;
        string fullRootPath = Path.GetFullPath(rootPath);
        if (string.IsNullOrEmpty(fullRootPath))
            return null;

        string[] dirs = Directory.GetDirectories(fullRootPath);
        if ((dirs == null) || (dirs.Length <= 0))
            return null;
        var ret = new List<string>();
        foreach (var item in dirs) {
            ret.Add(item);
        }
        return ret;
    }
    public List<string> GetAllLocalSubFiles(string rootPath)//获得当前文件夹下所有文件的绝对路径
    {
        if (string.IsNullOrEmpty(rootPath))
            return null;
        string fullRootPath = Path.GetFullPath(rootPath);
        if (string.IsNullOrEmpty(fullRootPath))
            return null;

        string[] files = Directory.GetFiles(fullRootPath,"*.obj");//获得所有obj文件的路径
        if ((files == null) || (files.Length <= 0))
            return null;
        var ret = new List<string>();
        foreach (var item in files)
        {
            ret.Add(item);
        }
        return ret;
    }
    public void SwitchTab(GameObject tab)
    {
        if (scrollRect.content != null)
            scrollRect.content.gameObject.SetActive(false);//隐藏当前
        scrollRect.content = tabToContent[tab].GetComponent<RectTransform>();
        scrollRect.content.gameObject.SetActive(true);//显示当前
        if (selectedTab != null)
        {
            selectedTab.GetComponent<Image>().color = Color.white;
        }
        selectedTab = tab;
        selectedTab.GetComponent<Image>().color = Color.cyan;
    }
    public void SwitchItemsItem(GameObject item)
    {
        if (selectedItem != null)
            selectedItem.GetComponent<Image>().color = Color.white;
        selectedItem = item;
        selectedItem.GetComponent<Image>().color = Color.cyan;
    }
}
