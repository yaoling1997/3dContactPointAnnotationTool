&emsp;&emsp;师姐说物体间不能有穿透，于是我试了下给物体加rigidbody和meshCollider
<img src="https://i.loli.net/2019/01/08/5c3486962e357.png" alt="1.png" title="1.png" />
&emsp;&emsp;然后就报错：
<img src="https://i.loli.net/2019/01/08/5c34869642753.png" alt="2.png" title="2.png" />
&emsp;&emsp;说是用meshCollider要么去掉刚体要么就把刚体设置为iskinematic。说白了就是用meshCollider没法检测和别的物体的碰撞，于是就没加collider了。
&emsp;&emsp;然后就研究单幅图像如何生成人体模型比较方便，找到了一篇叫作Keep it SMPL: Automatic Estimation of 3D Human Pose and Shape from a Single Image的论文，[这个网站](http://smplify.is.tue.mpg.de/)上面有它的源码，是用python写的，linux下跑通应该没有压力，不过没试过，我倒是想在windows上跑通，然后直接取结果就好了，以后有空研究研究这东西怎么玩。
<img src="https://i.loli.net/2019/01/08/5c3488699cd5a.png" alt="3.png" title="3.png" />