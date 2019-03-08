using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelSMPLController : MonoBehaviour {
    public const float MinValue = -100;
    public const float MaxValue = 100;
    public InputField inputFieldHeight;
    public InputField inputFieldWeight;
    private ObjManager objManager;
    private PanelStatusController panelStatusController;
    void Awake() {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        panelStatusController = objManager.panelStatusController;
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {        
        if (IsSMPL(panelStatusController.selectedObj))//当前选中的是smpl模型
        {
            inputFieldHeight.interactable = true;
            inputFieldWeight.interactable = true;
            UpdateInputFieldValues();
        }
        else {
            inputFieldHeight.interactable = false;
            inputFieldWeight.interactable = false;
        }
    }
    private bool IsSMPL(GameObject model) {
        return model != null && model.GetComponent<ItemController>().modelType.Equals(ItemController.ModelType.SMPL_MODEL);
    }
    public void UpdateInputFieldValues()
    {
        var model = panelStatusController.selectedObj;
        if (!inputFieldHeight.isFocused)
        {
            var smr = model.GetComponent<SkinnedMeshRenderer>();
            var v = smr.GetBlendShapeWeight(0);
            if (v.Equals(0))
                v = -smr.GetBlendShapeWeight(1);
            inputFieldHeight.text = (-v).ToString();//修正inputField的text的值
        }
        if (!inputFieldWeight.isFocused)
        {
            var smr = model.GetComponent<SkinnedMeshRenderer>();
            var v = smr.GetBlendShapeWeight(2);
            if (v.Equals(0))
                v = -smr.GetBlendShapeWeight(3);
            inputFieldWeight.text = (-v).ToString();//修正inputField的text的值            
        }
    }
    public void InputFeildHeightOnValueChanged() {
        var model = panelStatusController.selectedObj;        
        if (IsSMPL(model)) {
            var smr = model.GetComponent<SkinnedMeshRenderer>();
            var v = -ObjManager.StringToFloat(inputFieldHeight.text);
            v = Mathf.Clamp(v, MinValue, MaxValue);
            float pos = 0,neg=0;
            if (v >= 0)
            {
                pos = v;
            }
            else
            {
                neg = -v;
            }
            smr.SetBlendShapeWeight(0,pos);
            smr.SetBlendShapeWeight(1, neg);
        }
    }
    public void InputFeildWeightOnValueChanged()
    {
        var model = panelStatusController.selectedObj;
        if (IsSMPL(model))
        {
            var smr = model.GetComponent<SkinnedMeshRenderer>();
            var v = -ObjManager.StringToFloat(inputFieldWeight.text);
            v = Mathf.Clamp(v, MinValue, MaxValue);
            float pos = 0, neg = 0;
            if (v >= 0)
            {
                pos = v;
            }
            else
            {
                neg = -v;
            }
            smr.SetBlendShapeWeight(2, pos);
            smr.SetBlendShapeWeight(3, neg);
        }
    }
}
