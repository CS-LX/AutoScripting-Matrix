# [智械]矩阵 AutoScripting-Matrix
## 简介
这是一个给生存战争带来矩阵电路系统的模组。它与原版传输信号的电路存在些许关联，但是功能却大相径庭。传输内容为矩阵使得你可以用它做一些~~炫酷的~~实用的机械、操作
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
| 方块            | 输入/输出面                             | 作用                                                                                          |
|---------------|--------------------------------|----------------------------------------------------------------------------------------------------|
| 矩阵源            | 输出：6端                             | 输出玩家拟定好的矩阵，初始放置时默认数据为单位阵，破坏后不会丢失数据。                            |
| 矩阵开关          | 输出：前后左右底端                  | 拉杆朝下为关，输出零矩阵；拉杆朝上为开，输出玩家拟定好的矩阵。初始放置时默认数据为单位阵，破坏后不会丢失数据。                                                                                            |
| 矩阵按钮          | 输出：前后左右底端                    | 每当按下按钮即输出一个持续约0.10秒的玩家拟定好的矩阵，之后恢复输出零矩阵。初始放置时默认数据为单位阵，破坏后不会丢失数据。 |
| 矩阵源: 世界方块坐标      | 输出：前端                        | 输出包含本元件所处的坐标的位移矩阵。 |
| 矩阵源: 玩家变换      | 输入：后端（时钟端） 输出：前端         | 输出包含离此元件最近的玩家的世界变换矩阵（包含玩家坐标与旋转）。当时钟端没有接线时，此元件每时每刻输出；而当时钟端接线后，当时钟端输入矩阵M11上升沿（由0开始上升）时，获取一次矩阵并且输出。 |
| 矩阵源: 玩家摄像机视图      | 输入：后端（时钟端） 输出：前端         | 输出最近玩家的摄像机视图矩阵，包含观测的位置，方向，垂直Y方向 （注意: 若要直接使用此矩阵作为绘制的变换，须对其求逆）。当时钟端没有接线时，此元件每时每刻输出；而当时钟端接线后，当时钟端输入矩阵M11上升沿（由0开始上升）时，获取一次矩阵并且输出。 |
| 矩阵源: 随机常数矩阵 | 输入：后端（时钟端） 输出：前端         | 输出一个介于0~1的随机常数矩阵。当时钟端没有接线时，此元件每时每刻输出；而当时钟端接线后，当时钟端输入矩阵M11上升沿（由0开始上升）时，获取一次矩阵并且输出。 |
| 矩阵源: 从正交投影创建 | 输入：前后左右端 输出：底端         | 输出一个正交投影矩阵。上端（浮点）和右端（浮点）分别代表投影平面的宽度和高度，下端（浮点）和左端（浮点）分别代表近裁剪面和远裁剪面的距离。 |
| 矩阵源: 从透视投影创建 | 输入：前后左右端 输出：底端         | 输出一个透视投影矩阵。上端（浮点）和右端（浮点）分别代表垂直视角的视野范围和宽高比，下端（浮点）和左端（浮点）分别代表近裁剪面和远裁剪面的距离。 |

---

### 转换器
> 注: 一些转换器有标注“点对点”，它们的运算逻辑为将矩阵的每个对应元素分别计算或转换的结果，再返回由这些结果组成的新矩阵。此种转换模型应与一般的矩阵转换模式区别开。
> (点对点转换器与一般的转换器在模型上也会有些许区别)
#### 多输入多输出：

| 方块            | 输入/输出面                             | 作用                                                                                          |
|---------------|--------------------------------|----------------------------------------------------------------------------------------------------|
| 矩阵导线       | 输入：6端  输出：6端             | 传输矩阵数据，导线交汇处计算为加法。不同颜色导线不会互相连接。|
| 矩阵继电器     | 输入：左右后端  输出：前端        | 当左或右面输入矩阵M11大于0时，输出后端输入的矩阵，否则输出0矩阵。 |
#### 单输入单输出：

| 方块            | 输入/输出面                             | 作用                                                                                          |
|---------------|--------------------------------|----------------------------------------------------------------------------------------------------|
#### 双输入单输出：

| 方块            | 输入/输出面                             | 作用                                                                                          |
|---------------|--------------------------------|----------------------------------------------------------------------------------------------------|
#### 单输入多输出
| 方块            | 输入/输出面                             | 作用                                                                                          |
|---------------|--------------------------------|----------------------------------------------------------------------------------------------------|
| 矩阵TO解构器   | 输入：底端  输出：左端（1点）右端（2点）| 将输入矩阵提取其位移矩阵与取向矩阵，并在左端输出位移矩阵（T），右端输出取向矩阵（O）。|
| 矩阵TRS解构器 | 输入：底端  输出：左端（1点）顶端（2点）右端（3点）| 将输入矩阵提取其位移、旋转（欧拉角）与缩放，并在左端输出位移（三维向量），上端输出旋转（弧度制欧拉角），右端输出缩放（三维向量）。|
| 矩阵RUF(XYZ)解构器 | 输入：底端  输出：左端（1点）顶端（2点）右端（3点）| 将经过输入矩阵变换后的的X单位向量，Y单位向量，Z单位向量分别输出至左端，上端，右端。 |

---

### 用电器
| 方块            | 输入/输出面                             | 作用                                                                                          |
|---------------|--------------------------------|----------------------------------------------------------------------------------------------------|
| 实体变换板     | 输入：左端（1点）右端（2点）      | 将触碰到的实体变换按照左端输入矩阵进行映射并返回给实体，而实体的速度按照右端输入矩阵进行映射并返回给实体。掉落物同理。 |
