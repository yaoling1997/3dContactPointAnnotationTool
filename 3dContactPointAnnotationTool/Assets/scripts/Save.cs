using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
[System.Serializable]
public class Save
{
    [System.Serializable]
    public class SaveVector3 {
        public float x, y, z;
        public SaveVector3(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public SaveVector3(Vector3 v) {
            x = v.x;
            y = v.y;
            z = v.z;
        }
        public Vector3 ToVector3() {
            return new Vector3(x,y,z);
        }
    }
    [System.Serializable]
    public class SaveCamera {
        public SaveVector3 position;//相机位置
        public SaveVector3 eulerAngles;//欧拉角
        public SaveCamera(Vector3 position,Vector3 eulerAngles) {
            this.position = new SaveVector3(position.x, position.y, position.z);
            this.eulerAngles = new SaveVector3(eulerAngles.x, eulerAngles.y, eulerAngles.z);
        }
    }
    [System.Serializable]
    public class SaveImage {
        public string path;//图片路径
        public string alpha;//透明度
        public string scale;//缩放
        public SaveImage(string path,string alpha,string scale) {
            this.path = path;
            this.alpha = alpha;
            this.scale = scale;
        }
    }
    [System.Serializable]
    public class SaveContactPoint {
        public SaveVector3 position;
        public SaveVector3 eulerAngles;
        public SaveVector3 scale;
        public SaveContactPoint(Vector3 position, Vector3 eulerAngles, Vector3 scale) {
            this.position = new SaveVector3(position.x, position.y, position.z);
            this.eulerAngles = new SaveVector3(eulerAngles.x, eulerAngles.y, eulerAngles.z);
            this.scale = new SaveVector3(scale.x, scale.y, scale.z);
        }
    }
    [System.Serializable]
    public class SaveObjModel
    {
        public string path;//obj文件路径
        public List<SaveVector3> position;//列表中第一个存的是obj_model_root的信息
        public List<SaveVector3> eulerAngles;
        public List<SaveVector3> scale;
        public int id;//在panelModels中的序号
        public SaveObjModel(string path,List<Vector3> position, List<Vector3> eulerAngles, List<Vector3> scale,int id)
        {
            this.path = path;
            this.position = Vector3ToSaveVector3(position);
            this.eulerAngles = Vector3ToSaveVector3(eulerAngles);
            this.scale = Vector3ToSaveVector3(scale);
            this.id = id;
        }
    }
    //[System.Serializable]
    //public class SaveSMPLJoint//smpl模型关节点
    //{
    //    public SaveVector3 eulerAngles;
    //    public SaveSMPLJoint(Vector3 eulerAngles) {
    //        this.eulerAngles = new SaveVector3(eulerAngles.x, eulerAngles.y, eulerAngles.z);
    //    }
    //}
    [System.Serializable]
    public class SaveSMPL//smpl模型
    {
        public SaveVector3 position;
        public SaveVector3 eulerAngles;
        public List<float> poseParam;
        public List<float> shapeParam;
        public int id;
        public SaveSMPL(Vector3 position,Vector3 eulerAngles, List<float> poseParam, List<float> shapeParam, int id)
        {
            this.position = new SaveVector3(position.x, position.y, position.z);
            this.eulerAngles = new SaveVector3(eulerAngles.x, eulerAngles.y, eulerAngles.z);
            this.poseParam = poseParam;
            this.shapeParam = shapeParam;
            this.id = id;
        }
    }
    [System.Serializable]
    public class SaveItemWarehouse//ItemWarehouse的路径
    {
        public string path;
        public SaveItemWarehouse(string path) {
            this.path = path;
        }
    }
    public SaveCamera mainCamera;
    public SaveImage image;
    public List<SaveContactPoint> contactPointList;
    public List<SaveObjModel> objModelList;//存objModel的列表
    public List<SaveSMPL> SMPLList;//存SMPL模型的列表
    public SaveItemWarehouse itemWarehouse;
    public static SaveVector3 Vector3ToSaveVector3(Vector3 v) {
        return new SaveVector3(v.x,v.y,v.z);
    }
    public static List<SaveVector3> Vector3ToSaveVector3(List<Vector3> v){
        var re= new List<SaveVector3>();
        foreach (var item in v) {
            re.Add(Vector3ToSaveVector3(item));
        }
        return re;
    }
    public static List<Vector3> SaveVector3ToVector3(List<SaveVector3> v)
    {
        var re = new List<Vector3>();
        foreach (var item in v)
        {
            re.Add(item.ToVector3());
        }
        return re;
    }
    public static void SaveCameraInfo(out SaveCamera saveCamera)//存储主相机的位置和角度
    {
        var objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        saveCamera = new SaveCamera(objManager.mainCamera.transform.position, objManager.mainCamera.transform.eulerAngles);
    }
    public static void SaveContactPointsInfo(out List<SaveContactPoint> contactPointList)//存储接触点信息
    {
        var objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        var scrollViewContent = objManager.panelContactPoints.GetComponent<PanelContactPointsController>().scrollViewContent;
        contactPointList = new List<SaveContactPoint>();
        foreach (var item in scrollViewContent.GetComponentsInChildren<ScrollViewItemController>())
        {
            var t = item.model.transform;
            contactPointList.Add(new SaveContactPoint(t.position,t.eulerAngles,t.localScale));
        }
    }
    public void LoadContactPoints()//还原接触点
    {
        var objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        foreach (var item in contactPointList)
        {
            objManager.CreateContactPointSphere(item.position.ToVector3(), item.eulerAngles.ToVector3(), item.scale.ToVector3());
        }
    }
    public static void SaveModelsInfo(out List<SaveObjModel> objModelList,out List<SaveSMPL> SMPLList)//存储Obj模型和SMPL模型信息
    {
        var objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        var scrollViewContent = objManager.panelModels.GetComponent<PanelModelsController>().scrollViewContent;
        objModelList = new List<SaveObjModel>();
        SMPLList = new List<SaveSMPL>();
        int id = 0;
        foreach (var item in scrollViewContent.GetComponentsInChildren<ScrollViewItemController>()) {
            var t = item.model.transform;
            var ic = item.model.GetComponent<ItemController>();
            if (ic.modelType.Equals(ItemController.ModelType.OBJ_MODEL_ROOT))//是obj模型的root
            {
                var position = new List<Vector3>();
                var eulerAngles = new List<Vector3>();
                var scale = new List<Vector3>();
                position.Add(t.position);
                eulerAngles.Add(t.eulerAngles);
                scale.Add(t.localScale);
                foreach (var s in item.GetSons()) {
                    var st = s.GetComponent<ScrollViewItemController>().model.transform;
                    position.Add(st.position);
                    eulerAngles.Add(st.eulerAngles);
                    scale.Add(st.localScale);
                }
                objModelList.Add(new SaveObjModel(ic.path,position,eulerAngles,scale, id));
            }
            else if (ic.modelType.Equals(ItemController.ModelType.SMPL_MODEL))//是SMPL模型
            {
                List<float> poseParam;
                List<float> shapeParam;                
                objManager.panelModels.GetComponent<PanelModelsController>().GetSMPLParam(item.model,out poseParam,out shapeParam);
                SMPLList.Add(new SaveSMPL(t.position,t.eulerAngles, poseParam,shapeParam, id));
            }
            id++;//第几个选项卡
        }
    }
    public void LoadModels()//还原Obj模型和SMPL模型
    {
        var objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        var pmc = objManager.panelModels.GetComponent<PanelModelsController>();
        Debug.Log("objModelList.Count:"+ objModelList.Count);
        int i = 0,j = 0;
        while (i < objModelList.Count || j < SMPLList.Count) {
            if (j == SMPLList.Count || (i < objModelList.Count && objModelList[i].id < SMPLList[j].id))//加载obj
            {
                var info = objModelList[i];
                Debug.Log("info.path:" + info.path);
                objManager.LoadObj(info.path, SaveVector3ToVector3(info.position), SaveVector3ToVector3(info.eulerAngles), SaveVector3ToVector3(info.scale));
                i++;
            }
            else {
                var m=pmc.AddHumanModel(null);
                var info = SMPLList[j];
                pmc.SetSMPLParam(m, info.poseParam, info.shapeParam);
                m.transform.position = info.position.ToVector3();
                m.transform.eulerAngles = info.eulerAngles.ToVector3();
                j++;
            }
        }
    }
    public static void SaveByBin(string path)
    {
        var objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();        
        //序列化过程（将save对象转换为字节流）

        //创建save对象并保存当前游戏状态
        Save save = new Save();

        SaveCameraInfo(out save.mainCamera);//存储主相机的位置和角度

        var pbic = objManager.panelBackgroundImageControllerScript;
        save.image = new SaveImage(objManager.imagePath, pbic.inputFieldAlpha.text, pbic.inputFieldScale.text);//存储图片路径,透明度和缩放大小
        
        SaveContactPointsInfo(out save.contactPointList);//存储接触点信息

        SaveModelsInfo(out save.objModelList,out save.SMPLList);//存储Obj模型和SMPL模型信息

        save.itemWarehouse = new SaveItemWarehouse(objManager.dontDestroyController.itemWarehousePath);//存储ItemWarehouse路径

        Debug.Log("SaveByBin： ");
        //创建一个二进制格式化程序
        BinaryFormatter bf = new BinaryFormatter();
        //创建一个文件流
        var fileStream = File.Create(path);

        //用二进制格式化程序的序列化方法来序列化Save对象，参数：创建的文件流和需要序列化的对象
        bf.Serialize(fileStream, save);
        //关闭流
        fileStream.Close();
        Debug.Log("Save succeed!");
    }
    public static void LoadByBin(string path)
    {
        if (!File.Exists(path))//文件不存在
            return;
        //反序列化过程
        //创建一个二进制格式化程序
        BinaryFormatter bf = new BinaryFormatter();
        //打开一个文件流
        FileStream fileStream = File.Open(path, FileMode.Open);
        //调用格式化程序的反序列化方法，将文件流转换为一个save对象
        Save save = bf.Deserialize(fileStream) as Save;
        //关闭文件流
        fileStream.Close();

        //加载save中的数据
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);        
        var objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();

        objManager.mainCamera.transform.position = save.mainCamera.position.ToVector3();//还原主相机
        objManager.mainCamera.transform.eulerAngles = save.mainCamera.eulerAngles.ToVector3();        

        objManager.LoadImage(save.image.path);//还原图片
        var pbic = objManager.panelBackgroundImageControllerScript;
        pbic.inputFieldAlpha.text = save.image.alpha;
        pbic.inputFieldScale.text = save.image.scale;

        var pcpc = objManager.panelContactPoints.GetComponent<PanelContactPointsController>();
        pcpc.ButtonClearOnClick();//清空当前接触点
        save.LoadContactPoints();//还原接触点

        var pmc = objManager.panelModels.GetComponent<PanelModelsController>();
        pmc.ButtonClearOnClick();//清空当前模型        
        save.LoadModels();//还原模型

        objManager.panelItemWareHouseController.AddItemWarehouse(save.itemWarehouse.path);//加载ItemWarehouse

        Debug.Log("LoadByBin： ");
    }
}
