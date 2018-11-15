&emsp;&emsp;今天要做的第一件事就是把obj文件里不同的对象分割开来。
&emsp;&emsp;通过仔细观察发现obj文件中以"o "开头的行会跟着一个对象的名字。g代表对象所属组名，我这里只要用到对象名就行了所以没管组名了。然后这个o的位置在该对象的vn和f之间。记录一下f开始的下标就能够把obj文件中的多个对象分离出来。
<img src="https://i.loli.net/2018/11/13/5bea2f17a5831.png" alt="0.png" title="0.png" style="zoom:50%"/>
&emsp;&emsp;具体代码我也是在别人的代码基础上改的，不过还是贴一贴吧。
&emsp;&emsp;这个是ObjFormatAnalyzer.cs的：

```
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hont
{
    public class ObjFormatAnalyzer
    {
        public struct Vector
        {
            public float X;
            public float Y;
            public float Z;
        }

        public struct FacePoint
        {
            public int VertexIndex;
            public int TextureIndex;
            public int NormalIndex;
        }

        public struct Face
        {
            public FacePoint[] Points;
            public bool IsQuad;
        }

        public Vector[] VertexArr;
        public Vector[] VertexNormalArr;
        public Vector[] VertexTextureArr;
        public Face[] FaceArr;
        public int[] ObjFaceBegin;
        public string[] ObjName;

        public void Analyze(string content)
        {
            content = content.Replace("\r", string.Empty).Replace('\t', ' ');

            var lines = content.Split('\n');
            var vertexList = new List<Vector>();
            var vertexNormalList = new List<Vector>();
            var vertexTextureList = new List<Vector>();
            var faceList = new List<Face>();
            var objFaceBeginList = new List<int>();
            var objNameList = new List<string>();

            for (int i = 0; i < lines.Length; i++)
            {
                var currentLine = lines[i];
                //Debug.Log("current line: " + i);
                //Debug.Log("content: " + currentLine);
                //Debug.Log("size: " + currentLine.Length);
                if (currentLine.Contains("#") || currentLine.Length == 0)
                {
                    continue;
                }

                if (currentLine.Contains("v "))
                {
                    var splitInfo = currentLine.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    vertexList.Add(new Vector() { X = float.Parse(splitInfo[1]), Y = float.Parse(splitInfo[2]), Z = float.Parse(splitInfo[3]) });
                }
                else if (currentLine.Contains("vt "))
                {
                    var splitInfo = currentLine.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    vertexTextureList.Add(new Vector() { X = float.Parse(splitInfo[1]), Y = float.Parse(splitInfo[2]), Z = float.Parse(splitInfo[3]) });
                }
                else if (currentLine.Contains("vn "))
                {
                    var splitInfo = currentLine.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    vertexNormalList.Add(new Vector() { X = float.Parse(splitInfo[1]), Y = float.Parse(splitInfo[2]), Z = float.Parse(splitInfo[3]) });
                }
                else if (currentLine.Contains("f "))
                {
                    var splitInfo = currentLine.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    var isQuad = splitInfo.Length > 4;
                    var face1 = splitInfo[1].Split('/');
                    var face2 = splitInfo[2].Split('/');
                    var face3 = splitInfo[3].Split('/');
                    var face4 = isQuad ? splitInfo[4].Split('/') : null;
                    var face = new Face();
                    face.Points = new FacePoint[4];
                    face.Points[0] = new FacePoint() { VertexIndex = int.Parse(face1[0]), TextureIndex = int.Parse(face1[1]), NormalIndex = int.Parse(face1[2]) };
                    face.Points[1] = new FacePoint() { VertexIndex = int.Parse(face2[0]), TextureIndex = int.Parse(face2[1]), NormalIndex = int.Parse(face2[2]) };
                    face.Points[2] = new FacePoint() { VertexIndex = int.Parse(face3[0]), TextureIndex = int.Parse(face3[1]), NormalIndex = int.Parse(face3[2]) };
                    face.Points[3] = isQuad ? new FacePoint() { VertexIndex = int.Parse(face4[0]), TextureIndex = int.Parse(face4[1]), NormalIndex = int.Parse(face4[2]) } : default(FacePoint);
                    face.IsQuad = isQuad;

                    faceList.Add(face);
                }
                else if (currentLine.Contains("o ")) {
                    string objName = "";
                    int p = currentLine.IndexOf('o');
                    int length = currentLine.Length;
                    for (p++; currentLine[p] == ' '; p++) ;
                    for (; p < length; p++)
                        objName += currentLine[p];
                    objFaceBeginList.Add(faceList.Count);
                    objNameList.Add(objName);
                }
            }

            VertexArr = vertexList.ToArray();
            VertexNormalArr = vertexNormalList.ToArray();
            VertexTextureArr = vertexTextureList.ToArray();
            FaceArr = faceList.ToArray();
            ObjFaceBegin = objFaceBeginList.ToArray();
            ObjName = objNameList.ToArray();
        }
    }
}
```
 
&emsp;&emsp;这个是ObjFormatAnalyzerFactory.cs的：
```
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Hont
{
    public static class ObjFormatAnalyzerFactory
    {
        public static List<GameObject> AnalyzeToGameObject(string objFilePath)
        {
            if (!File.Exists(objFilePath)) return null;

            var objFormatAnalyzer = new ObjFormatAnalyzer();

            objFormatAnalyzer.Analyze(File.ReadAllText(objFilePath));
            int length = objFormatAnalyzer.ObjFaceBegin.Length;
            var re = new List<GameObject>();
            var sourceVertexArr = objFormatAnalyzer.VertexArr;
            var sourceUVArr = objFormatAnalyzer.VertexTextureArr;
            var faceArr = objFormatAnalyzer.FaceArr;
            var notQuadFaceArr = objFormatAnalyzer.FaceArr.Where(m => !m.IsQuad).ToArray();
            var quadFaceArr = objFormatAnalyzer.FaceArr.Where(m => m.IsQuad).ToArray();

            for (int objId = 0; objId < length; objId++)
            {
                var go = new GameObject();
                go.name = objFormatAnalyzer.ObjName[objId];
                var meshRenderer = go.AddComponent<MeshRenderer>();
                var meshFilter = go.AddComponent<MeshFilter>();

                var mesh = new Mesh();

                var vertexList = new List<Vector3>();
                var uvList = new List<Vector2>();

                int faceBeginId = objFormatAnalyzer.ObjFaceBegin[objId];
                int faceEndId = faceArr.Length;//左闭右开
                if (objId < length - 1)
                    faceEndId = objFormatAnalyzer.ObjFaceBegin[objId + 1];
                int triangleNum = 0;
                for (int i = faceBeginId; i < faceEndId; i++)
                    if (faceArr[i].IsQuad)
                        triangleNum += 6;
                    else triangleNum += 3;
                var triangles = new int[triangleNum];
                for (int i = faceBeginId, j = 0; i < faceEndId; i++)
                {
                    var currentFace = faceArr[i];

                    triangles[j] = j;
                    triangles[j + 1] = j + 1;
                    triangles[j + 2] = j + 2;
                    
                    var vec = sourceVertexArr[currentFace.Points[0].VertexIndex - 1];
                    vertexList.Add(new Vector3(vec.X, vec.Y, vec.Z));

                    var uv = sourceUVArr[currentFace.Points[0].TextureIndex - 1];
                    uvList.Add(new Vector2(uv.X, uv.Y));

                    vec = sourceVertexArr[currentFace.Points[1].VertexIndex - 1];
                    vertexList.Add(new Vector3(vec.X, vec.Y, vec.Z));

                    uv = sourceUVArr[currentFace.Points[1].TextureIndex - 1];
                    uvList.Add(new Vector2(uv.X, uv.Y));

                    vec = sourceVertexArr[currentFace.Points[2].VertexIndex - 1];
                    vertexList.Add(new Vector3(vec.X, vec.Y, vec.Z));

                    uv = sourceUVArr[currentFace.Points[2].TextureIndex - 1];
                    uvList.Add(new Vector2(uv.X, uv.Y));

                    if (currentFace.IsQuad)
                    {
                        triangles[j + 3] = j + 3;
                        triangles[j + 4] = j + 4;
                        triangles[j + 5] = j + 5;
                        j += 3;

                        vec = sourceVertexArr[currentFace.Points[0].VertexIndex - 1];
                        vertexList.Add(new Vector3(vec.X, vec.Y, vec.Z));

                        uv = sourceUVArr[currentFace.Points[0].TextureIndex - 1];
                        uvList.Add(new Vector2(uv.X, uv.Y));

                        vec = sourceVertexArr[currentFace.Points[2].VertexIndex - 1];
                        vertexList.Add(new Vector3(vec.X, vec.Y, vec.Z));

                        uv = sourceUVArr[currentFace.Points[2].TextureIndex - 1];
                        uvList.Add(new Vector2(uv.X, uv.Y));

                        vec = sourceVertexArr[currentFace.Points[3].VertexIndex - 1];
                        vertexList.Add(new Vector3(vec.X, vec.Y, vec.Z));

                        uv = sourceUVArr[currentFace.Points[3].TextureIndex - 1];
                        uvList.Add(new Vector2(uv.X, uv.Y));
                    }

                    j += 3;
                }

                mesh.vertices = vertexList.ToArray();
                mesh.uv = uvList.ToArray();
                mesh.triangles = triangles;

                meshFilter.mesh = mesh;
                meshRenderer.material = new Material(Shader.Find("Standard"));
                //go.transform.parent = re.transform;
                re.Add(go);
            }
            return re;
        }
    }
}
```
&emsp;&emsp;用的话就这样就行了，让解析出来的obj都成为一个gameObject的儿子。或者随便怎么玩都行。
```
            var re = ObjFormatAnalyzerFactory.AnalyzeToGameObject(path);
            foreach (var item in re)
                item.transform.parent = model3d.transform;            

```
&emsp;&emsp;运行一下，可以看见Model3d下有两个对象分别对应人和椅子了。
<img src="https://i.loli.net/2018/11/13/5bea2f1857449.png" alt="1.png" title="1.png" style="zoom:50%"/>
&emsp;&emsp;接下来想弄个列表来让使用者可以选中场景里的对象，于是用到了scrollView，具体怎么使用ScrollView请看：[Unity5.4.1 Scroll_View的简单使用](https://blog.csdn.net/qq_26437925/article/details/54025053)。把按钮当做content的子对象就成了这副模样，并且实现了点击按钮可以改变按钮颜色表示该物体被选中。
<img src="https://i.loli.net/2018/11/13/5bea2f18a9526.png" alt="2.png" title="2.png" style="zoom:50%"/>
&emsp;&emsp;光改变按钮颜色还不够，于是又想着改变按钮对应的模型颜色。我是每个按钮的脚本绑定了对应的模型对象，出发点击事件就直接改变模型材质的颜色即可。具体怎么改变模型颜色请看：[Unity——通过脚本给物体改变颜色](https://blog.csdn.net/yy763496668/article/details/53015674)。然后实现了改变按钮对应模型颜色的功能。

<img src="https://i.loli.net/2018/11/13/5bea2f18a9a69.png" alt="3.png" title="3.png" style="zoom:50%"/>
&emsp;&emsp;整体效果如下，输入obj模型和图像的路径点击ok会加载obj模型和图像，并在scrollView中添加与obj模型对应的按钮，点击即可选中模型。
<img src="https://i.loli.net/2018/11/13/5bea2f18aa489.png" alt="4.png" title="4.png" style="zoom:50%"/>
&emsp;&emsp;之后的工作是如何自动求解3d模型的接触点了，先添加个按钮，明天再想办法做吧。
<img src="https://i.loli.net/2018/11/13/5bea2f18aa708.png" alt="5.png" title="5.png" style="zoom:50%"/>
&emsp;&emsp;设置tag的时候必须现在Tags and Layers manager中定义。
