&emsp;&emsp;调了半天发现是逻辑错误，改了一下终于没那么奇怪了：
<img src="https://i.loli.net/2018/11/14/5beb8ac20962b.png" alt="1.png" title="1.png" style="zoom:50%"/>
&emsp;&emsp;但是有的接触点很明显跑偏了。再回顾一下自己是怎么求的，我是直接用的下面的代码求解一个点是否在另一个物体内部：
```
var bounds = uso.mesh.bounds;
if (bounds.Contains(mesh.vertices[mesh.triangles[i+j]]))
{
    xj = true;
}

```
&emsp;&emsp;这个bounds我又去unity文档上查了一下，发现它只是物体的包围盒，所以显然会得到多余的点，没办法只能再改了。
&emsp;&emsp;既然unity没有自带的方法那就自己算得了，思路是对于一个给定点枚举另一个三维模型的三角面片通过面片法向量和面上一点通过点积的正负来判断。一个三角面片的法向量貌似是通过obj模型对应顶点对应的点的法向量相加再除以3得到的。然而网上的那个代码没有把顶点法向量读到gameobject中，于是自己修改如下：
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
            var sourceVertexNormalArr = objFormatAnalyzer.VertexNormalArr;
            var sourceUVArr = objFormatAnalyzer.VertexTextureArr;
            var faceArr = objFormatAnalyzer.FaceArr;

            for (int objId = 0; objId < length; objId++)
            {
                var go = new GameObject();
                go.name = objFormatAnalyzer.ObjName[objId];
                var meshRenderer = go.AddComponent<MeshRenderer>();
                var meshFilter = go.AddComponent<MeshFilter>();

                var mesh = new Mesh();

                var vertexList = new List<Vector3>();
                var vertexNormalList = new List<Vector3>();
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
                    vec = sourceVertexNormalArr[currentFace.Points[0].VertexIndex - 1];
                    vertexNormalList.Add(new Vector3(vec.X, vec.Y, vec.Z));
                    var uv = sourceUVArr[currentFace.Points[0].TextureIndex - 1];
                    uvList.Add(new Vector2(uv.X, uv.Y));

                    vec = sourceVertexArr[currentFace.Points[1].VertexIndex - 1];
                    vertexList.Add(new Vector3(vec.X, vec.Y, vec.Z));
                    vec = sourceVertexNormalArr[currentFace.Points[1].VertexIndex - 1];
                    vertexNormalList.Add(new Vector3(vec.X, vec.Y, vec.Z));
                    uv = sourceUVArr[currentFace.Points[1].TextureIndex - 1];
                    uvList.Add(new Vector2(uv.X, uv.Y));

                    vec = sourceVertexArr[currentFace.Points[2].VertexIndex - 1];
                    vertexList.Add(new Vector3(vec.X, vec.Y, vec.Z));
                    vec = sourceVertexNormalArr[currentFace.Points[2].VertexIndex - 1];
                    vertexNormalList.Add(new Vector3(vec.X, vec.Y, vec.Z));
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
                        vec = sourceVertexNormalArr[currentFace.Points[0].VertexIndex - 1];
                        vertexNormalList.Add(new Vector3(vec.X, vec.Y, vec.Z));
                        uv = sourceUVArr[currentFace.Points[0].TextureIndex - 1];
                        uvList.Add(new Vector2(uv.X, uv.Y));

                        vec = sourceVertexArr[currentFace.Points[2].VertexIndex - 1];
                        vertexList.Add(new Vector3(vec.X, vec.Y, vec.Z));
                        vec = sourceVertexNormalArr[currentFace.Points[2].VertexIndex - 1];
                        vertexNormalList.Add(new Vector3(vec.X, vec.Y, vec.Z));
                        uv = sourceUVArr[currentFace.Points[2].TextureIndex - 1];
                        uvList.Add(new Vector2(uv.X, uv.Y));

                        vec = sourceVertexArr[currentFace.Points[3].VertexIndex - 1];
                        vertexList.Add(new Vector3(vec.X, vec.Y, vec.Z));
                        vec = sourceVertexNormalArr[currentFace.Points[3].VertexIndex - 1];
                        vertexNormalList.Add(new Vector3(vec.X, vec.Y, vec.Z));
                        uv = sourceUVArr[currentFace.Points[3].TextureIndex - 1];
                        uvList.Add(new Vector2(uv.X, uv.Y));
                    }

                    j += 3;
                }

                mesh.vertices = vertexList.ToArray();
                mesh.normals = vertexNormalList.ToArray();
                mesh.uv = uvList.ToArray();
                mesh.triangles = triangles;

                meshFilter.mesh = mesh;
                meshRenderer.material = new Material(Shader.Find("Standard"));
                //meshRenderer.material = new Material(Shader.Find("UCLA Game Lab/Wireframe/Single-Sided"));                
                re.Add(go);
            }
            return re;
        }
    }
}
```
&emsp;&emsp;发现得到模型的光照效果也不一样了，加入法向量之后的效果：
<img src="https://i.loli.net/2018/11/14/5bebc0ba431c0.png" alt="2.png" title="2.png" style="zoom:50%"/>
&emsp;&emsp;之前的没加法向量的效果：
<img src="https://i.loli.net/2018/11/14/5bebc0ba41223.png" alt="3.png" title="3.png" style="zoom:50%"/>
&emsp;&emsp;然后拿这些法向量来算，发现压根算不出接触点来。到底是什么问题呢？用Debug.DrawLine图形化显示一下（只能在scene中看到线），发现根本不是我想的那样。
```
Debug.DrawLine(c, c+n/10,Color.red,10000);
```
&emsp;&emsp;顶点和顶点法向量：
<img src="https://i.loli.net/2018/11/14/5bebcb66a0a29.png" alt="4.png" title="4.png" style="zoom:50%"/>
&emsp;&emsp;三顶点相加/3和顶点法向量相加/3：
<img src="https://i.loli.net/2018/11/14/5bebcb667659a.png" alt="5.png" title="5.png" style="zoom:50%"/>
&emsp;&emsp;貌似这个法向量是只影响光照的，和几何半毛钱关系没有，所以只能用叉积来算了。
&emsp;&emsp;用叉积算完还是不对，再图形化看看，发现有的三角面片竟然有两个朝向的法向量，简直无语。
<img src="https://i.loli.net/2018/11/14/5bebd195c3a02.png" alt="6.png" title="6.png" />
&emsp;&emsp;看了一下obj模型，竟然还有这样的三角面片，明明是同一个但是顺序相反，真是坑爹。
<img src="https://i.loli.net/2018/11/14/5bebd1947bcdf.png" alt="7.png" title="7.png" />
&emsp;&emsp;所以只能再结合一下光照法向量了，毕竟他们都是朝外的。
&emsp;&emsp;实验发现他们并不都是朝外的，GG！
&emsp;&emsp;而且回想一下发现这个方法只对凸的物体有用，那些不规则的物体是不能用这种方法的。
&emsp;&emsp;为何不用射线法呢？具体怎么实现明天再弄吧。