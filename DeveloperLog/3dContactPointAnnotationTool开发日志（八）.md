&emsp;&emsp;今天上午去实验室打算把项目从github上pull下来发现貌似不行，然后强行pull下来后项目变得乱七八糟了，有的组件都不知道去哪里了。去github上看了看发现上面day6和day7都没有，特别奇怪。没办法又得慢慢改，这git的问题贼烦人。
&emsp;&emsp;修改好后再commit给我弹了这么些东西：
<img src="https://i.loli.net/2018/11/15/5bed55a9a57fc.png" alt="1.png" title="1.png" style="zoom:50%"/>
&emsp;&emsp;去网上搜寻解决方案，都是些不知所云的东西，看得我头皮发麻。于是只好暴力重建仓库，也就是删除原有的，重建一个，clone下来，再把修改好的项目放到clone下来的文件夹里再push，终于没有这些烦人的红字了。
&emsp;&emsp;按照射线法的思路，采用了[射线和三角形的相交检测（ray triangle intersection test）](https://www.cnblogs.com/samen168/p/5162337.html)中的方法来判断三角形和射线是否相交。然后得到的接触点结果如下：
<img src="https://i.loli.net/2018/11/15/5bed55b3ea4b3.png" alt="2.png" title="2.png" style="zoom:50%"/>
&emsp;&emsp;不仅相当不准确而且非常耗时间，于是只能再想别的办法。
&emsp;&emsp;既然只是粗略地求表面的接触点，可以直接判断枚举的点是否在另一个物体某个三角面片的包围盒内就行了吧。改了后1分钟就求出来了，效果也很好而且也不用复杂的数学和算法知识。改进后的结果如下：
<img src="https://i.loli.net/2018/11/15/5bed55b40f3fc.png" alt="3.png" title="3.png" style="zoom:50%"/>
&emsp;&emsp;选中椅子再来一发，椅子的三角形比较粗糙，结果也不是很好，三角形还是越精密越好：
<img src="https://i.loli.net/2018/11/15/5bed55b4de749.png" alt="4.png" title="4.png" style="zoom:50%"/>