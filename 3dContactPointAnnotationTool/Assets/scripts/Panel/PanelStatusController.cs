using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PanelStatusController : MonoBehaviour {
    public GameObject selectedObj;//当前选中的对象
    public Slider sliderTriangles;//控制三角形数量的slider
    public GameObject panelEditStatus;//另一个编辑status的面板

    private EventSystem eventSystem;
    private Dictionary<int, GameObject> dicInputFields;//面板上所有inputFields
    private int dicId;//选中字典中第几个对象

    // Use this for initialization
    void Awake () {
        selectedObj = null;
    }
    private void Start()
    {
        eventSystem = EventSystem.current;//获取当前EventSystem
        dicInputFields = new Dictionary<int, GameObject>();
        dicId = 0;
        foreach(var item in GetComponentsInChildren<InputField>())
        {
            dicInputFields.Add(dicId,item.gameObject);//把组件的对象加入字典
            dicId++;
        }
        dicId = 0;
        GameObject obj;
        dicInputFields.TryGetValue(dicId, out obj);
        eventSystem.SetSelectedGameObject(obj, new BaseEventData(eventSystem));//设置第一个可交互的UI为高亮状态
    }
    // Update is called once per frame
    void Update () {
        //选中对象不为空且按下tab
        if (eventSystem.currentSelectedGameObject != null && Input.GetKeyDown(KeyCode.Tab))
        {
            var selectedObj = eventSystem.currentSelectedGameObject;
            dicId = 0;
            foreach(var item in dicInputFields)
            {
                if (item.Value == selectedObj)
                {
                    dicId = item.Key + 1;
                    if (dicId == dicInputFields.Count)
                        dicId = 0;
                    break;
                }
            }
            GameObject obj;
            dicInputFields.TryGetValue(dicId, out obj);
            eventSystem.SetSelectedGameObject(obj, new BaseEventData(eventSystem));
        }
        if (selectedObj == null)//未选中物体
        {
            sliderTriangles.interactable = false;
            foreach (var item in GetComponentsInChildren<InputField>())
            {
                item.interactable = false;                
            }
        }
        UpdateInputFieldValues();
    }
    public void UpdateInputFieldValues()//更新所有inputFiled显示的值
    {
        var obj = selectedObj;
        if (obj == null)
            return;
        foreach (var item in gameObject.GetComponentsInChildren<InputField>())
        {
            var iovc = item.GetComponent<InputFieldOnValueChanged>();
            iovc.disableOnValueChanged = true;
            if (item.transform.parent.name.Equals("PanelStatusItemPosX"))
            {
                if (!item.isFocused)
                    item.text = obj.transform.position.x.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemPosY"))
            {
                if (!item.isFocused)
                    item.text = obj.transform.position.y.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemPosZ"))
            {
                if (!item.isFocused)
                    item.text = obj.transform.position.z.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemRotX"))
            {
                if (!item.isFocused)
                    item.text = obj.transform.eulerAngles.x.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemRotY"))
            {
                if (!item.isFocused)
                    item.text = obj.transform.eulerAngles.y.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemRotZ"))
            {
                if (!item.isFocused)
                    item.text = obj.transform.eulerAngles.z.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemScaleX"))
            {
                if (!item.isFocused)
                    item.text = obj.transform.localScale.x.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemScaleY"))
            {
                if (!item.isFocused)
                    item.text = obj.transform.localScale.y.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemScaleZ"))
            {
                if (!item.isFocused)
                    item.text = obj.transform.localScale.z.ToString();
            }
            else
            {
                Debug.Log("statusPanelController:can not find correct parent!");
            }
            iovc.disableOnValueChanged = false;
        }
    }
    public void SetSelectedObj(GameObject obj)
    {
        selectedObj = obj;
        if (obj == null)
        {
            foreach(var item in GetComponentsInChildren<InputField>())
            {
                item.interactable = false;
                item.text = 0.ToString();
            }
            return;
        }
        var itemController = obj.GetComponent<ItemController>();
        if (itemController.trianglesEditable)//修改三角形slider
        {
            sliderTriangles.interactable = true;
            sliderTriangles.value = sliderTriangles.GetComponent<SliderTrianglesController>().TriangleNumToValue(obj.GetComponent<Model3dItemController>().GetTriangleMultiNum());//三角面片倍数to slider的value
        }
        else
        {
            sliderTriangles.value = 0;
            sliderTriangles.interactable = false;
        }
        foreach(var item in gameObject.GetComponentsInChildren<InputField>())//更新interactable
        {            
            if (item.transform.parent.name.Equals("PanelStatusItemPosX"))
            {
                item.interactable = itemController.positionEditable;
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemPosY"))
            {
                item.interactable = itemController.positionEditable;
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemPosZ"))
            {
                item.interactable = itemController.positionEditable;
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemRotX"))
            {
                item.interactable = itemController.rotationEditable;
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemRotY"))
            {
                item.interactable = itemController.rotationEditable;
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemRotZ"))
            {
                item.interactable = itemController.rotationEditable;
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemScaleX"))
            {
                item.interactable = itemController.scaleEditable;
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemScaleY"))
            {
                item.interactable = itemController.scaleEditable;
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemScaleZ"))
            {
                item.interactable = itemController.scaleEditable;
            }else
            {
                Debug.Log("statusPanelController:can not find correct parent!");
            }            
        }
        UpdateInputFieldValues();
    }
    public void ButtonResetOnClick()//Reset按钮被点击
    {
        panelEditStatus.SetActive(false);
        if (selectedObj == null)
            return;
        ButtonResetPositionOnClick();
        ButtonResetRotationOnClick();
        ButtonResetScaleOnClick();
    }
    public void ButtonResetPositionOnClick()//ResetPosition按钮被点击
    {
        panelEditStatus.SetActive(false);
        if (selectedObj == null)
            return;
        //foreach (var i in new string[] {"X","Y","Z" }) {
        //    transform.Find("PanelStatusItemPos"+i).GetComponentInChildren<InputField>().text="0";
        //}
        selectedObj.transform.position = Vector3.zero;
    }
    public void ButtonResetRotationOnClick()//ResetRotation按钮被点击
    {
        panelEditStatus.SetActive(false);
        if (selectedObj == null)
            return;
        //foreach (var i in new string[] { "X", "Y", "Z" })
        //{
        //    transform.Find("PanelStatusItemRot" + i).GetComponentInChildren<InputField>().text = "0";
        //}
        selectedObj.transform.eulerAngles = Vector3.zero;
    }
    public void ButtonResetScaleOnClick()//ResetScale按钮被点击
    {
        panelEditStatus.SetActive(false);
        if (selectedObj == null)
            return;
        //foreach (var i in new string[] { "X", "Y", "Z" })
        //{
        //    transform.Find("PanelStatusItemScale" + i).GetComponentInChildren<InputField>().text = "1";
        //}
        selectedObj.transform.localScale = new Vector3(1,1,1);
    }

}
