&emsp;&emsp;一种可行的思路就是枚举一个模型的三角面片，然后判断三角形是否与另一个物体相交即可。为了让效果更好我想只渲染模型的线框。
&emsp;&emsp;在网上查了半天好像Unity里都没有自带的方便的渲染线框的方式，我又自己玩了玩发现可以将材质换成unity自带的VR/SpatialMapping/Wireframe。不过看上去模型不是镂空的，只是表面显示了三角面片，要是只有线框就更好了。
<img src="https://i.loli.net/2018/11/13/5bea75e10c92f.png" alt="1.png" title="1.png" style="zoom:50%"/>
&emsp;&emsp;又回过头看了看，发现[Unity用shader显示物体线框](https://blog.csdn.net/chenggong2dm/article/details/47662633)提到的方法还挺好的，就是下载一个[UCLA Wireframe Shader](https://assetstore.unity.com/packages/vfx/shaders/directx-11/ucla-wireframe-shader-21897)资源然后导入到项目中。
<img src="https://i.loli.net/2018/11/13/5bea75e860207.png" alt="2.png" title="2.png" style="zoom:50%"/>
&emsp;&emsp;将它提供的wireframe材质拖到模型上，效果还挺不错的。
<img src="https://i.loli.net/2018/11/13/5bea75e6db20e.png" alt="3.png" title="3.png" style="zoom:50%"/>
&emsp;&emsp;颜色也可以轻松改变，超赞。
<img src="https://i.loli.net/2018/11/13/5bea76d8a15e3.png" alt="4.png" title="4.png" style="zoom:50%"/>
&emsp;&emsp;想在代码里调用就这么写：
```
meshRenderer.material = new Material(Shader.Find("UCLA Game Lab/Wireframe/Single-Sided"));
```
&emsp;&emsp;路径名和下图Shader中的一致就行。
<img src="https://i.loli.net/2018/11/13/5bea79fb88490.png" alt="5.png" title="5.png" style="zoom:50%"/>

&emsp;&emsp;添加了一个toggle，命名为wireframe，实现了模型网格材质与默认材质间的切换。
<img src="https://i.loli.net/2018/11/13/5bea836cb04f7.png" alt="6.png" title="6.png" style="zoom:50%"/>
&emsp;&emsp;tag必须先定义后使用。不然会报错。
<img src="https://i.loli.net/2018/11/15/5bed1c0424b41.png" alt="7.png" title="7.png" style="zoom:50%"/>
&emsp;&emsp;这个是压根没有这个组件，然后获取该组件会报错。虽然它讲的不是很明确，但实际上就是这样。

<img src="https://i.loli.net/2018/11/15/5bed1c05106c6.png" alt="8.png" title="8.png" style="zoom:50%"/>
&emsp;&emsp;试着求了求接触点，通过判断一个模型每个三角面片的点是否在另一个模型里，在的话就画一个球。不过这球都画脑袋上去了，不知道是什么bug。

<img src="https://i.loli.net/2018/11/15/5bed1c099edec.png" alt="9.png" title="9.png" style="zoom:50%"/>