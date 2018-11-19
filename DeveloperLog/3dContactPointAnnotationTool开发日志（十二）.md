&emsp;&emsp;因为ReferenceImage的锚点是固定的左下角，缩放时controller面板也会跟着动。为了使Scale的时候controller上的slider不会远离指针，于是把controller固定到了左下角。
<img src="https://i.loli.net/2018/11/19/5bf24e0c883ea.png" alt="1.png" title="1.png" style="zoom:50%"/>
&emsp;&emsp;在网上找了半天也没有找到好的在图像上画球的方法，没办法只好用image来当求了，放上球的照片即可。得把三维坐标转到UI坐标里，具体方法请看[Unity3D 世界坐标上一个点对应UI上一个点](https://blog.csdn.net/aa20274270/article/details/52529075?locationNum=3&fps=1)。
&emsp;&emsp;于是具体的玩法就是先计算接触点，再把图片和三维物体对齐，点击Overlay按钮，就会生成以image为父亲的带有球图片的小image，效果如下：
<img src="https://i.loli.net/2018/11/19/5bf272456f6dc.png" alt="2.png" title="2.png" style="zoom:50%"/>
<img src="https://i.loli.net/2018/11/19/5bf272456b865.png" alt="4.png" title="4.png" style="zoom:50%"/>
<img src="https://i.loli.net/2018/11/19/5bf272456d883.png" alt="5.png" title="5.png" style="zoom:50%"/>
<img src="https://i.loli.net/2018/11/19/5bf27245191a8.png" alt="3.png" title="3.png" style="zoom:50%"/>
&emsp;&emsp;想发布到windows平台试运行一下，结果给我报了这么多错误，说什么找不到PrefabUtility：
<img src="https://i.loli.net/2018/11/19/5bf295d638fd2.png" alt="6.png" title="6.png" style="zoom:50%"/>
&emsp;&emsp;之后百度发现UnityEditor是只能在编辑模式下调用的库，于是把之前的代码：
```
var scrollViewItem= UnityEditor.PrefabUtility.InstantiatePrefab(prefabScrollViewItem) as GameObject;
```
改成了
```
var scrollViewItem = Instantiate(prefabScrollViewItem,new Vector3(0,0,0),Quaternion.identity);
```
就可以过编译了。
<img src="https://i.loli.net/2018/11/19/5bf295d5d1897.png" alt="7.png" title="7.png" style="zoom:50%"/>
&emsp;&emsp;然而实际运行却是这幅鬼样，不知道问题出在哪里。Editor里运行是正常的，发布出来的版本运行却是不正常的，看来Unity也没有自己想象中的那么好用。
<img src="https://i.loli.net/2018/11/19/5bf295d638bf3.png" alt="8.png" title="8.png" style="zoom:50%"/>