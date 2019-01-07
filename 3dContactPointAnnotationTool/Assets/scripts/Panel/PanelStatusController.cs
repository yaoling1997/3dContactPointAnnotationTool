using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PanelStatusController : MonoBehaviour {
    public GameObject selectedObj;//当前选中的对象
    public Slider sliderTriangles;//控制三角形数量的slider
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
        Model3dItemController model3dItemController = null;
        if (obj.layer.Equals(Macro.MODEL3D_ITEM))//该obj是model
        {
            model3dItemController = obj.GetComponent<Model3dItemController>();
            sliderTriangles.interactable = model3dItemController.trianglesEditable;

            sliderTriangles.value = sliderTriangles.GetComponent<SliderTrianglesController>().TriangleNumToValue(obj.GetComponent<Model3dItemController>().GetTriangleMultiNum());//三角面片倍数to slider的value
        }
        else//该obj是接触点
        {
            sliderTriangles.value = 0;
            sliderTriangles.interactable = false;
        }        
        foreach(var item in gameObject.GetComponentsInChildren<InputField>())
        {            
            if (item.transform.parent.name.Equals("PanelStatusItemPosX"))
            {
                item.interactable = model3dItemController==null?true: model3dItemController.positionEditable;//null说明选中的是接触点
                item.text = obj.transform.position.x.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemPosY"))
            {
                item.interactable = model3dItemController == null ? true : model3dItemController.positionEditable;//null说明选中的是接触点
                item.text = obj.transform.position.y.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemPosZ"))
            {
                item.interactable = model3dItemController == null ? true : model3dItemController.positionEditable;//null说明选中的是接触点
                item.text = obj.transform.position.z.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemRotX"))
            {
                item.interactable = model3dItemController == null ? true : model3dItemController.rotationEditable;//null说明选中的是接触点
                item.text = obj.transform.eulerAngles.x.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemRotY"))
            {
                item.interactable = model3dItemController == null ? true : model3dItemController.rotationEditable;//null说明选中的是接触点
                item.text = obj.transform.eulerAngles.y.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemRotZ"))
            {
                item.interactable = model3dItemController == null ? true : model3dItemController.rotationEditable;//null说明选中的是接触点
                item.text = obj.transform.eulerAngles.z.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemScaleX"))
            {
                item.interactable = model3dItemController == null ? true : model3dItemController.scaleEditable;//null说明选中的是接触点
                item.text = obj.transform.localScale.x.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemScaleY"))
            {
                item.interactable = model3dItemController == null ? true : model3dItemController.scaleEditable;//null说明选中的是接触点
                item.text = obj.transform.localScale.y.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemScaleZ"))
            {
                item.interactable = model3dItemController == null ? true : model3dItemController.scaleEditable;//null说明选中的是接触点
                item.text = obj.transform.localScale.z.ToString();
            }else
            {
                Debug.Log("statusPanelController:can not find correct parent!");
            }            
        }
    }
}
