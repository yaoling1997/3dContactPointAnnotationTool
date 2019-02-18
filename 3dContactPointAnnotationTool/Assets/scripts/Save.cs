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
        public SaveVector3 position;
        public SaveVector3 eulerAngles;
        public SaveVector3 scale;
        public int id;//在panelModels中的序号
        public SaveObjModel(string path,Vector3 position, Vector3 eulerAngles, Vector3 scale,int id)
        {
            this.path = path;
            this.position = new SaveVector3(position.x, position.y, position.z);
            this.eulerAngles = new SaveVector3(eulerAngles.x, eulerAngles.y, eulerAngles.z);
            this.scale = new SaveVector3(scale.x, scale.y, scale.z);
            this.id = id;
        }
    }
    public SaveCamera mainCamera;
    public SaveImage image;
    public List<SaveContactPoint> contactPointList;
    public List<SaveObjModel> objModelList;//存objModel的列表
    public void SaveContactPointsInfo()//存储接触点信息
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
    public void SaveObjModelsInfo() {//存储Obj模型信息
        var objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        var scrollViewContent = objManager.panelModels.GetComponent<PanelModelsController>().scrollViewContent;
        foreach (var item in scrollViewContent.GetComponentsInChildren<ScrollViewItemController>()) {

        }
    }
    public static void SaveByBin(string path)
    {
        var objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();        
        //序列化过程（将save对象转换为字节流）

        //创建save对象并保存当前游戏状态
        Save save = new Save();

        save.mainCamera = new SaveCamera(objManager.mainCamera.transform.position, objManager.mainCamera.transform.eulerAngles);//存储主相机的位置和角度

        var pbic = objManager.panelBackgroundImageControllerScript;
        save.image = new SaveImage(objManager.imagePath, pbic.inputFieldAlpha.text, pbic.inputFieldScale.text);//存储图片路径,透明度和缩放大小
        
        save.SaveContactPointsInfo();//存储接触点信息

        Debug.Log("SaveByBin： ");
        //创建一个二进制格式化程序
        BinaryFormatter bf = new BinaryFormatter();
        //创建一个文件流
        var fileStream = File.Create(path);

        //用二进制格式化程序的序列化方法来序列化Save对象，参数：创建的文件流和需要序列化的对象
        bf.Serialize(fileStream, save);
        //关闭流
        fileStream.Close();
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


        Debug.Log("LoadByBin： ");
    }
}
