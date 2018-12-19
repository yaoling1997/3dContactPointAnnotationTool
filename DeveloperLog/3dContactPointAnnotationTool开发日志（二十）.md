&emsp;&emsp;为了使工具更人性化，我又在每个status的text上绑了个可以拖拽实现值改变的脚本，但是不知道为啥rotx那个值越过+-90范围后连续修改就会产生抖动的现象，试了很多方法也没能弄好，不过实际用起来问题应该不大。
&emsp;&emsp;不过拖拽时鼠标碰到屏幕边缘就动不了了，于是我想像unity那样可以让鼠标出现在屏幕的另一边，这样就拖拽起来就更加的方便。
&emsp;&emsp;关于如何实现该功能请看完下列教程：
&emsp;&emsp;1.[unity中锁定鼠标移动&&隐藏鼠标&&强制是鼠标移动到某一位置](https://www.cnblogs.com/GouBin/p/5853644.html)
&emsp;&emsp;2.[C# 鼠标光标到达屏幕边缘后从另一边缘出现](https://blog.csdn.net/e295166319/article/details/80939714)
&emsp;&emsp;3.[在unity中添加System.Windows.Forms引用和注意事项](https://blog.csdn.net/zxy13826134783/article/details/79660950)，其中第三部修改Edit是在Unity里修改，不要像我一样在Visual Studio里弄半天都找不到 Project Settings在哪T_T...
&emsp;&emsp;做完上面这几步后可能还会有这样的错误： error CS0012: The type \`System.Drawing.Rectangle' is defined in an assembly that is not referenced. Consider adding a reference to assembly \`System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
<img src="https://i.loli.net/2018/12/19/5c1a0206218ed.png" alt="1.png" title="1.png" style="zoom:80%"/>
&emsp;&emsp;像教程3那样在该文件夹下找到System.Drawing.dll文件并拖到plugins文件夹下即可。
<img src="https://i.loli.net/2018/12/19/5c1a02074433f.png" alt="2.png" title="2.png" style="zoom:80%"/>
&emsp;&emsp;效果如下：
<img src="https://i.loli.net/2018/12/19/5c1a0361559d2.gif" alt="Unity 2018_clip.gif" title="Unity 2018_clip.gif" />
&emsp;&emsp;不过可能很少有人会发现这个功能，所以我又加了按钮可以让用户通过点击按钮来改变值，不过必须点击一次才能改变一下所以显得很蠢。长按按钮来改变的方式貌似有点难，不会。
<img src="https://i.loli.net/2018/12/19/5c1a39ef10e80.png" alt="3.png" title="3.png" style="zoom:80%"/>
&emsp;&emsp;刚刚闲来无事看unity关于eventSystem的视频教程的时候无意中看到了一个叫作Event Trigger的东西，利用它提供的PointerDown和PointerUp就能够轻松响应鼠标长按按钮事件：
<img src="https://i.loli.net/2018/12/19/5c1a47ce730be.png" alt="4.png" title="4.png" style="zoom:80%"/>
&emsp;&emsp;给要实现长按功能的组件添加一个EventTrigger，添加这两个Event然后把组件丢进去，调用组件绑定的自己写的脚本里的方法。我的脚本写的关键代码如下：
```
void Update () {
    if (ifPointerDown)//鼠标按在按钮上
    {
        float delta = speed;
        //Debug.Log("name:"+transform.name);
        if (transform.name.Equals("ButtonDec"))
            delta = -speed;
        if (inputField.interactable)
            inputField.text = (float.Parse(inputField.text) + delta).ToString();
    }
}
public void PointerDown()
{
    ifPointerDown = true;
}
public void PointerUp()
{
    ifPointerDown = false;
}
```
&emsp;&emsp;这样就能实现长按按钮改变输入框的值功能了！按钮也不一定得是按钮，可以是别的UI组件。
<img src="https://i.loli.net/2018/12/19/5c1a491cf2488.gif" alt="Unity 2018_clip.gif" title="Unity 2018_clip.gif" />