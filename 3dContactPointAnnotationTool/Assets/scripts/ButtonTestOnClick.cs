using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTestOnClick : MonoBehaviour {
    public SkinnedMeshRenderer smr;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void OnClick()
    {
        Mesh mesh= new Mesh();
        smr.BakeMesh(mesh);
        //mesh = smr.sharedMesh;
        foreach(var item in mesh.vertices)
        {
            var a=GameObject.CreatePrimitive(PrimitiveType.Sphere);
            a.transform.position = item;
            a.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
        }
    }
}
