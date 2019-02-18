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
    public SaveCamera mainCamera;
    public SaveImage image;
    public static void SaveByBin(string path)
    {
        var objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();        
        //序列化过程（将save对象转换为字节流）

        //创建save对象并保存当前游戏状态
        Save save = new Save();
        save.mainCamera = new SaveCamera(objManager.mainCamera.transform.position, objManager.mainCamera.transform.eulerAngles);//存储主相机的位置和角度
        var pbic = objManager.panelBackgroundImageControllerScript;
        save.image = new SaveImage(objManager.imagePath, pbic.inputFieldAlpha.text, pbic.inputFieldScale.text);//存储图片路径,透明度和缩放大小

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
        objManager.mainCamera.transform.position = save.mainCamera.position.ToVector3();
        objManager.mainCamera.transform.eulerAngles = save.mainCamera.eulerAngles.ToVector3();        
        objManager.LoadImage(save.image.path);
        var pbic = objManager.panelBackgroundImageControllerScript;
        pbic.inputFieldAlpha.text = save.image.alpha;
        pbic.inputFieldScale.text = save.image.scale;
        Debug.Log("LoadByBin： ");
    }
}
