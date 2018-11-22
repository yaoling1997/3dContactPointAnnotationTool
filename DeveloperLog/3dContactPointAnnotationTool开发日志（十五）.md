&emsp;&emsp;有时候拖动一个窗口的时候可能直接拖出去了那就再也拖不回来只能reset重新来过：
<img src="https://i.loli.net/2018/11/22/5bf6130e92186.png" alt="1.png" title="1.png" style="zoom:80%"/>
&emsp;&emsp;于是开了个类成员变量在start里记录了一下panel的位置：
```
var lp = panel.GetComponent<RectTransform>().localPosition;
localPosition = new Vector2(lp.x,lp.y);
```
&emsp;&emsp;在OnClick中将其复原：
```
panel.GetComponent<RectTransform>().localPosition = localPosition;
```
&emsp;&emsp;这样再点击对应的按钮就能复原panel的位置了！
&emsp;&emsp;那个坐标轴我都放到原点了为啥还要放到相机里当儿子，为自己感到智商捉急。
<img src="https://i.loli.net/2018/11/22/5bf617db71444.png" alt="3.png" title="3.png" style="zoom:80%"/>
&emsp;&emsp;也许使用者不想看见这个丑陋的坐标轴，于是我又添加了按钮A来控制它是否显示。
<img src="https://i.loli.net/2018/11/22/5bf61ae797559.png" alt="4.png" title="4.png" style="zoom:80%"/>
&emsp;&emsp;换了一种计算接触点的方法，枚举选中物体和未选中物体的三角面片，各自生成与x、y、z轴平行的包围盒然后算包围盒的相交部分，再生成x、y、z缩放比例与相交部分对应的不规则球，复杂度是三角面片数量的平方的，效果如下：
<img src="https://i.loli.net/2018/11/22/5bf65195a3084.png" alt="5.png" title="5.png" style="zoom:80%"/>
&emsp;&emsp;很明显的缺陷就是如果三角形太大并且不平行坐标轴就会导致较大误差，就像这样：
<img src="https://i.loli.net/2018/11/22/5bf653e957a38.png" alt="6.png" title="6.png" style="zoom:80%"/>
&emsp;&emsp;如果接触点过多会导致异常的卡，上图有1w多个接触点，就很卡了。
&emsp;&emsp;想让结果更精确就得将三角形划分得更细致，于是又得弄个slider让用户能控制一个物体三角面片的数量。
&emsp;&emsp;划分三角形的时候我每次都找最长边最长的三角形进行划分，在最长边上取个中点然后和该边所对顶点一连线就成了两个小三角形。本来想用堆来维护的，但是发现捉急的c#没有优先队列这个玩意，难不成还得手写一个？
&emsp;&emsp;网上搜了搜，发现一个叫作sortedList的东西貌似可以用用。
&emsp;&emsp;写完之后运行才发现这货不能有重复键的点，被坑了。。
<img src="https://i.loli.net/2018/11/22/5bf6a3f480c76.png" alt="7.png" title="7.png" style="zoom:80%"/>
&emsp;&emsp;魔高一尺道高一丈，网上还是有解决不能有重复键的方法，那就是自定一个类，顺便学会了c#如何比较自定义的类的两个实例的大小，具体请看[C# SortedList 可重复键的排序键/值对集合](http://blog.51cto.com/fengyuzaitu/1903015)。
&emsp;&emsp;然而用了这个方法GetByIndex方法就消失了，真是神奇得可怕，搞得我只能让它从大到小排序然后用GetEnumerator方法来访问第一个元素，再用removeAt(0)删除第一个元素了。
&emsp;&emsp;然后发现sortedlist插入移除复杂度竟然是O(n)的，真是****....不过人家的名字都带list了你还以为它效率能高到哪里去？真是太天真了。
&emsp;&emsp;貌似sortedDictionary是个不错的替代品。唉，当初自己手写队列早就弄完了，还会用这些渣渣封装数据结构？不过我竟然会对这些鬼东西感兴趣，所以还是了解一下吧。各集合类型的性能分析请看[C#集合类型总结和性能分析](https://blog.csdn.net/chen8238065/article/details/47018271)
&emsp;&emsp;当然为了用到sortedDictionary的Remove函数，比较类要写成这样：
```
public class CostComparer:IComparer<Cost>
{
    public int Compare(Cost x,Cost y)
    {
        if (x.cost == y.cost && x.id == y.id)
            return 0;
        if (x.cost < y.cost||(x.cost==y.cost&&x.id<y.id))
            return 1;
        else
            return -1;
    }
}
```
&emsp;&emsp;之前少了第一个if导致一直remove失败，现在终于好了！
&emsp;&emsp;还有就是要取到第一个元素得这么写，不要少了moveNext，否则取到的啥都没有：
```
var e = sd.GetEnumerator();
e.MoveNext();
var ec = e.Current;
int[] triangle = ec.Value;//取最长边最长的三角形进行划分      
```
&emsp;&emsp;终于过编译了，来看看效果。先是1倍三角形：
<img src="https://i.loli.net/2018/11/22/5bf6b58217036.png" alt="8.png" title="8.png" style="zoom:80%"/>
&emsp;&emsp;接下来就是见证奇迹的时刻：
<img src="https://i.loli.net/2018/11/22/5bf6b57ee1100.png" alt="9.png" title="9.png" style="zoom:80%"/>
&emsp;&emsp;瞬间爆炸，不知道哪里写萎了，不过已经搞到晚上10点钟了，明天再接着找bug吧，因为欢乐时光就要开始了！:)