using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelSettingController : MonoBehaviour {
    private ObjManager objManager;
    public Dropdown dropdownExportPrecision;//导出的数据精确到小数点后几位
    public InputField inputFieldActionLimit;//最多保存多少条撤销

    public int exportPrecision;//导出的数据精确到小数点后几位
    void Awake() {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        exportPrecision = 3;
        dropdownExportPrecision.value=exportPrecision;
        inputFieldActionLimit.text = objManager.editorUndoRedoSystem.ActionLimit.ToString();
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
}
