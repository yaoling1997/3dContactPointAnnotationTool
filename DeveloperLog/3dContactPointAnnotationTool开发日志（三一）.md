&emsp;&emsp;在玩的时候遇到了一个python的问题：
```
Traceback (most recent call last):
  File ".\convert.py", line 13, in <module>
    a=pickle.load(readFile)
ImportError: No module named multiarray
```
<img src="https://i.loli.net/2019/01/14/5c3bf047226cd.png" alt="1.png" title="1.png" />
如何解决请看[python “No module named multiarray ”的解决方法](https://blog.csdn.net/pulci/article/details/52175451)
&emsp;&emsp;改了一下它的代码，把它自带的渲染那部分代码给去掉了，因为跑不通。生成的.pkl文件里有相机位置、焦距、姿势参数(72个)和形状参数(10个)，然后用python版的smpl模型试了下：
<img src="https://i.loli.net/2019/01/14/5c3c772b1f89b.jpg" alt="im0001.jpg" title="im0001.jpg" />
&emsp;&emsp;对应的smpl_python版生成的模型：
<img src="https://i.loli.net/2019/01/14/5c3c7b32ad16a.png" alt="2.png" title="2.png" />
<img src="https://i.loli.net/2019/01/14/5c3c7b32af073.png" alt="3.png" title="3.png" />
&emsp;&emsp;可以看到也只是看起来姿势比较像但还是不够准确的。
&emsp;&emsp;python版的不能直接在unity里调姿势，只能生成一个固定的obj文件，怎么把姿势参数用到unity版本的smpl模型上我还没弄明白。
