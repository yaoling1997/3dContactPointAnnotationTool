&emsp;&emsp;昨天是实现了显示GameObject子GameObject的选项卡功能，今天就是要让statusPanel可以控制它们的位置、旋转和缩放了。
&emsp;&emsp;没什么难的，对应选项卡绑定上对应的物体或子物体即可。
&emsp;&emsp;删除操作的话只需要在删除当前选项卡之前递归地删除它的子孙选项卡和对应的模型即可，隐藏的话也是递归地将他们的active设置为false。
&emsp;&emsp;由于smpl模型只有一个skinnedMeshRenderer，然而网格的各个部分是绑定在它的子对象中的，如何获得各种变换后对应网格的顶点坐标呢？
&emsp;&emsp;使用BakeMesh方法即可，用法是先new一个mesh作为容器，然后调用该方法就能把经过各种乱七八糟变换后的mesh的snapshot(快照，就是网格渲染的样子，也是Mesh类型)存入括号里的mesh。
&emsp;&emsp;比如我的OnClick方法是这么写的，就是点击按钮后会在网格的每个顶点坐标生成一个半径为0.1的球：
```
public void OnClick()
{
    Mesh mesh= new Mesh();
    smr.BakeMesh(mesh);
    foreach(var item in mesh.vertices)
    {
        var a=GameObject.CreatePrimitive(PrimitiveType.Sphere);
        a.transform.position = item;
        a.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
    }
}

```
&emsp;&emsp;变换前：
<img src="https://i.loli.net/2018/12/21/5c1c9f9759329.png" alt="1.png" title="1.png" style="zoom:80%"/>
&emsp;&emsp;只旋转子对象：
<img src="https://i.loli.net/2018/12/21/5c1c9f977130d.png" alt="2.png" title="2.png" style="zoom:80%"/>
&emsp;&emsp;点击按钮：
<img src="https://i.loli.net/2018/12/21/5c1c9f9772b6c.png" alt="3.png" title="3.png" style="zoom:80%"/>
&emsp;&emsp;不过对最外层的变换貌似只响应scale变换，position和rotation的变换却没反应，但是对于直接导入的obj模型是这三类属性都不响应的，真奇怪。
&emsp;&emsp;没办法，只能禁用最外层的scale编辑了...
&emsp;&emsp;smpl模型也不能增加三角形数量，于是也把smpl模型三角形数量编辑给禁了。
&emsp;&emsp;关节点的话只能编辑旋转，编辑position的话会产生很鬼畜的结果，所以把关节点的position和scale也禁了。总之哪里不对禁哪里。
<img src="https://i.loli.net/2018/12/21/5c1cc54dec03a.png" alt="4.png" title="4.png" style="zoom:80%"/>
&emsp;&emsp;也相应地把计算接触点那里改成适应skinnedMeshRenderer的版本，来看看效果吧：
&emsp;&emsp;先调好人的姿势以及和椅子的位置：
<img src="https://i.loli.net/2018/12/21/5c1cc85c6c2f8.png" alt="5.png" title="5.png" style="zoom:80%"/>
<img src="https://i.loli.net/2018/12/21/5c1cc85bd4b5d.png" alt="6.png" title="6.png" style="zoom:80%"/>
&emsp;&emsp;然后计算接触点：
<img src="https://i.loli.net/2018/12/21/5c1cc85ce5420.png" alt="7.png" title="7.png" style="zoom:80%"/>
<img src="https://i.loli.net/2018/12/21/5c1cc85ce5c28.png" alt="8.png" title="8.png" style="zoom:80%"/>
&emsp;&emsp;看上去还挺正常的。不过接触点貌似太黑了点，要是红红的就好看一些，应该是阴影的问题。
&emsp;&emsp;于是加了这两行代码：
```
//不产生阴影也不接收阴影
mr.receiveShadows = false;
mr.shadowCastingMode =UnityEngine.Rendering.ShadowCastingMode.Off;
```
&emsp;&emsp;然而还是有些接触点是黑色的，这些都是卡在另一个模型里没有光线照射到的球：
<img src="https://i.loli.net/2018/12/21/5c1ccc10e021a.png" alt="9.png" title="9.png" style="zoom:80%"/>
&emsp;&emsp;应该能够用设置layer的方式来解决这个问题，给接触点设置为单独的层，然后新建light再设置light的culling mask，只照射接触点所在层就行了：
<img src="https://i.loli.net/2018/12/21/5c1cd076c4cf8.png" alt="10.png" title="10.png" style="zoom:80%"/>
&emsp;&emsp;最后用一个哲♂学场景测一下效果：
<img src="https://i.loli.net/2018/12/21/5c1cd0dd555ae.png" alt="11.png" title="11.png" style="zoom:80%"/>
<img src="https://i.loli.net/2018/12/21/5c1cd0dd64c82.png" alt="12.png" title="12.png" style="zoom:80%"/>
<img src="https://i.loli.net/2018/12/21/5c1cd18a4bbf1.png" alt="13.png" title="13.png" style="zoom:80%"/>
<img src="https://i.loli.net/2018/12/21/5c1cd18a4ebeb.png" alt="14.png" title="14.png" style="zoom:80%"/>
<img src="https://i.loli.net/2018/12/21/5c1cd1bc76de8.png" alt="15.png" title="15.png" />
&emsp;&emsp;可以发现所有接触点在光线的照射下熠熠生辉，我成功地驱逐了黑暗！