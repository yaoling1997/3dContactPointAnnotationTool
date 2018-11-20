&emsp;&emsp;为了使生成的项目能够显示报错信息我又勾选了下面这几个选项：
<img src="https://i.loli.net/2018/11/20/5bf377007ca2e.png" alt="0.png" title="0.png" style="zoom:80%"/>
&emsp;&emsp;然后生成的项目运行时可以显示错误信息了，貌似是shader是空的。
<img src="https://i.loli.net/2018/11/20/5bf377006296d.png" alt="1.png" title="1.png" style="zoom:80%"/>
&emsp;&emsp;之前的代码是这么写的，调用了Shader.Find()，貌似不行。
```
meshRenderer.material = new Material(Shader.Find("Standard"));
```
&emsp;&emsp;于是直接把要用的shader给拖到我的objManager对象的脚本里：
<img src="https://i.loli.net/2018/11/20/5bf3826b81b10.png" alt="2.png" title="2.png" style="zoom:80%"/>
&emsp;&emsp;然后用上下面的代码：
```
meshRenderer.material = new Material(GameObject.Find("ObjManager").GetComponent<ObjManager>().shaderStandard);
```
&emsp;&emsp;然后就没问题了，哈哈哈哈！
<img src="https://i.loli.net/2018/11/20/5bf3826c2c326.png" alt="3.png" title="3.png" style="zoom:80%"/>
&emsp;&emsp;为了使其更加人性化，我又给每个scrollViewItem增加了delete功能，结果又要修改scrollViewItem的结构，农了一个多小时才完成，顺带解决了一些bug：
<img src="https://i.loli.net/2018/11/20/5bf3b2f41dc7b.png" alt="4.png" title="4.png" style="zoom:80%"/>
&emsp;&emsp;按一下sphere8旁边的x，sphere8就没了！
<img src="https://i.loli.net/2018/11/20/5bf3b2f41daab.png" alt="5.png" title="5.png" style="zoom:80%"/>
&emsp;&emsp;生成到windows平台上也是可以正常使用的！
<img src="https://i.loli.net/2018/11/20/5bf3b42713c7d.png" alt="6.png" title="6.png" style="zoom:80%"/>
&emsp;&emsp;可能有时候用户想手动添加接触点，所以还得弄个添加按钮。不过默认半径为1，所以可能会显得点有点大。然后又发现转2d点的时候貌似会比原来的大上一圈。
<img src="https://i.loli.net/2018/11/20/5bf3c8f44ceaa.png" alt="7.png" title="7.png" style="zoom:80%"/>
&emsp;&emsp;于是仔细看了看发现模型的scale的值对应的是直径的值，我之前一直把它当半径来着。
<img src="https://i.loli.net/2018/11/20/5bf3c8f3e9416.png" alt="8.png" title="8.png" style="zoom:80%"/>
&emsp;&emsp;改了之后就正常了。
<img src="https://i.loli.net/2018/11/20/5bf3ca522a328.png" alt="9.png" title="9.png" style="zoom:80%"/>
