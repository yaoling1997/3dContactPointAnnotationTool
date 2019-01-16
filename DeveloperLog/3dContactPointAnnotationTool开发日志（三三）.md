&emsp;&emsp;添加背景图片后发现Runtime Transform Gizmo无法选中物体了:
<img src="https://i.loli.net/2019/01/16/5c3e95ae911cf.png" alt="1.png" title="1.png" style="zoom:80%"/>
&emsp;&emsp;于是改了一下EditorObjectSelection.cs中的WereAnyUIElementsHovered函数：
```
private bool WereAnyUIElementsHovered()
{
    if (EventSystem.current == null) return false;

    Vector2 inputDevPos;
    if (!InputDevice.Instance.GetPosition(out inputDevPos)) return false;

    PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
    eventDataCurrentPosition.position = new Vector2(inputDevPos.x, inputDevPos.y);

    List<RaycastResult> results = new List<RaycastResult>();
    EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
    //added by me
    int cnt = results.Count;
    foreach(var item in results)
    {
        if (item.gameObject.layer.Equals(Macro.BACKGROUND))
            cnt--;
    }
    return cnt != 0;
    //
    //return results.Count != 0;
}
```
&emsp;&emsp;然后可以正常点击物体了：
<img src="https://i.loli.net/2019/01/16/5c3e95ad6791c.png" alt="2.png" title="2.png" style="zoom:80%"/>
