using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewItemController : MonoBehaviour {
    public GameObject model;//scrollViewItem对应的model
    public Button buttonShowSons;//显示儿子scrollViewItem的按钮
    public GameObject panelTab;//显示缩进的panel
    GameObject scrollViewContent;//scrollViewItem所放置到的content
    public GameObject par;//该scrollViewItem的父scrollViewItem

    private List<GameObject> sons;//该scrollViewItem的儿子scrollViewItem们

    private ObjManager objManager;
    private bool ifShowSons;//是否显示儿子
    
    void Awake () {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        ifShowSons = false;
        sons = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Init(GameObject item,GameObject scrollViewContent)//初始化scrollViewItem
    {
        this.scrollViewContent = scrollViewContent;
        var buttonModel = transform.Find("ButtonModel");        
        buttonModel.GetComponentInChildren<Text>().text = item.name;
        model = item;//将模型赋值给item的脚本        
        scrollViewContent.GetComponent<ContentController>().Add(gameObject);//将scrollViewItem添加到scrollView里 
        NoSons();        
    }
    public void Init(GameObject item, GameObject scrollViewContent,Color unselectedColor)//初始化scrollViewItem，未选中时模型的颜色
    {
        Init(item, scrollViewContent);
        var buttonModel = transform.Find("ButtonModel");
        buttonModel.GetComponent<ButtonModelOnClick>().unselectedColor = unselectedColor;        
    }
    public void ShowSons()//显示儿子
    {
        ifShowSons = true;
        buttonShowSons.GetComponent<Image>().sprite = objManager.spriteTriangleDown;//向下三角形
        foreach(var item in sons)
        {
            item.SetActive(true);
        }
    }
    public void HideSons()//隐藏儿子
    {
        ifShowSons = false;
        buttonShowSons.GetComponent<Image>().sprite= objManager.spriteTriangleRight;//向右三角形
        foreach (var item in sons)
        {
            item.GetComponent<ScrollViewItemController>().HideSons();
            item.SetActive(false);
        }
    }
    public void ButtonShowSonsOnClick()//按钮被点击
    {
        ifShowSons = !ifShowSons;
        if (ifShowSons)
            ShowSons();
        else
            HideSons();
    }
    public void NoSons()//没儿子
    {
        buttonShowSons.interactable = false;//设置其为不可交互状态（有儿子了才能交互）
        var colorTmp = buttonShowSons.GetComponent<Image>().color;
        colorTmp.a = 0;
        buttonShowSons.GetComponent<Image>().color = colorTmp;//三角按钮变成透明
    }
    public void AddSon(GameObject item)//增加儿子scrollViewItem
    {        
        if (sons.Contains(item))//已包含该儿子
            return;
        sons.Add(item);
        item.GetComponent<ScrollViewItemController>().SetPar(gameObject);

        buttonShowSons.interactable = true;//有儿子，可交互
        var colorTmp = buttonShowSons.GetComponent<Image>().color;//显示三角形
        colorTmp.a = 1;
        buttonShowSons.GetComponent<Image>().color = colorTmp;//三角按钮不透明
        ShowSons();//添加一个儿子show一下儿子
    }
    public void RemoveSon(GameObject item)//删除儿子scrollViewItem
    {
        if (!sons.Contains(item))//不包含该儿子
            return;
        sons.Remove(item);
        if (sons.Count == 0)
            NoSons();
    }
    public void SetPar(GameObject item)//设置父亲
    {
        par = item;
        //修改缩进
        var prts = item.GetComponent<ScrollViewItemController>().panelTab.GetComponent<RectTransform>().sizeDelta;
        var rt = panelTab.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(prts.x+ rt.sizeDelta.y,rt.sizeDelta.y);
        var rtSvi = GetComponentInParent<RectTransform>();
        rtSvi.sizeDelta =new Vector2(item.GetComponent<RectTransform>().sizeDelta.x+ rt.sizeDelta.y, rt.sizeDelta.y);
        var rtContent = scrollViewContent.GetComponent<RectTransform>();
        rtContent.sizeDelta = new Vector2(Mathf.Max(rtContent.sizeDelta.x, rtSvi.sizeDelta.x), rtContent.sizeDelta.y);
    }
    public void Delete()//删除该scrollView及其子孙scrollView和对应model
    {
        var sonsCopy = new List<GameObject>(sons);
        foreach(var item in sonsCopy)
        {
            item.GetComponent<ScrollViewItemController>().Delete();
        }
        if (model.transform.parent==objManager.model3d)
            objManager.model3d.GetComponent<Model3dController>().RemoveSon();//删除一个大模型
        if (par != null)//有父亲
            par.GetComponent<ScrollViewItemController>().RemoveSon(gameObject);
        Destroy(model);//删除item对应模型
        Destroy(gameObject);//删除ScrollViewItem
    }
}
