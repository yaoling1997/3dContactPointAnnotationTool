using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Hont
{
    public static class ObjFormatAnalyzerFactory
    {
        public static List<GameObject> AnalyzeToGameObject(string objFilePath,bool useGlobalMaterial)
        {
            if (!File.Exists(objFilePath)) return null;

            var objFormatAnalyzer = new ObjFormatAnalyzer();

            objFormatAnalyzer.Analyze(File.ReadAllText(objFilePath));
            int length = objFormatAnalyzer.ObjFaceBeginArr.Length;//有多少个3d模型
            var re = new List<GameObject>();
            var sourceVertexArr = objFormatAnalyzer.VertexArr;
            var sourceVertexNormalArr = objFormatAnalyzer.VertexNormalArr;
            var sourceUVArr = objFormatAnalyzer.VertexTextureArr;
            var faceArr = objFormatAnalyzer.FaceArr;
            Debug.Log("vertex num:"+ sourceVertexArr.Length);
            Debug.Log("face num:" + faceArr.Length);
            Debug.Log("length:" + length);
            for (int objId = 0; objId < length; objId++)
            {
                var go = new GameObject();
                go.name = objFormatAnalyzer.ObjNameArr[objId];
                var skinnedMeshRenderer = go.AddComponent<SkinnedMeshRenderer>();//添加skinnedMeshRenderer

                var mesh = new Mesh();

                var vertexList = new List<Vector3>();
                var vertexNormalList = new List<Vector3>();
                var uvList = new List<Vector2>();

                int faceBeginId = objFormatAnalyzer.ObjFaceBeginArr[objId];
                int faceEndId = faceArr.Length;//左闭右开
                if (objId < length - 1)
                    faceEndId = objFormatAnalyzer.ObjFaceBeginArr[objId + 1];
                Debug.Log("faceBeginId: "+faceBeginId);
                Debug.Log("faceEndId: " + faceEndId);
                int triangleNum = 0;
                for (int i = faceBeginId; i < faceEndId; i++)
                    if (faceArr[i].IsQuad)
                        triangleNum += 6;
                    else triangleNum += 3;
                var triangles = new int[triangleNum];
                int nNum = 0;
                int tNum = 0;
                for (int i = faceBeginId, j = 0; i < faceEndId; i++)
                {
                    var currentFace = faceArr[i];
                    var defaultVec = default(ObjFormatAnalyzer.Vector);
                    triangles[j] = j;
                    triangles[j + 1] = j + 1;
                    triangles[j + 2] = j + 2;
                    for (int k = 0; k < 3; k++)//0,1,2是一个三角形
                    {
                        var vId = currentFace.Points[k].VertexIndex - 1;
                        var nId = currentFace.Points[k].NormalIndex - 1;
                        var tId = currentFace.Points[k].TextureIndex - 1;
                        var vec = sourceVertexArr[vId];

                        vertexList.Add(new Vector3(vec.X, vec.Y, vec.Z));
                        vec = defaultVec;
                        if (nId != -1)
                        {
                            vec = sourceVertexNormalArr[nId];
                            nNum++;
                        }
                        vertexNormalList.Add(new Vector3(vec.X, vec.Y, vec.Z));
                        var uv = defaultVec;
                        if (tId != -1)
                        {
                            uv = sourceUVArr[tId];
                            tNum++;
                        }                        
                        uvList.Add(new Vector2(uv.X, uv.Y));
                    }

                    if (currentFace.IsQuad)
                    {
                        triangles[j + 3] = j + 3;
                        triangles[j + 4] = j + 4;
                        triangles[j + 5] = j + 5;
                        j += 3;
                        for (int k = 0; k < 4; k++)//0,2,3是一个三角形
                        {
                            if (k == 1)
                                continue;
                            var vId = currentFace.Points[k].VertexIndex - 1;
                            var nId = currentFace.Points[k].NormalIndex - 1;
                            var tId = currentFace.Points[k].TextureIndex - 1;
                            var vec = sourceVertexArr[vId];
                            vertexList.Add(new Vector3(vec.X, vec.Y, vec.Z));
                            vec = nId < sourceVertexNormalArr.Length ? sourceVertexNormalArr[nId] : defaultVec;
                            vertexNormalList.Add(new Vector3(vec.X, vec.Y, vec.Z));
                            var uv = tId < sourceUVArr.Length ? sourceUVArr[tId] : defaultVec;
                            uvList.Add(new Vector2(uv.X, uv.Y));
                        }
                    }

                    j += 3;
                }

                mesh.vertices = vertexList.ToArray();
                if (nNum == 0)
                {
                    mesh.normals = null;
                }
                else {
                    mesh.normals = vertexNormalList.ToArray();
                }                
                if (tNum == 0)
                {
                    mesh.uv = null;
                }
                else {
                    mesh.uv = uvList.ToArray();
                }
                Debug.Log("mesh.normals.length: " + mesh.normals.Length);
                Debug.Log("mesh.uv.length: "+mesh.uv.Length);
                //mesh.normals= vertexNormalList.ToArray();
                //mesh.uv = uvList.ToArray();
                mesh.triangles = triangles;

                skinnedMeshRenderer.sharedMesh = mesh;
                if (useGlobalMaterial)
                    skinnedMeshRenderer.material = new Material(GameObject.Find("ObjManager").GetComponent<ObjManager>().panelModel_PointsInformationController.GetMaterial());
                else//使用标准material
                    skinnedMeshRenderer.material = new Material(GameObject.Find("ObjManager").GetComponent<ObjManager>().materialStandard);
                re.Add(go);
            }
            return re;
        }
    }
}