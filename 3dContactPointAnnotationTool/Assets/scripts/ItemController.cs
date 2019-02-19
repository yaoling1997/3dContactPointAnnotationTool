using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour {
    public enum ModelType
    {//关联model所属类型
        MAIN_CAMERA,
        OBJ_MODEL,
        SMPL_MODEL,
        SMPL_MODEL_JOINT,
        CONTACT_POINT
    }
    public ModelType modelType;// 模型的类型
    public bool trianglesEditable;//是否可以从面板编辑triangles
    public bool positionEditable;//是否可以从面板编辑position
    public bool rotationEditable;//是否可以从面板编辑rotation
    public bool scaleEditable;//是否可以从面板编辑scale
    public bool canDelete;//是否可以删除
    public string path;//相关文件路径(obj文件路径)
    void Awake(){
        modelType = ModelType.OBJ_MODEL;
        trianglesEditable = true;
        positionEditable = true;
        rotationEditable = true;
        scaleEditable = true;
        canDelete = true;
        path = null;
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public ItemController SetModelType(ModelType modelType) {
        this.modelType = modelType;
        trianglesEditable = true;
        positionEditable = true;
        rotationEditable = true;
        scaleEditable = true;
        canDelete = true;
        switch (modelType)
        {
            case ModelType.MAIN_CAMERA:
                trianglesEditable = false;
                scaleEditable = false;
                canDelete = false;//主相机不能删除
                break;
            case ModelType.OBJ_MODEL:
                break;
            case ModelType.SMPL_MODEL:
                trianglesEditable = false;
                scaleEditable = false;
                break;
            case ModelType.SMPL_MODEL_JOINT:
                trianglesEditable = false;
                positionEditable = false;
                scaleEditable = false;
                canDelete = false;//smpl模型的关节点不能删除
                break;
            case ModelType.CONTACT_POINT:
                trianglesEditable = false;
                break;
        }
        return this;
    }
    public void CloneAttributes(ItemController ic) {//clone 属性
        modelType = ic.modelType;
        trianglesEditable = ic.trianglesEditable;
        positionEditable = ic.positionEditable;
        rotationEditable = ic.rotationEditable;
        scaleEditable = ic.scaleEditable;
        canDelete = ic.canDelete;
    }
}
