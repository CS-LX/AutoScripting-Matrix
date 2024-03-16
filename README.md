- 作者：Linxium

# [智械]矩阵 AutoScripting-Matrix

<!-- TOC -->

* [[智械]矩阵 AutoScripting-Matrix](#智械矩阵-autoscripting-matrix)
  * [简介](#简介)
  * [预备知识](#预备知识)
    * [矩阵](#矩阵)
      * [基础概念](#基础概念)
        * [举个例子](#举个例子)
        * [元素表示](#元素表示)
        * [名称](#名称)
        * [其他例子](#其他例子)
      * [矩阵变换](#矩阵变换)
      * [TRS矩阵](#trs矩阵)
        * [基础概念](#基础概念-1)
        * [提取变换](#提取变换)
        * [组合变换](#组合变换)
      * [运算](#运算)
        * [加法与减法](#加法与减法)
        * [乘法](#乘法)
        * [逆矩阵与除法](#逆矩阵与除法)
        * [转置](#转置)
        * [点对点运算](#点对点运算)
    * [浮点数](#浮点数)
    * [向量](#向量)
  * [方块介绍](#方块介绍)
    * [源](#源)
      * [矩阵源-从观察创建](#矩阵源-从观察创建)
      * [二阶方阵转矩阵](#二阶方阵转矩阵)
      * [四维横向量转矩阵](#四维横向量转矩阵)
      * [矩阵源-实时钟](#矩阵源-实时钟)
    * [转换器](#转换器)
      * [导线](#导线)
      * [处理器](#处理器)
      * [计算器](#计算器)
      * [解构器](#解构器)
    * [用电器](#用电器)
      * [复杂摄像机LED](#复杂摄像机led)
        
        <!-- TOC -->

## 简介

这是一个给生存战争带来矩阵电路系统的模组。它与原版传输信号的电路存在些许关联，但是功能却大相径庭。传输内容为矩阵使得你可以用它做一些~~炫酷的~~实用的机械、操作

---

## 预备知识

这个模组所涉及到的知识包含小学学习的小数、中学学习的向量以及大学学习的矩阵。其中矩阵的知识较难。（~~可能会劝退很多玩家~~）**但是！** 想要玩这个mod，你不需要非常精通矩阵，你只需知晓一些基本概念与玩法，便可以”矩阵在手，天下我有~“

在这个教程中，为照顾不同学历，不同学龄的玩家，作者会尽可能简单地讲解知识概念，保证各位玩家能看懂！

### 矩阵

#### 基础概念

由 $m \times n$ 个数 $a _ {ij}(i=1,2,3...,m; j=1,2,3...,n)$ 排成的 $m$ 行 $n$ 列的数表被称为 $m$ 行 $n$ 列矩阵，简称 $m \times n$ 矩阵。

##### 举个例子

如：

$$
\mathbf{M}=\begin{bmatrix}1 & 2 \\\\\\ 3 & 4\end{bmatrix}
$$

##### 元素表示

上式矩阵，被称为 $2 \times 2$ 矩阵，其中 $1,2,3,4$ 这四个数被称为矩阵的**元素**，简称**元**。其中 $2$ 是矩阵第一行第二列的数，为矩阵 $\mathbf{M}$ 的 $(1, 2)$ 元。而在此mod中，任意矩阵的 $(i, j)$ 元可以写为 $Mij$ .~~（因为老K在SC的代码里就是这么表示矩阵元素的）~~

##### 名称

你可能要问：为什么以 $M$ 作为矩阵的名称？那必然是因为： $M$ 是矩阵英语 $matrix$ 的首字母，用 $M$ 作为矩阵名称那必然是非常合适~

当然，你也可以用其它符号作为矩阵名称，并且可以在矩阵名称下方加上小尾巴表示矩阵的尺寸，如上面的矩阵可以写成： $\mathbf{M} _ {2 \times 2}$ .

##### 其他例子

上面只有一个例子，没看懂？

好吧，那我在下面多举几个例子好啦~

$$
\mathbf{M} _ {2 \times 2}=\begin{bmatrix}1 & 1 \\\\\\ 4 & 5\end{bmatrix}
$$

$$
\mathbf{A} _ {4 \times 3}=\begin{bmatrix}1 & 0 & -1\\\\\\ 0 & 1 & 0 \\\\\\ 0 & 0 & 1 \\\\\\ -1 & 0 & 1\end{bmatrix}
$$

$$
\begin{bmatrix}T _ {11} & T _ {12} & R _ {11} & R _ {12} \\\\\\ T _ {21} & T _ {22} & R _ {21} & R _ {22} \\\\\\ B _ {11} & B _ {12} & L _ {11} & L _ {12} \\\\\\ B _ {21} & B _ {22} & L _ {21} & L _ {22} \end{bmatrix}
$$

上面这些都是矩阵哦~

#### 矩阵变换

那么，介绍了那么多基础知识，矩阵...到底有什么用呢？

矩阵的用处繁多，但是对于我们的mod，我们只需要知晓它其中一种作用——变换

> 矩阵变换是一种常见的计算机图形学技术，通过矩阵相乘的方式对向量进行平移、旋转和缩放等操作。
> 
> 在进行复杂的物体变换时，通常需要结合多种变换操作，而这些需要使用TRS矩阵。
> 
> TRS矩阵是由平移矩阵（T）、旋转矩阵（R）和缩放矩阵（S）相乘而成的一个复合矩阵，用来实现对向量的多种变换。通过TRS矩阵，我们可以方便地将平移、旋转和缩放操作结合在一起，实现各种复杂的物体变换效果。
> 
> 在计算机图形学和计算机动画中，TRS矩阵是一种非常重要且常用的技术，可以帮助我们实现各种动态和逼真的图形效果。**而在本mod中，矩阵系统的基础及电压皆为TRS矩阵。掌握TRS矩阵，你就可以玩转本mod~**

#### TRS矩阵

##### 基础概念

TRS矩阵是一个包含了位移（Translation）、旋转（Rotation）和缩放（Scale）信息的矩阵，通常用于描述物体在三维空间中的变换。

一个TRS矩阵如下式所示：

$$
\begin{bmatrix}R _ x & R _ y & R _ z &  0 \\\\\\ U _ x & U _ y & U _ z &  0 \\\\\\ F _ x & F _ y & F _ z & 0 \\\\\\ d _ x & d _ y & d _ z & 1 \end{bmatrix}
$$

 将一组正交基坐标应用上式的矩阵进行变换（即矩阵乘法）：

$\vec{v _ x} = (1, 0, 0)→\vec{v ^ , _ x} = (R _ x + d _ x, R _ y + d _ y, R _ z + d _ z)$ 

$\vec{v _ y} = (0, 1, 0)→\vec{v ^ , _ y} = (U _ x + d _ x, U _ y + d _ y, U _ z + d _ z)$

$\vec{v _ z} = (0, 0, 1)→\vec{v ^ , _ z} = (F _ x + d _ x, F _ y + d _ y, F _ z + d _ z)$ 

由此可见：TRS矩阵第一行前三个数表示基向量 $\vec{x}$ 变换后的新坐标，TRS矩阵第而行前三个数表示基向量 $\vec{y}$ 变换后的新坐标，TRS矩阵第三行前三个数表示基向量 $\vec{z}$ 变换后的新坐标；而第四行前三个数表示坐标系原点变换后应该平移至的新坐标。

##### 提取变换

要从TRS矩阵中获取位移、旋转和缩放信息，可以按照以下步骤进行解析：

1. 位移（Translation）：TRS矩阵的第四行向量表示了物体的位移信息，即矩阵中的元素T(x, y, z)。通过提取这个向量，就可以得到物体在三维空间中的位移信息。

2. 旋转（Rotation）：TRS矩阵的旋转部分通常是一个3x3的旋转矩阵R（M11至M33所囊括的区域），表示了物体围绕各个坐标轴的旋转变换。可以通过对这个旋转矩阵进行解析，得到物体的旋转角度和方向。

3. 缩放（Scale）：TRS矩阵的缩放部分通常是一个对角矩阵S，表示了物体在各个坐标轴上的缩放比例。可以通过提取对角元素，得到物体在各个坐标轴上的缩放比例。

为简化计算与操作，mod提供了一系列解构器，使得你可以快速地提取矩阵的变换。

##### 组合变换

我们可以通过位移、旋转、缩放创建对应矩阵，最后把它们乘起来得到最终的TRS矩阵。但是，采用此方法非常困难，计算次数非常大 ~~（以至于摆一个创建TRS矩阵的电路都要占5~6个区块）~~ 。所以为了方便起见，mod提供了一系列矩阵源，使得你可以快速地创建想要的矩阵。

#### 运算

##### 加法与减法

矩阵加法与减法是将两个矩阵对应的元素相加减：

$$
\begin{bmatrix}1 & 2 \\\\\\ 3 & 4\end{bmatrix} + \begin{bmatrix}5 & 6 \\\\\\ 7 & 8\end{bmatrix}
= \begin{bmatrix}1 + 5 & 2 + 6 \\\\\\ 3 + 7 & 4 + 8\end{bmatrix}
= \begin{bmatrix}6 & 8 \\\\\\ 10 & 12\end{bmatrix}
$$

$$
\begin{bmatrix}1 & 2 \\\\\\ 3 & 4\end{bmatrix} - \begin{bmatrix}5 & 6 \\\\\\ 7 & 8\end{bmatrix}
= \begin{bmatrix}1 - 5 & 2 - 6 \\\\\\ 3 - 7 & 4 - 8\end{bmatrix}
= \begin{bmatrix}-4 & -4 \\\\\\ -4 & -4\end{bmatrix}
$$

##### 乘法

矩阵乘法如下式所示：

$$
\begin{bmatrix}
a & b \\\\\\
c & d
\end{bmatrix}
\begin{bmatrix}
e & f \\\\\\
g & h
\end{bmatrix}
=
\begin{bmatrix}
ae + bg & af + bh \\\\\\
ce + dg & cf + dh
\end{bmatrix}
$$

以此可以类推到 $4 \times 4$ 的TRS矩阵的乘法表达式。 ~~（太过庞大，这里就不展示了）~~

矩阵与普通数字的乘法大有不同。数字的乘法倾向于描述数字的倍数；而矩阵乘法更倾向于合并两个变换。

> 举个例子：
> 
> 瓦尔特站在木屋里，他要去河边取水。他先从屋子里走了出来，然后走到了河边打水。
> 
> 对于上面的例子，我们可以说瓦尔特做了两个动作：先[从屋子里走了出来]，然后[走到了河边打水]。
> 
> **而我们亦可以说他只做了一个动作：** [从屋子里走了出来 $\times$ 走到了河边打水]

##### 逆矩阵与除法

如果一个矩阵乘另一个矩阵得到了单位阵： $\mathbf{M} \times \mathbf{N} = \mathbf{I}$ ，那么说 $\mathbf{N}$ 是 $\mathbf{M}$ 的逆矩阵。$\mathbf{N}$ 也可以表示为 $\mathbf{M ^ {-1}}$ .

站在矩阵变换的角度：如果一个矩阵 $\mathbf{A}$ 对应着一个线性变换T，那么逆矩阵 $\mathbf{A ^ {-1}}$ 对应着T的逆变换，即将T作用在向量上的结果再次作用在这个结果上可以得到原始向量。

> 还是那个例子：
> 
> 瓦尔特站在木屋里，他要去河边取水。他先从屋子里走了出来，然后走到了河边打水。
> 
> 对于上面的例子，瓦尔特的动作 [从屋子里走了出来] 的“逆动作”则是 [从外面走进了屋子]

在了解了逆矩阵的定义以及使用后，我们便可以借助逆矩阵定义矩阵除法：

$$
\mathbf{M} / \mathbf{N} = \mathbf{M} \mathbf{N ^ {-1}} (乘号已省略)
$$

##### 转置

矩阵转置是指将一个矩阵的行和列互换的操作。

转置后的矩阵的行数变为原矩阵的列数，列数变为原矩阵的行数。转置操作不改变矩阵中元素的值，只是改变了元素的位置。

矩阵 $\mathbf{A}$ :

$$
\begin{bmatrix}
1 & 2 & 3\\\\\\
4 & 5 & 6
\end{bmatrix}
$$

矩阵 $\mathbf{A}$ 的转置矩阵 $\mathbf{A ^ {T}}$ ：

$$
\begin{bmatrix}
1 & 4 \\\\\\
2 & 5 \\\\\\
3 & 6
\end{bmatrix}
$$

##### 行列式

矩阵行列式是一个数学工具，用来判断矩阵是否可逆以及求解方程组的解。行列式的值可以告诉我们矩阵的特征值和特征向量的性质，对于线性代数和数学分析等领域有着重要的应用。

由于矩阵行列式与矩阵变换——即本mod的主题——相距甚远，所以在此不作说明。感兴趣的玩家们可以自行了解~

##### 点对点运算

矩阵的点对点运算是指对两个尺寸相同的矩阵中对应位置上的元素进行相同的运算操作。

以点对点乘法作例子：

$$
\begin{bmatrix}
a & b \\\\\\
c & d
\end{bmatrix}
\times
\begin{bmatrix}
e & f \\\\\\
g & h
\end{bmatrix}
=
\begin{bmatrix}
a \times e & b \times f \\\\\\
c \times g & d \times h
\end{bmatrix}
$$

本mod提供了大量点对点运算板，玩家们可以用它们实现快速的数据处理。

### 浮点数

对于只有 $1 \times 1$ 矩阵，我们一般把它叫做数。而在此mod中，我们把它叫做浮点数。

> 浮点数就是计算机里用来表示带小数点的数字的一种数据类型。比如我们常见的3.14、1.5、0.001等都属于浮点数。浮点数可以表示很大或很小的数字，但在计算时可能会有一些精度上的误差。所以在处理需要非常准确的小数运算时，需要特别注意浮点数的精度问题。 

### 向量

对于只有一行或者一列的矩阵（其余元素全为0），我们可以把它看作行向量或列向量。

> **注意：在本mod中，向量统一使用行向量！**

---

## 方块介绍

  本mod方块分为三大类：

- 源

- 转换器

- 用电器
  
  > 源指可以提供矩阵数据的方块，如`拉杆、按钮`等  
  > 转换器指可起到传输作用并且对矩阵进行一些转换操作的方块，如`导线、四维计算板`等
  > 用电器指可以利用矩阵产生一些效果的方块，如`方块展示板，实体变换板、矩阵LED`等
  
  ---

### 源

| 方块                | 输入/输出面                       | 作用                                                                                                                                |
| ----------------- | ---------------------------- | --------------------------------------------------------------------------------------------------------------------------------- |
| 矩阵源               | 输出：6端                        | 输出玩家拟定好的矩阵，初始放置时默认数据为单位阵，破坏后不会丢失数据。<br /><br />可通过对话框编辑内容。                                                                        |
| 矩阵开关              | 输出：前后左右底端                    | 拉杆朝下为关，输出零矩阵；拉杆朝上为开，输出玩家拟定好的矩阵。初始放置时默认数据为单位阵，破坏后不会丢失数据。<br /><br />可通过对话框编辑内容。                                                    |
| 矩阵按钮              | 输出：前后左右底端                    | 每当按下按钮即输出一个持续约0.10秒的玩家拟定好的矩阵，之后恢复输出零矩阵。初始放置时默认数据为单位阵，破坏后不会丢失数据。<br /><br />可通过对话框编辑内容。                                            |
| 矩阵源: 世界方块坐标       | 输出：前端                        | 输出包含本元件所处的坐标的位移矩阵。                                                                                                                |
| 矩阵源: 玩家变换         | 输入：后端（时钟端） <br /><br />输出：前端 | 输出包含离此元件最近的玩家的世界变换矩阵（包含玩家坐标与旋转）。当时钟端没有接线时，此元件每时每刻输出；而当时钟端接线后，当时钟端输入矩阵M11上升沿（由0开始上升）时，获取一次矩阵并且输出。                                  |
| 矩阵源: 玩家摄像机视图      | 输入：后端（时钟端）<br /><br /> 输出：前端 | 输出最近玩家的摄像机视图矩阵，包含观测的位置，方向，垂直Y方向 **（注意: 若要直接使用此矩阵作为绘制的变换，须对其求逆）**。当时钟端没有接线时，此元件每时每刻输出；而当时钟端接线后，当时钟端输入矩阵M11上升沿（由0开始上升）时，获取一次矩阵并且输出。 |
| 矩阵源: 玩家摄像机投影      | 输出：左右端                       | 输出最近玩家的摄像机投影矩阵，左端输出投影矩阵，右端输出屏幕投影矩阵。                                                                                               |
| 矩阵源: 从位移创建        | 输入：左后右底端<br /><br />输出：前端    | 创建一个位移矩阵，x为左端（浮点）+底端输入M11，y为后端（浮点）+底端输入M12，z为右端（浮点）+底端输入M13。                                                                      |
| 矩阵源: 从欧拉角(三轴旋转)创建 | 输入：左后右底端<br /><br />输出：前端    | 根据欧拉角创建一个旋转矩阵，**采用弧度制！** yaw为左端（浮点）+底端输入M11，pitch为后端（浮点）+底端输入M12，roll为右端（浮点）+底端输入M13。                                             |
| 矩阵源: 从缩放创建        | 输入：底端<br /><br />输出：前端       | 根据缩放创建一个缩放矩阵。缩放矩阵的三轴缩放皆为输入值。                                                                                                      |
| 矩阵源: 从三轴缩放创建      | 输入：左后右底端<br /><br />输出：前端    | 根据三轴缩放创建一个缩放矩阵。x为左端（浮点）+底端输入M11，y为后端（浮点）+底端输入M12，z为右端（浮点）+底端输入M13。                                                                |
| 矩阵源: 从观察创建        | 输入：左后右端<br /><br />输出：前端     | 详见[矩阵源-从观察创建](#矩阵源-从观察创建)                                                                                                         |
| 矩阵源: 随机常数矩阵       | 输入：后端（时钟端）<br /><br /> 输出：前端 | 输出一个介于0~1的随机常数矩阵。当时钟端没有接线时，此元件每时每刻输出；而当时钟端接线后，当时钟端输入矩阵M11上升沿（由0开始上升）时，获取一次矩阵并且输出。                                                 |
| 二阶方阵转矩阵           | 输入：前后左右端<br /><br />输出：底端    | 详见[二阶方阵转矩阵](#二阶方阵转矩阵)                                                                                                             |
| 四维横向量转矩阵          | 输入：前后左右端<br /><br />输出：底端    | 详见[四维横向量转矩阵](#四维横向量转矩阵)                                                                                                           |
| 浮点转四维横向量          | 输入：前后左右端<br /><br />输出：底端    | 输出一个四维向量(x, y, z, w)。x：前端；y：右端；z：后端；w：左端。各输入端口都为浮点。                                                                               |
| 矩阵源: 从正交投影创建      | 输入：前后左右端 <br /><br />输出：底端   | 输出一个正交投影矩阵。前端（浮点）和右端（浮点）分别代表投影平面的宽度和高度，后端（浮点）和左端（浮点）分别代表近裁剪面和远裁剪面的距离。                                                             |
| 矩阵源: 从透视投影创建      | 输入：前后左右端 <br /><br />输出：底端   | 输出一个透视投影矩阵。前端（浮点）和右端（浮点）分别代表垂直视角的视野范围和宽高比，后端（浮点）和左端（浮点）分别代表近裁剪面和远裁剪面的距离。                                                          |
| 矩阵源: 实时钟          | 输入：底端<br /><br />输出：前后左右端    | 详见[矩阵源-实时钟](#矩阵源-实时钟)                                                                                                             |

#### 矩阵源-从观察创建

  创建一个观察矩阵。其中左端输入摄像机位置（三维向量），后端输入目标位置（三维向量），右端输入向上方向（三维向量）。

- 摄像机位置：表示相机的位置，即相机所在的坐标点。

- 目标位置：表示相机要观察的目标位置，即相机要对准的物体的坐标点。

- 向上方向：表示相机的上方向向量，用于确定相机的方向。通常情况下，up向量应该垂直于相机的观察方向向量，确保相机的朝向是正确的。在这个元件种，up向量会被重新计算，以确保它与相机的观察方向向量和右方向向量垂直。

#### 二阶方阵转矩阵

将四个输入端输入的二阶方阵转为四阶方阵。

$$
上端\begin{bmatrix}T _ {11} & T _ {12} \\\\\\ T _ {21} & T _ {22}\end{bmatrix}右端\begin{bmatrix}R _ {11} & R _ {12} \\\\\\ R _ {21} & R _ {22}\end{bmatrix}下端\begin{bmatrix}B _ {11} & B _ {12} \\\\\\ B _ {21} & B _ {22}\end{bmatrix}左端\begin{bmatrix}L _ {11} & L _ {12} \\\\\\ L _ {21} & L _ {22}\end{bmatrix}→输出\begin{bmatrix}T _ {11} & T _ {12} & R _ {11} & R _ {12} \\\\\\ T _ {21} & T _ {22} & R _ {21} & R _ {22} \\\\\\ B _ {11} & B _ {12} & L _ {11} & L _ {12} \\\\\\ B _ {21} & B _ {22} & L _ {21} & L _ {22} \end{bmatrix}
$$

#### 四维横向量转矩阵

将四个输入端输入的横向量并成四阶方阵。

$$
上端\begin{pmatrix}T _ {11} & T _ {12} & T _ {13} & T _ {14}\end{pmatrix}右端\begin{pmatrix}R _ {11} & R _ {12} & R _ {13} & R _ {14}\end{pmatrix}下端\begin{pmatrix}B _ {11} & B _ {12} & B _ {13} & B _ {14}\end{pmatrix}左端\begin{pmatrix}L _ {11} & L _ {12} & L _ {13} & L _ {14}\end{pmatrix}→输出\begin{bmatrix}T _ {11} & T _ {12} & T _ {13} & T _ {14} \\\\\\ R _ {11} & R _ {12} & R _ {13} & R _ {14} \\\\\\ B _ {11} & B _ {12} & B _ {13} & B _ {14} \\\\\\ L _ {11} & L _ {12} & L _ {13} & L _ {14} \end{bmatrix}
$$

#### 矩阵源-实时钟

  根据控制矩阵输出实时时间。

  **输入：**

| 端口  | 作用   | 详细             |
| --- | ---- |:-------------- |
| 前   | 输出时间 | 为常数矩阵          |
| 右   | 输出时间 | 为常数矩阵          |
| 后   | 输出时间 | 为常数矩阵          |
| 左   | 输出时间 | 为常数矩阵          |
| 底   | 控制矩阵 | 详见下方“**控制矩阵**” |

  **控制矩阵：**

- ⌊M11⌋=0
  
  | 端口  | 输出（系统时间） |
  | --- | -------- |
  | 前   | 毫秒       |
  | 右   | 秒        |
  | 后   | 分        |
  | 左   | 时        |

- ⌊M11⌋=1
  
  | 端口  | 输出（系统时间） |
  | --- | -------- |
  | 前   | 星期       |
  | 右   | 天        |
  | 后   | 月        |
  | 左   | 年        |

- ⌊M11⌋=2
  
  | 端口  | 输出（游戏世界时间） |
  | --- | ---------- |
  | 前   | 秒          |
  | 右   | 分          |
  | 后   | 时          |
  | 左   | 天          |
  
  ------

### 转换器

> 注: 一些转换器有标注“点对点”，它们的运算逻辑为将矩阵的每个对应元素分别计算或转换的结果，再返回由这些结果组成的新矩阵。此种转换模型应与一般的矩阵转换模式区别开。
> (点对点转换器与一般的转换器在模型上也会有些许区别)

#### 导线

| 方块    | 输入/输出面                   | 作用                                                            |
| ----- | ------------------------ | ------------------------------------------------------------- |
| 矩阵导线  | 输入：6端 <br/><br/> 输出：6端   | 传输矩阵数据，导线交汇处计算为加法。不同颜色导线不会互相连接。                               |
| 矩阵继电器 | 输入：左右后端 <br/><br/> 输出：前端 | 当左或右面输入矩阵M11大于0时，输出后端输入的矩阵，否则输出0矩阵。                           |
| 矩阵穿线块 | 输入：6端 <br/><br/>输出：6端    | 跨方块传输矩阵数据。带有导线截面的两面为接线端，其余四面绝缘。<br/>可以用它优化排线，或是作为不同颜色导线的连接桥梁。 |

#### 处理器

  **非点对点：**

| 方块       | 作用                                  |
| -------- | ----------------------------------- |
| 矩阵求逆     | 求输入矩阵的逆矩阵。                          |
| 矩阵转置     | 转置输入矩阵并输出。                          |
| 矩阵二次方    | 按照矩阵乘法法则，将输入矩阵乘它本身。                 |
| 矩阵三次方    | 按照矩阵乘法法则，将输入矩阵乘它本身再乘它本身。            |
| 矩阵转平均浮点数 | 将矩阵内每一个非0元素相加再除以非0元素的个数，将所得浮点数结果输出。 |
| 浮点数转常数矩阵 | 输出一个内部元素全为输入浮点数的矩阵。                 |
| 矩阵转三维横向量 | 输出一个三维向量，内容是输入矩阵M11，M12，M13的值。      |
| 矩阵提取取向   | 求输入矩阵的取向矩阵。                         |
| 矩阵提取平移   | 求输入矩阵的平移矩阵。                         |
| 矩阵求行列式   | 求输入矩阵的行列式，输出浮点数。                    |

  **点对点：**

| 方块      | 作用                                  |
| ------- | ----------------------------------- |
| 矩阵弧度转角度 | 依据点对点计算，将输入矩阵内所有元素转为角度制。**（使用弧度制）** |
| 矩阵角度转弧度 | 依据点对点计算，将输入矩阵内所有元素转为弧度制。**（使用角度制）** |
| 矩阵正弦    | 依据点对点计算，将输入矩阵内所有元素求正弦值。**（使用弧度制）**  |
| 矩阵余弦    | 依据点对点计算，将输入矩阵内所有元素求余弦值。**（使用弧度制）**  |
| 矩阵正切    | 依据点对点计算，将输入矩阵内所有元素求正切值。**（使用弧度制）**  |
| 矩阵反正弦   | 依据点对点计算，将输入矩阵内所有元素求反正弦值。**（使用弧度制）** |
| 矩阵反余弦   | 依据点对点计算，将输入矩阵内所有元素求反余弦值。**（使用弧度制）** |
| 矩阵反正切   | 依据点对点计算，将输入矩阵内所有元素求反正切值。**（使用弧度制）** |

#### 计算器

  **非点对点**

| 方块    | 作用                           |
| ----- | ---------------------------- |
| 矩阵乘法器 | 按照矩阵乘法法则，将输入矩阵相乘。            |
| 矩阵除法器 | 按照矩阵乘法法则，将左端输入矩阵乘右端输入矩阵的逆矩阵。 |

  **点对点**

| 方块    | 作用（ML代替“左端输入矩阵各元素“，MR代替“右端输入矩阵各元素“） |
| ----- | ----------------------------------- |
| 矩阵加法器 | ML + MR                             |
| 矩阵减法器 | ML - MR                             |
| 矩阵乘法器 | ML * MR                             |
| 矩阵除法器 | ML / MR                             |
| 矩阵乘方器 | ML ^ MR                             |
| 矩阵除余器 | ML % MR                             |
| 矩阵对数器 | ln(ML) / ln(MR)                     |
| 矩阵取小器 | min(ML, MR)                         |
| 矩阵取大器 | max(ML, MR)                         |

#### 解构器

| 方块            | 输入/输出面                                   | 作用                                                                |
| ------------- | ---------------------------------------- | ----------------------------------------------------------------- |
| 矩阵TO解构器       | 输入：底端  <br /><br />输出：左端（1点）右端（2点）       | 将输入矩阵提取其位移矩阵与取向矩阵，并在左端输出位移矩阵（T），右端输出取向矩阵（O）。                      |
| 矩阵TRS解构器      | 输入：底端  <br /><br />输出：左端（1点）顶端（2点）右端（3点） | 将输入矩阵提取其位移、旋转（欧拉角）与缩放，并在左端输出位移（三维向量），上端输出旋转（弧度制欧拉角），右端输出缩放（三维向量）。 |
| 矩阵RUF(XYZ)解构器 | 输入：底端  <br /><br />输出：左端（1点）顶端（2点）右端（3点） | 将经过输入矩阵变换后的的X单位向量，Y单位向量，Z单位向量分别输出至左端，上端，右端。                       |
| 矩阵转二阶方阵       | 输入：底端<br /><br />输出：前后左右端                | 与“[二阶方阵转矩阵](#二阶方阵转矩阵)”相反。                                         |
| 矩阵转四维横向量      | 输入：底端<br /><br />输出：前后左右端                | 与“[四维横向量转矩阵](#四维横向量转矩阵)”相反。                                       |
| 四维横向量转浮点      | 输入：底端<br /><br />输出：前后左右端                | 解构输入四维向量(x, y, z, w)。x：前端输出；y：右端输出；z：后端输出；w：左端输出。各出端口都为浮点。        |

  ---

### 用电器

| 方块       | 输入/输出面          | 作用                                                          |
| -------- | --------------- | ----------------------------------------------------------- |
| 实体变换板    | 输入：左端（1点）右端（2点） | 将触碰到的实体变换按照左端输入矩阵进行映射并返回给实体，而实体的速度按照右端输入矩阵进行映射并返回给实体。掉落物同理。 |
| 复杂摄像机LED | 输入：前后左右端        | 详见[复杂摄像机LED](#复杂摄像机LED)                                     |

#### 复杂摄像机LED

  创建一个摄像机，并且将摄像机捕捉到的画面显示出来。

  **输入：**

| 端口  | 作用                       | 详细                                                                  |
| --- | ------------------------ |:------------------------------------------------------------------- |
| 前   | 摄像机视角矩阵，即摄像机方位、旋转        |                                                                     |
| 右   | 摄像机投影矩阵，即摄像机绘制画面所采用的投影矩阵 | 一般采用透视矩阵或正交矩阵                                                       |
| 后   | 显示屏变换矩阵                  | 默认显示屏位置为位于方块中心、朝向为正上方的正方形。此矩阵为这个正方形显示屏的四个端点的变换。（无法显示画面则可能因为这个输入未到位） |
| 左   | 控制矩阵                     | 详见下方“**控制矩阵**”                                                      |

  **控制矩阵：**

- M11：画面是否自适应显示，即不会被拉伸（M11>0：是，反之否）
