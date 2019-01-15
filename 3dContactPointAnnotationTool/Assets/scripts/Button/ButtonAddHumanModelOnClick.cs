using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine.UI;

public class ButtonAddHumanModelOnClick : MonoBehaviour {
    public GameObject scrollViewModelsContent;//scrollViewModels的content
    private ObjManager objManager;
    private Dictionary<string, int> _boneNameToJointIndex;
    // Use this for initialization
    void Start () {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        _boneNameToJointIndex = new Dictionary<string, int>();
        //_boneNameToJointIndex.Add("Pelvis", 0);
        //_boneNameToJointIndex.Add("L_Hip", 1);
        //_boneNameToJointIndex.Add("R_Hip", 2);
        //_boneNameToJointIndex.Add("Spine1", 3);
        //_boneNameToJointIndex.Add("L_Knee", 4);
        //_boneNameToJointIndex.Add("R_Knee", 5);
        //_boneNameToJointIndex.Add("Spine2", 6);
        //_boneNameToJointIndex.Add("L_Ankle", 7);
        //_boneNameToJointIndex.Add("R_Ankle", 8);
        //_boneNameToJointIndex.Add("Spine3", 9);
        //_boneNameToJointIndex.Add("L_Foot", 10);
        //_boneNameToJointIndex.Add("R_Foot", 11);
        //_boneNameToJointIndex.Add("Neck", 12);
        //_boneNameToJointIndex.Add("L_Collar", 13);
        //_boneNameToJointIndex.Add("R_Collar", 14);
        //_boneNameToJointIndex.Add("Head", 15);
        //_boneNameToJointIndex.Add("L_Shoulder", 16);
        //_boneNameToJointIndex.Add("R_Shoulder", 17);
        //_boneNameToJointIndex.Add("L_Elbow", 18);
        //_boneNameToJointIndex.Add("R_Elbow", 19);
        //_boneNameToJointIndex.Add("L_Wrist", 20);
        //_boneNameToJointIndex.Add("R_Wrist", 21);
        //_boneNameToJointIndex.Add("L_Hand", 22);
        //_boneNameToJointIndex.Add("R_Hand", 23);

        _boneNameToJointIndex.Add("Pelvis", 0);
        _boneNameToJointIndex.Add("R_Hip", 1);
        _boneNameToJointIndex.Add("L_Hip", 2);
        _boneNameToJointIndex.Add("Spine1", 3);
        _boneNameToJointIndex.Add("R_Knee", 4);
        _boneNameToJointIndex.Add("L_Knee", 5);
        _boneNameToJointIndex.Add("Spine2", 6);
        _boneNameToJointIndex.Add("R_Ankle", 7);
        _boneNameToJointIndex.Add("L_Ankle", 8);
        _boneNameToJointIndex.Add("Spine3", 9);
        _boneNameToJointIndex.Add("R_Foot", 10);
        _boneNameToJointIndex.Add("L_Foot", 11);
        _boneNameToJointIndex.Add("Neck", 12);
        _boneNameToJointIndex.Add("R_Collar", 13);
        _boneNameToJointIndex.Add("L_Collar", 14);
        _boneNameToJointIndex.Add("Head", 15);
        _boneNameToJointIndex.Add("R_Shoulder", 16);
        _boneNameToJointIndex.Add("L_Shoulder", 17);
        _boneNameToJointIndex.Add("R_Elbow", 18);
        _boneNameToJointIndex.Add("L_Elbow", 19);
        _boneNameToJointIndex.Add("R_Wrist", 20);
        _boneNameToJointIndex.Add("L_Wrist", 21);
        _boneNameToJointIndex.Add("R_Hand", 22);
        _boneNameToJointIndex.Add("L_Hand", 23);

    }

    // Update is called once per frame
    void Update () {
		
	}
    public void OnClick()//按钮被点击
    {
        OpenFileName ofn = new OpenFileName();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = "文本文件(*.txt)\0*.txt";
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
        ofn.title = "导入人体模型参数";
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
        if (LocalDialog.GetOpenFileName(ofn))
        {
            Debug.Log("file: " + ofn.file);
            //StartCoroutine(GetImage(ofn.file));
            AddHumanModel(ofn.file);
        }

    }
    public void SetParam(GameObject model,List<float> poseParam, List<float> shapeParam)//设置pose和shape参数
    {
        var smr = model.GetComponent<SkinnedMeshRenderer>();
        var _bones = smr.bones;
        var _boneNamePrefix = "";


        foreach (Transform bone in _bones)
        {
            if (bone.name.EndsWith("root"))
            {
                int index = bone.name.IndexOf("root");
                _boneNamePrefix = bone.name.Substring(0, index);
                break;
            }
        }
        foreach (var bone in _bones)
        {
            string boneName = bone.name;
            boneName = boneName.Replace(_boneNamePrefix, "");
            int id;
            if (_boneNameToJointIndex.TryGetValue(boneName,out id))
            {
                bone.transform.localRotation = Quaternion.Euler(new Vector3(poseParam[id * 3], poseParam[id * 3 + 1], poseParam[id * 3 + 2]));
            }

        }
        float _shapeBlendsScale = 5.0f;
        for (int i = 0; i < 10; i++)
        {
            float pos, neg;
            float beta = shapeParam[i] / _shapeBlendsScale;

            if (beta >= 0)
            {
                pos = beta;
                neg = 0.0f;
            }
            else
            {
                pos = 0.0f;
                neg = -beta;
            }

            smr.SetBlendShapeWeight(i * 2 + 0, pos * 100.0f); // map [0, 1] space to [0, 100]
            smr.SetBlendShapeWeight(i * 2 + 1, neg * 100.0f); // map [0, 1] space to [0, 100]
        }

    }
    public void AddHumanModel(string path)//点击添加一个人物模型
    {
        WWW www = new WWW("file://" + path);
        if (www != null && string.IsNullOrEmpty(www.error))
        {
            objManager.humanModelId++;
            var item = Instantiate(objManager.humanModel, new Vector3(0, 0, 0), Quaternion.identity);//实例化一个人物模型
            item.name = "SMPL" + objManager.humanModelId;
            var prefabScrollViewItem = objManager.prefabScrollViewItem;
            var model3d = objManager.model3d;
            model3d.GetComponent<Model3dController>().AddSon(item);//将解析出来的obj的父亲设置为model3d
            item.tag = Macro.UNSELECTED;//将tag设置为未选中
            item.AddComponent<Model3dItemController>();//添加该脚本

            item.GetComponent<Model3dItemController>().trianglesEditable = false;//禁用三角形编辑
            item.GetComponent<Model3dItemController>().scaleEditable = false;//禁用scale编辑

            var scrollViewItem = Instantiate(prefabScrollViewItem, new Vector3(0, 0, 0), Quaternion.identity);
            scrollViewItem.GetComponent<ScrollViewItemController>().Init(item, scrollViewModelsContent);
            var par = scrollViewItem;
            foreach (var o in item.GetComponentsInChildren<Transform>())
            {
                if (o.gameObject == par.GetComponent<ScrollViewItemController>().model)
                    continue;

                var svi = Instantiate(prefabScrollViewItem, new Vector3(0, 0, 0), Quaternion.identity);//创建对应的scrollViewItem
                svi.GetComponent<ScrollViewItemController>().Init(o.gameObject, scrollViewModelsContent);
                svi.transform.Find("ButtonDelete").gameObject.SetActive(false);//儿子们的delete按钮都不显示，不能使用
                while (par.GetComponent<ScrollViewItemController>().model != o.parent.gameObject)
                {
                    par = par.GetComponent<ScrollViewItemController>().par;
                }
                par.GetComponent<ScrollViewItemController>().AddSon(svi);
                par = svi;

                var model3dItemController = o.gameObject.AddComponent<Model3dItemController>();
                if (!o.name.Equals("m_avg_root"))//不是m_avg_root
                {
                    model3dItemController.trianglesEditable = false;//禁用三角形编辑
                    model3dItemController.positionEditable = false;//禁用坐标编辑
                    model3dItemController.scaleEditable = false;//禁用scale编辑
                }
            }
            var lines = File.ReadAllLines(path);
            var poseParam = new List<float>();
            var shapeParam = new List<float>();
            for (int i=0;i<lines.Length;i++)
            {
                if (i < 72)
                    poseParam.Add(float.Parse(lines[i]) / Mathf.PI * 180);
                else
                    shapeParam.Add(float.Parse(lines[i]));
            }

            SetParam(item, poseParam, shapeParam);
        }
        else
        {
            Debug.Log("no such txt!");
        }
    }
}
