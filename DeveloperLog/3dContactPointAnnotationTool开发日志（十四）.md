&emsp;&emsp;貌似每次让用户手动输入文件路径太不人道了，于是参考[Unity 实用教程 之 调用系统窗口选择文件或路径](https://jingyan.baidu.com/article/380abd0a265fb71d90192c83.html)增加了让用户浏览文件的功能，点击输入框旁边的+就可以找到文件并加载进来：
<img src="https://i.loli.net/2018/11/21/5bf4f3a37b4fb.png" alt="1.png" title="1.png" style="zoom:80%"/>
&emsp;&emsp;貌似调整位置再计算接触点是假的，原因应该是我计算用的点的坐标知识mesh原来的点的坐标，并没有考虑缩放、位移和旋转这些因素：
<img src="https://i.loli.net/2018/11/21/5bf4f820d50a7.png" alt="2.png" title="2.png" style="zoom:80%"/>
&emsp;&emsp;要想将变换应用到mesh.vertices上，可以参考[Matrix4x4.TRS Creates a translation, rotation and scaling matrix](https://docs.unity3d.com/ScriptReference/Matrix4x4.TRS.html)
&emsp;&emsp;改了之后就正常了。
<img src="https://i.loli.net/2018/11/21/5bf50b6d3c4a7.png" alt="3.png" title="3.png" style="zoom:80%"/>
&emsp;&emsp;不过感觉求接触点还是不够准，因为有的包围盒可能会扁到四个点在一个平面上,判断三角形的点在包围盒内几乎是找不到这样的点的。
&emsp;&emsp;算法啥的之后再改，先添加个坐标轴吧，但是发现Unity的坐标轴是左手系，3dsmax里的是右手系，下载的坐标轴模型也是右手系。查了一下如何使模型镜面对称，发现只要让scale变成负数就行了。
&emsp;&emsp;然而在透视视图中让坐标轴远离原点会发现z坐标轴好像不是沿z方向的，这是视角的问题，贼尴尬。
<img src="https://i.loli.net/2018/11/21/5bf5568dcbcd6.png" alt="4.png" title="4.png" style="zoom:80%"/>
<img src="https://i.loli.net/2018/11/21/5bf5568e232ae.png" alt="5.png" title="5.png" style="zoom:80%"/>
&emsp;&emsp;只有放置在原点才是毫无违和感的，感觉这东西有点难弄，现在是直接放到MainCamera下当儿子的，但总感觉怪怪的。
<img src="https://i.loli.net/2018/11/21/5bf5568e23341.png" alt="6.png" title="6.png" style="zoom:80%"/>