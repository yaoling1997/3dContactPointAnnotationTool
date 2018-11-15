&emsp;&emsp;今天看的时候发现其实www的方式是可以根据指定路径读取本地图片到Image中的。也就是昨天提到的第二种方式。
&emsp;&emsp;随便选了个图片做示范：
<img src="https://i.loli.net/2018/11/07/5be2f8f984001.jpg" alt="2.jpg" title="2.jpg" style="zoom:50%"/>
<img src="https://i.loli.net/2018/11/07/5be2f8f98414d.png" alt="1.png" title="1.png" style="zoom:50%"/>
修改后的代码如下：
```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonOkOnClick : MonoBehaviour {
    public InputField imagePath;
    public Image referenceImage;
    
    public void Click() {
        Debug.Log("onClick");
        StartCoroutine(GetImage(imagePath.text));
    }
    private IEnumerator GetImage(string path) {
        Debug.Log(path);
        Debug.Log(path.Replace('\\', '/'));
        //WWW www = new WWW("file://"+path.Replace('\\','/'));

        WWW www = new WWW("file://" + "C:/Users/A/Desktop/2.jpg");
        Debug.Log(www.url);
        yield return www;
        if (www != null && string.IsNullOrEmpty(www.error))
        {
            Texture2D texture = new Texture2D(www.texture.width, www.texture.height);
            texture.SetPixels(www.texture.GetPixels());
            texture.Apply(true);
            texture.filterMode = FilterMode.Trilinear;
            referenceImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
        else {
            Debug.Log("no such image!");
        }
    }
}

```