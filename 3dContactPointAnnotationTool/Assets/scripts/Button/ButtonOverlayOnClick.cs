using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonOverlayOnClick : MonoBehaviour {
    public GameObject contactPoints;
    public Camera mainCamera;
    public GameObject image;
    public GameObject sphere2d;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void ClearOld2DContactPoints()//清除图片上原有了2d接触点(清除image的所有儿子)
    {
        foreach (var item in image.GetComponentsInChildren<Transform>())
        {
            if (item.name.Equals(image.name))
                continue;
            Destroy(item.gameObject);
        }
    }
    public void OnClick()
    {
        GetComponent<Button>().interactable = false;//处理的时候不能再次点击
        ClearOld2DContactPoints();
        int cnt = 0;
        var rectTransform = image.transform as RectTransform;
        foreach (var item in contactPoints.GetComponentsInChildren<Transform>())
        {
            if (item.name.Equals(contactPoints.name))
                continue;
            cnt++;
            Debug.Log(cnt);
            var p = item.position;
            var q = item.localScale;
            Debug.Log("p: "+p);
            var sp = mainCamera.WorldToScreenPoint(p);
            var radius = mainCamera.WorldToScreenPoint(new Vector3(p.x+q.x,p.y,p.z)).x-sp.x;
            Debug.Log("radius: " + radius);
            Debug.Log("sp: "+sp);
            Debug.Log("rectTransform: " + rectTransform.position);
            var lp = new Vector2();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, sp, null, out lp);
            Debug.Log("lp: " + lp);
            var cp2 = Instantiate(sphere2d);
            cp2.transform.SetParent(image.transform);
            var rectTransformCp2 = cp2.GetComponent<RectTransform>();//2维接触点的rectTransform
            rectTransformCp2.sizeDelta = new Vector2(2*radius,2*radius);
            rectTransformCp2.localPosition = new Vector2(lp.x+radius,lp.y+radius);
        }
        GetComponent<Button>().interactable = true;
    }
}
