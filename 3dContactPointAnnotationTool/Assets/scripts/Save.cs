using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
[System.Serializable]
public class Save
{
    public List<int> livingTargetPositions = new List<int>();
    public List<int> livingMonsterTypes = new List<int>();

    public int shootNum = 0;
    public int score = 0;
    public static void SaveByBin(string path)
    {
        var objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();        
        //序列化过程（将save对象转换为字节流）
        //创建save对象并保存当前游戏状态
        Save save = new Save();
        save.score = 1;
        Debug.Log("SaveByBin： " + save.score);
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
        Debug.Log("LoadByBin： " + save.score);
    }
}
