using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShowItemController : MonoBehaviour {
    private float rotateAnglePerSec=30;//每秒旋转角度
    private Vector3 center;//物体的中心
    private void Awake()
    {
        center = Vector3.zero;
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(center, Vector3.up, rotateAnglePerSec*Time.deltaTime);        
	}
    public void UpdateCamera(GameObject o)
    {
        if (o == null)
            return;        
        var smrs = o.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (smrs == null)
            return;
        var vList = new List<Vector3>();
        foreach (var smr in smrs) {
            vList.Add(smr.sharedMesh.bounds.max);
            vList.Add(smr.sharedMesh.bounds.min);
        }
        var bigBound = ObjManager.GetBoundsOfVector3Array(vList.ToArray());
        center = bigBound.center;
        var r = (bigBound.max - bigBound.min).magnitude / 2;
        transform.position = new Vector3(0, bigBound.center.y , 2*r);
        transform.forward = center - transform.position;
    }
}
