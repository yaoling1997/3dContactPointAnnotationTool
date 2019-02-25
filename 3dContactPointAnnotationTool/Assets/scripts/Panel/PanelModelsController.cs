using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.IO;

public class PanelModelsController : MonoBehaviour {
    public const float SHAPE_BLENDS_SCALE = 5.0f;
    public GameObject scrollViewContent;
    private ObjManager objManager;
    private PanelModel_PointsInformationController panelModel_PointsInformationController;
    private Dictionary<string, int> _boneNameToJointIndex;

    // Use this for initialization
    void Start () {
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        panelModel_PointsInformationController = objManager.panelModel_PointsInformationController;
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

        AddMainCameraSvi();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void ButtonAddHumanModelOnClick()//AddHumanModel按钮被点击
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
        else
        {
            AddHumanModel("");
        }

    }
    public void ButtonClearOnClick()//clear按钮被点击
    {
        foreach (var item in scrollViewContent.GetComponentsInChildren<ScrollViewItemController>())
        {
            item.ButtonDeleteOnClick();
        }
        objManager.humanModelId = 0;
    }
    public void SetSMPLParam(GameObject model, List<float> poseParam, List<float> shapeParam)//设置pose和shape参数
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
            if (_boneNameToJointIndex.TryGetValue(boneName, out id))
            {
                bone.transform.localRotation = Quaternion.Euler(new Vector3(poseParam[id * 3], poseParam[id * 3 + 1], poseParam[id * 3 + 2]));
            }

        }        
        for (int i = 0; i < 10; i++)
        {
            float pos, neg;
            float beta = shapeParam[i] / SHAPE_BLENDS_SCALE;

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
    public void GetSMPLParam(GameObject model, out List<float> poseParam, out List<float> shapeParam)//设置pose和shape参数
    {
        var smr = model.GetComponent<SkinnedMeshRenderer>();
        var _bones = smr.bones;
        var _boneNamePrefix = "";
        //poseParam = new List<float>();
        shapeParam = new List<float>();
        //还原姿势参数
        foreach (Transform bone in _bones)
        {
            if (bone.name.EndsWith("root"))
            {
                int index = bone.name.IndexOf("root");
                _boneNamePrefix = bone.name.Substring(0, index);
                break;
            }
        }
        var poseParamArray = new float[_bones.Length * 3];
        foreach (var bone in _bones)
        {
            string boneName = bone.name;
            boneName = boneName.Replace(_boneNamePrefix, "");
            int id;
            if (_boneNameToJointIndex.TryGetValue(boneName, out id))
            {
                var ea = bone.transform.localEulerAngles;
                poseParamArray[id * 3] = ea.x;
                poseParamArray[id * 3 + 1] = ea.y;
                poseParamArray[id * 3 + 2] = ea.z;                
            }
        }
        poseParam = new List<float>(poseParamArray);
        //还原形状参数
        for (int i = 0; i < 10; i++)
        {
            var pos = smr.GetBlendShapeWeight(i * 2 + 0) / 100;
            var neg = smr.GetBlendShapeWeight(i * 2 + 1) / 100;
            float beta = Mathf.Abs(pos) > Mathf.Abs(neg) ? pos : -neg;
            shapeParam.Add(beta * SHAPE_BLENDS_SCALE);
        }
    }

    private void AddMainCameraSvi()//添加与主相机对应的svi
    {
        var mainCamera = objManager.mainCamera.gameObject;//得到主相机        
        var prefabScrollViewItem = objManager.prefabScrollViewItem;
        mainCamera.AddComponent<ItemController>().SetModelType(ItemController.ModelType.MAIN_CAMERA); //添加该脚本

        var scrollViewItem = Instantiate(prefabScrollViewItem, new Vector3(0, 0, 0), Quaternion.identity);
        scrollViewItem.GetComponent<ScrollViewItemController>().Init(mainCamera, scrollViewContent, "MainCamera");
    }
    public GameObject AddHumanModel(string path)//点击添加一个人物模型
    {
        objManager.humanModelId++;
        var item = Instantiate(objManager.humanModel, new Vector3(0, 0, 0), Quaternion.identity);//实例化一个人物模型
        item.name = "SMPL" + objManager.humanModelId;
        var prefabScrollViewItem = objManager.prefabScrollViewItem;
        var model3d = objManager.model3d;
        model3d.GetComponent<Model3dController>().AddSon(item);//将解析出来的obj的父亲设置为model3d
        item.tag = Macro.UNSELECTED;//将tag设置为未选中
        item.GetComponent<SkinnedMeshRenderer>().material = panelModel_PointsInformationController.GetMaterial();
        item.AddComponent<Model3dItemController>();//添加该脚本
        item.AddComponent<ItemController>().SetModelType(ItemController.ModelType.SMPL_MODEL);//添加该脚本

        var scrollViewItem = Instantiate(prefabScrollViewItem, new Vector3(0, 0, 0), Quaternion.identity);
        scrollViewItem.GetComponent<ScrollViewItemController>().Init(item, scrollViewContent);
        var par = scrollViewItem;
        foreach (var o in item.GetComponentsInChildren<Transform>())
        {
            if (o.gameObject == par.GetComponent<ScrollViewItemController>().model)
                continue;

            o.gameObject.AddComponent<ItemController>().SetModelType(ItemController.ModelType.SMPL_MODEL_JOINT);
            if (!o.name.Equals("m_avg_root"))//不是m_avg_root
            {
                //只有m_avg_root进行scale变换才能得到准确的接触点计算结果
                //itemController.positionEditable = false;//禁用坐标编辑
                //itemController.scaleEditable = false;//禁用scale编辑
            }
            else//大部分数据都需要镜像对称一下,所以简单粗暴地把scaleX设置为-1,有可能会造成选项卡左右颠倒
            {
                o.transform.localScale = new Vector3(-o.transform.localScale.x, o.transform.localScale.y, o.transform.localScale.z);                
            }
            var sviName = "";//设置svi显示的名字,仅仅是显示的
            for (int i = 6; i < o.name.Length; i++)
            {
                sviName += o.name[i];//只取m_avg_后面的部分显示
            }
            if (sviName[0] == 'L')
            {
                var a = sviName.ToCharArray();
                a[0] = 'R';
                sviName = new string(a);
            }
            else if (sviName[0] == 'R')
            {
                var a = sviName.ToCharArray();
                a[0] = 'L';
                sviName = new string(a);
            }
            //添加了ItemController后再添加对应的scrollViewItem
            var svi = Instantiate(prefabScrollViewItem, new Vector3(0, 0, 0), Quaternion.identity);//创建对应的scrollViewItem
            svi.GetComponent<ScrollViewItemController>().Init(o.gameObject, scrollViewContent, sviName);
            while (par.GetComponent<ScrollViewItemController>().model != o.parent.gameObject)
            {
                par = par.GetComponent<ScrollViewItemController>().par;
            }
            par.GetComponent<ScrollViewItemController>().AddSon(svi);
            par = svi;

        }
        if (File.Exists(path))
        {
            var lines = File.ReadAllLines(path);
            var cameraPos = new Vector3(float.Parse(lines[0]), float.Parse(lines[1]), float.Parse(lines[2]));
            objManager.mainCamera.transform.position = cameraPos;
            objManager.mainCamera.transform.LookAt(Vector3.zero);
            objManager.mainCamera.transform.position = objManager.mainCamera.transform.position + objManager.mainCamera.transform.forward * 30;
            var poseParam = new List<float>();
            var shapeParam = new List<float>();
            for (int i = 3; i < 75; i++)
            {
                    poseParam.Add(float.Parse(lines[i]) / Mathf.PI * 180);
            }
            for (int i = 75 ; i < 85 ; i++)
                shapeParam.Add(float.Parse(lines[i]));
            SetSMPLParam(item, poseParam, shapeParam);
        }
        else
        {
            Debug.Log("no such txt!");
        }
        return item;
    }

}
