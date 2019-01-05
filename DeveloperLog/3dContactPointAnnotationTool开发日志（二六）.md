&emsp;&emsp;之前给老师看了看我的毕设，老师觉得操作太复杂了，要能像3ds max里那样可以拖动物体的轴进行平移，沿着显示的圆圈旋转以及缩放啥的。说白了就是在Unity3d的Game视图显示出Scene视图里的坐标轴等等。这些鬼操作要是自己动手实现估计能写个把月了，而且我也不知道咋写。
&emsp;&emsp;硬着头皮写了两天也就这幅挫样子：
<img src="https://i.loli.net/2019/01/05/5c30a9128e7e4.png" alt="4.png" title="4.png" style="zoom:80%"/>
&emsp;&emsp;不知道咋写没关系，可以上百度搜搜看看有没有现成的可以直接用的Unity包。费了九牛二虎之力在Unity3d的Asset Store里找到了一个看上去比较靠谱的，叫作runtime transform gizmo：
<img src="https://i.loli.net/2019/01/05/5c30a472a2408.png" alt="1.png" title="1.png" />
&emsp;&emsp;但是天下没有免费的午餐，要用这个可是得花30刀的！
<img src="https://i.loli.net/2019/01/05/5c30a481939b4.png" alt="2.png" title="2.png" />
&emsp;&emsp;不过天朝地大物博，这些正版的东西总是会有好心人免费分享的嘛，百度一下还真有，是在一个叫“蛮牛”的[网站](http://www.manew.com/thread-106716-1-1.html)上：
<img src="https://i.loli.net/2019/01/05/5c30a5f464987.png" alt="3.png" title="3.png" style="zoom:80%"/>
&emsp;&emsp;那个txt文本文档的内容就是百度网盘链接和提取码，想要知道的话还得注册"蛮牛"帐号并做上面的任务赚10个蛮牛币就能获得链接和提取码了，为了不断别人财路我还是点到为止吧。
&emsp;&emsp;将那个包导入到自己的项目中就能实现运行模式下平移旋转缩放物体了，而且它还自带相机的一些操作，什么zoom,orbit,focus到某个选中对象啥的，用起来超方便。
&emsp;&emsp;不过那些都是通过它文档里给的快捷键操作的，想通过按钮来操作还得改他的代码并自己创建按钮。
&emsp;&emsp;搞了半天效果如下：
<img src="https://i.loli.net/2019/01/05/5c30a7ea7ada0.png" alt="5.png" title="5.png" style="zoom:80%"/>
&emsp;&emsp;这样game视图里就有那些乱起八糟的轴和球来操控模型变形了，另外右上角那个跟unity里长得一模一样的坐标轴也是它自带的，超炫酷！