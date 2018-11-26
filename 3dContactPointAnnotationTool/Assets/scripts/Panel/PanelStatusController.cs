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
    }
    public void SetSelectedObj(GameObject obj)
    {
        selectedObj = obj;
        if (obj.transform.parent.name.Equals("Model3d"))//该obj是model
        {
            sliderTriangles.interactable = true;
            sliderTriangles.value = sliderTriangles.GetComponent<SliderTrianglesController>().TriangleNumToValue(obj.GetComponent<Model3dItemController>().GetTriangleMultiNum());//三角面片倍数to slider的value
        }
        else//该obj是接触点
        {
            sliderTriangles.value = 0;
            sliderTriangles.interactable = false;
        }        
        foreach(var item in gameObject.GetComponentsInChildren<InputField>())
        {
            item.interactable = true;
            if (item.transform.parent.name.Equals("PanelStatusItemPosX"))
            {                
                item.text = obj.transform.position.x.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemPosY"))
            {
                item.text = obj.transform.position.y.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemPosZ"))
            {
                item.text = obj.transform.position.z.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemRotX"))
            {
                item.text = obj.transform.eulerAngles.x.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemRotY"))
            {
                item.text = obj.transform.eulerAngles.y.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemRotZ"))
            {
                item.text = obj.transform.eulerAngles.z.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemScaleX"))
            {
                item.text = obj.transform.localScale.x.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemScaleY"))
            {
                item.text = obj.transform.localScale.y.ToString();
            }
            else if (item.transform.parent.name.Equals("PanelStatusItemScaleZ"))
            {
                item.text = obj.transform.localScale.z.ToString();
            }else
            {
                Debug.Log("statusPanelController:can not find correct parent!");
            }            
        }
    }
}
