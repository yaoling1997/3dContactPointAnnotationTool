&emsp;&emsp;把image也做成panel的形式了，并且放进了scrollView里，真实地显示出图像：
<img src="https://i.loli.net/2018/11/18/5bf0cb0130ee7.png" alt="1.png" title="1.png" style="zoom:50%"/>
&emsp;&emsp;其它两个scrollView的content也做成自适应大小了，就是添加一项content的height就会增加相应的那么多。
<img src="https://i.loli.net/2018/11/18/5bf0cb01315a6.png" alt="2.png" title="2.png" style="zoom:50%"/>
&emsp;&emsp;为了把三维模型投影到图片上，之前的做法显然不行于是又做了个图片自适应的panel用来对齐三维模型和图片。
<img src="https://i.loli.net/2018/11/18/5bf120c88643e.png" alt="3.png" title="3.png" style="zoom:50%"/>
&emsp;&emsp;可以通过调整alpha来控制图片透明度，scale控制图片大小。
<img src="https://i.loli.net/2018/11/18/5bf120c92978c.png" alt="4.png" title="4.png" style="zoom:50%"/>
<img src="https://i.loli.net/2018/11/18/5bf120c92a6fd.png" alt="5.png" title="5.png" style="zoom:50%"/>
&emsp;&emsp;slider和图片离得太远了，还得使PanelImage的宽度和高度能够自动更新才行。
<img src="https://i.loli.net/2018/11/18/5bf120c929f66.png" alt="6.png" title="6.png" style="zoom:50%"/>
&emsp;&emsp;发现这么简单的功能实现起来还是挺麻烦的，网上也没有很好的示例，只能靠自己yy。