&emsp;&emsp;今天的任务是实现选中接触点功能并添加模型或接触点的属性改变功能，先从最简单的位置x,y,z改变入手，于是创建了一个面板(PanelStatus)，添加了几个InputField来让用户输入数值改变选中物体的位置：
<img src="https://i.loli.net/2018/11/16/5beeb6b906471.png" alt="1.png" title="1.png" style="zoom:80%"/>
&emsp;&emsp;搞了半天终于实现了选中接触点功能，选中的球是青色，未选中的是红色，为了尽量重用以前的代码，我直接在之前的脚本上做了修改，效果如下：
<img src="https://i.loli.net/2018/11/16/5beeb6c32cada.png" alt="2.png" title="2.png" style="zoom:80%"/>
&emsp;&emsp;然后又实现了选中物体显示和修改坐标信息的功能：
<img src="https://i.loli.net/2018/11/16/5beeb6c158da0.png" alt="3.png" title="3.png" style="zoom:80%"/>
<img src="https://i.loli.net/2018/11/16/5beeb6c18a110.png" alt="4.png" title="4.png" style="zoom:80%"/>
&emsp;&emsp;还挺好玩的，接触点的位置也能修改。
<img src="https://i.loli.net/2018/11/16/5beeb6c335311.png" alt="5.png" title="5.png" style="zoom:80%"/>
<img src="https://i.loli.net/2018/11/16/5beeb6c34abc0.png" alt="6.png" title="6.png" style="zoom:80%"/>
&emsp;&emsp;光修改位置还不够，于是又增加了修改旋转角度和缩放大小的功能：
<img src="https://i.loli.net/2018/11/16/5beeb6c1cc753.png" alt="7.png" title="7.png" style="zoom:80%"/>
&emsp;&emsp;如果InputField的text儿子的高度太小了是不会显示字的，text的高度一般是小于InputField高度的，可以修改Top和Bottom参数，也就是离上边界和下边界的距离。
<img src="https://i.loli.net/2018/11/16/5beeba02d7abf.png" alt="8.png" title="8.png" style="zoom:80%"/>
&emsp;&emsp;那个clearAll按钮就是点击后回到初始状态，一切从头开始，每次点击执行下面的代码就行了：
```
public void OnClick()
{
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}

```