&emsp;&emsp;今天想着在Windows平台上跑通那个代码，不过它的官网上写的支持平台不包括windows，但我还是想试试，因为看他的依赖好像和平台的关系不是特别大。
<img src="https://i.loli.net/2019/01/10/5c3752acc0701.png" alt="0.png" title="0.png" />
&emsp;&emsp;看了下它的py代码，不知道是py2还是py3，于是干脆py2和py3上都试下。它的py依赖都在requirements.txt文件中：
	numpy>=1.11.0
	scipy>=0.17.1
	chumpy
	opendr
	matplotlib
&emsp;&emsp;其它的直接用pip安装就好，就是这个opendr在Windows10上装的时候有问题：
	Exception: Unable to get url: http://files.is.tue.mpg.de/mloper/opendr/osmesa/OSMesa.Windows.AMD64.zip
&emsp;&emsp;于是我百度到了这个玩意：[https://github.com/mattloper/opendr/issues/11](https://github.com/mattloper/opendr/issues/11)，上面有解决办法。说白了就是从[这个网站](https://github.com/polmorenoc/opendr)把项目clone下来然后将opendr文件夹丢到python插件的那个文件夹(D:\Python27\Lib\site-packages)，然后进到opendr文件夹里使用 python setup.py install           指令安装该py包即可。到此py的依赖都搞好了。
&emsp;&emsp;然后还要从官网下opencv和smpl for python。
&emsp;&emsp;win10+vs2017+opencv3.4请看[这个](https://www.cnblogs.com/lzhu/p/8198654.html)
&emsp;&emsp;smpl for python我从smpl上下载下来速度奇慢无比，感觉今天是弄不好了。