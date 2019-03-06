using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelSettingController : MonoBehaviour {
    private ObjManager objManager;
    public Dropdown dropdownExportPrecision;//导出的数据精确到小数点后几位
    public InputField inputFieldActionLimit;//最多保存多少条撤销    
    public InputField inputFieldFieldOfView;//相机视角宽度

    public int exportPrecision;//导出的数据精确到小数点后几位
    void Awake() {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        exportPrecision = 3;
        dropdownExportPrecision.value=exportPrecision;
        inputFieldActionLimit.text = objManager.editorUndoRedoSystem.ActionLimit.ToString();
        inputFieldFieldOfView.text = objManager.mainCamera.fieldOfView.ToString();
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void DropdownExportPrecisionOnValueChanged()
    {
        exportPrecision = dropdownExportPrecision.value;//第几项就是精确到第几位
    }
    public void InputFieldActionLimitOnValueChanged() {
        objManager.editorUndoRedoSystem.ActionLimit = StringToInt(inputFieldActionLimit.text);
    }
    public void InputFieldFieldOfViewOnValueChanged()//相机视角大小
    {
        var v = Mathf.Clamp(StringToFloat(inputFieldFieldOfView.text),1,179);
        objManager.mainCamera.fieldOfView = v;
    }
    public void InputFieldFieldOfViewOnEndEdit()//相机视角大小
    {
        inputFieldFieldOfView.text = objManager.mainCamera.fieldOfView.ToString();
    }
    public int StringToInt(string s) {
        int re = 0;
        try
        {
            re = int.Parse(s);
        }
        catch {
            return 0;
        }
        return re;
    }
    public float StringToFloat(string s) {
        float re = 0;
        try
        {
            re = float.Parse(s);
        }
        catch
        {
            return 0;
        }
        return re;
    }
}
