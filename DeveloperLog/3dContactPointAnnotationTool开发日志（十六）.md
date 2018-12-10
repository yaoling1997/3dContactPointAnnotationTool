&emsp;&emsp;调了一上午才发现是把下面这个函数：
```
private float DivideTriangle(int []triangle,out int []outTriangle,List<Vector3> vertices)//切割三角形
{
    float re = -1;
    int p = 0;
    for (int i = 0; i < 3; i++)
    {
        int a = triangle[i];
        int b = triangle[(i + 1) % 3];
        var len = (vertices[b] - vertices[a]).magnitude;
        if ( len > re)
        {
            p = i;
            re = len;
        }
    }
    outTriangle = new int[] { triangle[p],triangle[(p+1)%3],triangle[3-p- (p + 1) % 3] };//最长边的两个点以及该边所对的点的标号
    return re;
}
```
&emsp;&emsp;写成了：
```
private float DivideTriangle(int []triangle,out int []outTriangle,List<Vector3> vertices)//切割三角形
{
    float re = -1;
    int p = 0;
    for (int i = 0; i < 3; i++)
    {
        int a = triangle[i];
        int b = triangle[(i + 1) % 3];
        var len = (vertices[b] - vertices[a]).magnitude;
        if ( len > re)
        {
            p = i;
            re = len;
        }
    }
    outTriangle = new int[] { p,(p+1)%3,3-p- (p + 1) % 3 };//最长边的两个点以及该边所对的点的标号
    return re;
}
```
&emsp;&emsp;简直被自己蠢哭。不过改了之后还是不太对，三角形变多了，但是模型被我弄得支离破碎，颇有艺术美感...
<img src="https://i.loli.net/2018/11/23/5bf76be0a48a7.png" alt="1.png" title="1.png" style="zoom:80%"/>
<img src="https://i.loli.net/2018/11/23/5bf76be0a475f.png" alt="2.png" title="2.png" style="zoom:80%"/>
&emsp;&emsp;看了半天都没发现到底哪里有问题，直到我把
```
var t1 = new int[] { outTriangle[0], outTriangle[2], tot };
var t2 = new int[] { outTriangle[1], outTriangle[2], tot };
```
&emsp;&emsp;改成了
```
var t1 = new int[] { outTriangle[0], tot, outTriangle[2] };
var t2 = new int[] { outTriangle[1], outTriangle[2], tot};
```
&emsp;&emsp;才变正常了。仔细观察发现貌似三角形顶点的顺序不能瞎给，得统一是逆时针的才行，下面那一种就是两个三角形的顶点顺序都是逆时针的，tot是新加的顶点的标号，[2]是最长边所对点。
&emsp;&emsp;现在终于正常了：
<img src="https://i.loli.net/2018/11/23/5bf7999357a47.png" alt="2t.png" title="2t.png" style="zoom:80%"/>
<img src="https://i.loli.net/2018/11/23/5bf7999357b3a.png" alt="3.png" title="3.png" style="zoom:80%"/>
<img src="https://i.loli.net/2018/11/23/5bf799935815f.png" alt="4.png" title="4.png" style="zoom:80%"/>
&emsp;&emsp;在ContactPointsPanel里添加了一个clear按钮，一点就能删除所有接触点，方便又快捷！
<img src="https://i.loli.net/2018/11/23/5bf7a02ab1457.png" alt="5.png" title="5.png" style="zoom:80%"/>
<img src="https://i.loli.net/2018/11/23/5bf7a02aaf3ac.png" alt="6.png" title="6.png" style="zoom:80%"/>
&emsp;&emsp;然后来看看一倍和三倍的区别，先是一倍的，有14740个点：
<img src="https://i.loli.net/2018/11/23/5bf7a1376129c.png" alt="7.png" title="7.png" style="zoom:80%"/>
&emsp;&emsp;三倍的，稍微精确一点，只有11445个点，少了3000多个冗余点。
<img src="https://i.loli.net/2018/11/23/5bf7a1d7c00a6.png" alt="8.png" title="8.png" style="zoom:80%"/>
&emsp;&emsp;记录一下椅子和人的位置信息，以后也按这个数据测下：
人：
-0.25 -0.25 -0.1
90    0     0
1     1     1
椅子：
0     0     0
0     60    0
1     1     1
&emsp;&emsp;有时候计算接触点的时候不知道要等多久，所以弄个进度条再好不过了。
&emsp;&emsp;又调整了一下算法，先判断两个物体包围盒是否相交，再枚举一个物体所有三角面片包围盒，若不和另一个物体包围盒相交直接枚举下一个三角面片，否则和另一个物体所有三角面片包围盒求交。效率是稍微提高了点，不过为啥准确度也会提高就不得而知了，椅子只有一倍三角形的时候竟然只有8440多个接触点，三倍时只有4807个接触点，看上去少了很多冗余接触点的样子。
<img src="https://i.loli.net/2018/11/23/5bf7dd8bebe48.png" alt="9.png" title="9.png" style="zoom:80%"/>
<img src="https://i.loli.net/2018/11/23/5bf7dd8e03317.png" alt="10.png" title="10.png" style="zoom:80%"/>
&emsp;&emsp;还是很好玩的。
<img src="https://i.loli.net/2018/11/24/5bf8e719bdf68.gif" alt="11.gif" title="11.gif" />
&emsp;&emsp;突然想测一测这玩意的极限，首先拿人的模型来试试，先是1倍三角形：
<img src="https://i.loli.net/2018/11/23/5bf7e47876fc1.png" alt="10_.png" title="10_.png" style="zoom:80%"/>
&emsp;&emsp;2.5倍：
<img src="https://i.loli.net/2018/11/23/5bf7e4a8620bd.png" alt="11.png" title="11.png" style="zoom:80%"/>
&emsp;&emsp;结果3倍就爆炸了，也没有提示运行错误，不知道啥原因：
<img src="https://i.loli.net/2018/11/23/5bf7e4a6ac414.png" alt="12.png" title="12.png" style="zoom:80%"/>
&emsp;&emsp;15倍就更看不得了：
<img src="https://i.loli.net/2018/11/23/5bf7e4a73d857.png" alt="13.png" title="13.png" style="zoom:80%"/>
&emsp;&emsp;然后是这个简陋的椅子，先是1倍：
<img src="https://i.loli.net/2018/11/23/5bf7e5e93ecff.png" alt="14.png" title="14.png" style="zoom:80%"/>
&emsp;&emsp;3倍：
<img src="https://i.loli.net/2018/11/23/5bf7e5f1d8505.png" alt="15.png" title="15.png" style="zoom:80%"/>
&emsp;&emsp;6倍：
<img src="https://i.loli.net/2018/11/23/5bf7e5ed922f5.png" alt="16.png" title="16.png" style="zoom:80%"/>
&emsp;&emsp;10倍：
<img src="https://i.loli.net/2018/11/23/5bf7e5f31a346.png" alt="17.png" title="17.png" style="zoom:80%"/>
&emsp;&emsp;15倍都没爆炸：
<img src="https://i.loli.net/2018/11/23/5bf7e5f28fc3f.png" alt="18.png" title="18.png" style="zoom:80%"/>
&emsp;&emsp;以为这就是极限吗？不存在的。
&emsp;&emsp;76倍：
<img src="https://i.loli.net/2018/11/23/5bf7e8138be19.png" alt="19.png" title="19.png" style="zoom:80%"/>
&emsp;&emsp;219倍：
<img src="https://i.loli.net/2018/11/23/5bf7e814a4ef0.png" alt="20.png" title="20.png" style="zoom:80%"/>
&emsp;&emsp;290.5倍：
<img src="https://i.loli.net/2018/11/23/5bf7e81589806.png" alt="21.png" title="21.png" style="zoom:80%"/>
&emsp;&emsp;297.5倍，终于爆了哈哈哈！
<img src="https://i.loli.net/2018/11/23/5bf7e81615fac.png" alt="22.png" title="22.png" style="zoom:80%"/>
&emsp;&emsp;像我们程序员就得有刨根问底的精神！发现差不多将近20w左右的三角形顶点就爆了。
&emsp;&emsp;不过怎么改代码都无济于事，这应该是unity的上限，超过了这个值就GG了！
&emsp;&emsp;这个 yield return 不要经常调用，不然运算起来会慢出翔来。于是我设置了进度条更新的间隔为0.05(最大值为1)，超过这个间隔才调用yield return来显示一下：
```
var tmpV = sliderValueFz / (unselectedObjList.Count * totalSelectedObjTriangleNum);
if (tmpV- sliderGCP.value >= 0.05) {
    sliderGCP.value = tmpV;//更新进度条的值
    yield return new WaitForSeconds(0);
}

```
&emsp;&emsp;突然发现一个严峻的问题，那个调用物体bounds来优化的方法写错了，我竟然直接用的mesh.bounds，然而应该是用变换后的点的包围盒bounds来优化，o(︶︿︶)o 唉~再一次被自己蠢哭T_T
&emsp;&emsp;更正之后又被打回原形，先是1倍的，耗时2分04秒，共14740个接触点：
<img src="https://i.loli.net/2018/11/23/5bf7f3a2a5cc4.png" alt="23.png" title="23.png" style="zoom:80%"/>
&emsp;&emsp;然后是3倍的，耗时1分34秒，共11445个接触点：
<img src="https://i.loli.net/2018/11/23/5bf7f4785b98d.png" alt="24.png" title="24.png" style="zoom:80%"/>
&emsp;&emsp;最后是10倍的，耗时1分40秒，共个9885个接触点：
&emsp;&emsp;至于为什么椅子三角面片变多耗时反而变少？应该是算出来的冗余接触点少了，显示出来的物体少了，自然耗时变少了。
<img src="https://i.loli.net/2018/11/23/5bf7f5c6e5a6e.png" alt="25.png" title="25.png" style="zoom:80%"/>
&emsp;&emsp;拉近点看，感觉效果还不错：
<img src="https://i.loli.net/2018/11/23/5bf7f5cd5fc46.png" alt="26.png" title="26.png" style="zoom:80%"/>
<img src="https://i.loli.net/2018/11/23/5bf7f5c635998.png" alt="27.png" title="27.png" style="zoom:80%"/>
<img src="https://i.loli.net/2018/11/23/5bf7f5cc9111e.png" alt="28.png" title="28.png" style="zoom:80%"/>
<img src="https://i.loli.net/2018/11/23/5bf7f5c690c62.png" alt="29.png" title="29.png" style="zoom:80%"/>
&emsp;&emsp;不过计算时间这么慢肯定是不行的，之后想办法用k-d树或者瞎搞算法来优化一下计算的复杂度好了。另外那个Clear接触点按钮要是点数太多可能光清除就要一分钟左右，贼慢无比。
&emsp;&emsp;鉴于今天是周五晚上，就提前进入欢乐时光吧！
<img src="https://i.loli.net/2018/11/23/5bf7f70d2de0b.jpg" alt="1_18.jpg" title="1_18.jpg" style="zoom:50%"/>
