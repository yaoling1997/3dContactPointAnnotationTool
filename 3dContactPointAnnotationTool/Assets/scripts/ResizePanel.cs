using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResizePanel : MonoBehaviour, IPointerDownHandler, IDragHandler {

    public int minSize;
	
	private RectTransform panelRectTransform;
	private Vector2 originalLocalPointerPosition;
	private Vector2 originalSizeDelta;
    private ObjManager objManager;
    private Camera mainCamera;
    private Camera cameraBackground;

    void Awake () {
		panelRectTransform = transform.parent.GetComponent<RectTransform> ();
        objManager = GameObject.Find("ObjManager").GetComponent<ObjManager>();
        mainCamera = objManager.mainCamera;
        cameraBackground = objManager.cameraBackground;
        UpdateCameraViewport();
    }
	
	public void OnPointerDown (PointerEventData data) {
		originalSizeDelta = panelRectTransform.sizeDelta;
		RectTransformUtility.ScreenPointToLocalPointInRectangle (panelRectTransform, data.position, data.pressEventCamera, out originalLocalPointerPosition);
	}
	public void UpdateCameraViewport()//更新camera的视口
    {
        var sizeDelta = panelRectTransform.sizeDelta;
        var tmp = sizeDelta.x / Screen.width;
        var rect = mainCamera.rect;
        rect.width = 1 - tmp;
        mainCamera.rect = rect;
        cameraBackground.rect = rect;
    }
    public void OnDrag (PointerEventData data) {
		if (panelRectTransform == null)
			return;
		
		Vector2 localPointerPosition;
		RectTransformUtility.ScreenPointToLocalPointInRectangle (panelRectTransform, data.position, data.pressEventCamera, out localPointerPosition);
		Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;
		
		Vector2 sizeDelta = originalSizeDelta + new Vector2 (-offsetToOriginal.x,0);
        var maxSize = Mathf.Max(minSize, Screen.width - 200);
        sizeDelta = new Vector2 (
			Mathf.Clamp (sizeDelta.x, minSize, maxSize),
			sizeDelta.y
		);		
		panelRectTransform.sizeDelta = sizeDelta;
        UpdateCameraViewport();
    }
}