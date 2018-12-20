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
                var skinnedMeshRenderer = go.AddComponent<SkinnedMeshRenderer>();

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

                skinnedMeshRenderer.sharedMesh = mesh;
                skinnedMeshRenderer.material = new Material(GameObject.Find("ObjManager").GetComponent<ObjManager>().shaderStandard);
                re.Add(go);
            }
            return re;
        }
    }
}