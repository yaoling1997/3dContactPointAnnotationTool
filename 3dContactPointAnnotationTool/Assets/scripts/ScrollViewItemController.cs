using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTEditor;

public class ScrollViewItemController : MonoBehaviour {
    public GameObject model;//scrollViewItem对应的model
    public Button buttonShowSons;//显示儿子scrollViewItem的按钮
    public GameObject panelTab;//显示缩进的panel
    public GameObject scrollViewContent;//scrollViewItem所放置到的content
    public GameObject par;//该scrollViewItem的父scrollViewItem

    private List<GameObject> sons;//该scrollViewItem的儿子scrollViewItem们

    private ObjManager objManager;
    private EditorObjectSelection editorObjectSelection;
    private int status;//0表示未选中，1表示选中
    private Button buttonModel;//选中模型的按钮
    public Color selectedColor;//选中时模型的颜色
    public Color unselectedColor;//未选中时模型的颜色
    private GameObject panelStatus;
    private PanelStatusController panelStatusController;
    private bool ifShowSons;//是否显示儿子

    
    void Awake () {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        ifShowSons = false;
        sons = new List<GameObject>();
        status = 0;
        selectedColor = Color.cyan;//默认选中颜色
        unselectedColor = Color.white;//默认未选中颜色
        buttonModel = transform.Find("ButtonModel").GetComponent<Button>();        
    }
    void Start()
    {
        panelStatus = objManager.panelStatus;
        panelStatusController = panelStatus.GetComponentInChildren<PanelStatusController>();
        editorObjectSelection = objManager.editorObjectSelection;
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
        if (model.GetComponent<CorrespondingScrollViewItem>()==null)
            model.AddComponent<CorrespondingScrollViewItem>();
        model.GetComponent<CorrespondingScrollViewItem>().sviController = this;//将scrollViewItem赋值给model
        scrollViewContent.GetComponent<ContentController>().Add(gameObject);//将scrollViewItem添加到scrollView里 
        NoSons();
        SetModelColor(unselectedColor);
    }
    public void Init(GameObject item, GameObject scrollViewContent,Color unselectedColor)//初始化scrollViewItem，未选中时模型的颜色
    {
        this.unselectedColor = unselectedColor;
        Init(item, scrollViewContent);                
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
    public void Delete()//删除该scrollView及其子孙scrollView
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
        //Destroy(model);//删除item对应模型
        Destroy(gameObject);//删除ScrollViewItem
    }
    public void SetModelColor(Color color)//设置模型颜色
    {
        if (model.GetComponent<SkinnedMeshRenderer>() != null)
            model.GetComponent<SkinnedMeshRenderer>().material.color = color;
        else if (model.GetComponent<MeshRenderer>() != null)
            model.GetComponent<MeshRenderer>().material.color = color;

    }
    public void SetSelected()
    {
        panelStatusController.SetSelectedObj(model);//将该scrollVIew项的model赋值给panelStatus，显示该model信息        
        status = 1;
        buttonModel.GetComponent<Image>().color = Color.cyan;
        SetModelColor(selectedColor);
        model.tag = Macro.SELECTED;//设置为已选中
    }
    public void SetUnselected()
    {
        panelStatusController.SetSelectedObj(null);//将该scrollVIew项的model赋值给panelStatus，显示该model信息        
        status = 0;
        buttonModel.GetComponent<Image>().color = Color.white;
        SetModelColor(unselectedColor);
        model.tag = Macro.UNSELECTED;//设置为未选中
    }
    public void ChangeStatus()//反转状态
    {
        status ^= 1;
        if (status == 1)
        {
            editorObjectSelection.AddObjectToSelection(model, true);
            //SetSelected();
        }
        else
        {
            editorObjectSelection.RemoveObjectFromSelection(model, true);
            //SetUnselected();
        }
    }
    public void ButtonModelOnClick()
    {        
        ChangeStatus();
    }
    public void ButtonDeleteOnClick()//删除一系列的scrollView及model
    {
        List<GameObject> selectedObjects = new List<GameObject>();
        selectedObjects.Add(model);
        editorObjectSelection.SetSelectedObjects(selectedObjects, true);
        var deleteAction = new DeleteSelectedObjectsAction();
        deleteAction.Execute();
        //Delete();

    }

}
