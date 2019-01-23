using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour {
    public bool trianglesEditable;//是否可以从面板编辑triangles
    public bool positionEditable;//是否可以从面板编辑position
    public bool rotationEditable;//是否可以从面板编辑rotation
    public bool scaleEditable;//是否可以从面板编辑scale
    public bool canDelete;//是否可以删除
    void Awake(){
        trianglesEditable = true;
        positionEditable = true;
        rotationEditable = true;
        scaleEditable = true;
        canDelete = true;
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
