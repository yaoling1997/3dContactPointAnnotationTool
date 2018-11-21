using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices;

public class ButtonOpenImageOnClick : MonoBehaviour

{
    public InputField inputField;
    public void OnClick()

    {
        OpenFileName ofn = new OpenFileName();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = "图片文件(*.jpg*.png)\0*.jpg;*.png";
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
        ofn.title = "窗口标题";
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
        if (LocalDialog.GetOpenFileName(ofn)) {
            Debug.Log("file: " + ofn.file);
            inputField.text = ofn.file;
        }
    }
}