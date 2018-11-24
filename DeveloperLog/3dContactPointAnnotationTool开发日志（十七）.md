&emsp;&emsp;今天又改进了一下算法，把生成出来的接触点按中心坐标拍了个序，再把中心坐标一样的坐标点合并，x,y,z每个维度都只保留大的值。然后来看看效果：
&emsp;&emsp;先是1倍的，只剩下4096个接触点了，2^12个，少了1w多个接触点，耗时也只需要46秒。
<img src="https://i.loli.net/2018/11/24/5bf90deaf3f64.png" alt="1.png" title="1.png" style="zoom:80%"/>
&emsp;&emsp;然后是3倍的，4026个接触点，耗时46秒。
<img src="https://i.loli.net/2018/11/24/5bf90deb00301.png" alt="2.png" title="2.png" style="zoom:80%"/>
&emsp;&emsp;最后是10倍的，4030个接触点，耗时55秒。
<img src="https://i.loli.net/2018/11/24/5bf90ebc5fb1c.png" alt="3.png" title="3.png" style="zoom:80%"/>