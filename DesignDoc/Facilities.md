# 设施设计文档

## 画风约束
都是晶体状的物品 只有材质和Mesh不同  

## 共通设置
基础产量 = 1 (Item)  
基础设置消耗 = 1 (Item)  
InteractWith的调用约定按以下描述  
name(args...) => rets...  
Lua中实际调用时为InteractWith(string name, args...), 返回值为rets...

## FacilityEmpty
初始地块 只作为占位使用 无产出  
特性：无  
  
产量公式：无  
设置要求：Any  
设置消耗：None  
收获后退化：None

## FacilityStone
产出Stone 普通设施 无特殊要求  
特性：无  
  
产量公式：无  
设置要求: Any  
设置消耗：None  
收获后退化：-> FacilityEmpty

## FacilityQuartz
产出Quartz 有要求设施  
特性：共振：生长时长 *= 2^(放置时周围相同设施数)  
  
产量公式：基本产量  
设置要求：T where T based on FacilityStone  
设置消耗：Stone  
收获后退化：-> FacilityStone  

## FacilityMelody
产出Melody 有要求设施  
特性：旋律：每个FacilityMelody身上会携带一个Tone  
在收获Melody时如果与上次的Tone不同，会获得额外产出量  
身上携带一个InteractWith方法 签名为 get_tone() => int
调用后返回设施上携带的Tone的值  
  
产量公式：基本产量 * min(16, 连续不同次数)  
设置要求：T where T based on FacilityStone  
设置消耗：Quartz  
收获后退化：-> FacilityStone  

## FacilityChronos
产出Chronos 有要求设施  
特性：时间：每个FacilityChronos身上会携带一个随机的Delay(int), 表示帧数区间  
身上携带两个InteractWith方法 签名为 start(int) => Item.Type, int 与 check() => bool  
调用check后返回的信息为是否处于增产状态  
调用start时传入一个整型值Tolerance, 返回的结果是一个物品类型Require与一个整数值Framecount  
玩家需要在 [Framecount-Time, Framecount+Time] 的帧数区间内对设施UseItem(Require) 且有足够的物品供消耗  
如成功则设施进入增产状态 增产状态下再调用start方法无意义  
  
产量公式：check() == true ? round(基本产量 * 16 / min(16, sqrt(abs(Tolerance)))) : 基本产量  
设置要求：T where T based on FacilityQuartz  
设置消耗：Melody  
收获后退化：-> FacilityQuartz  

## FacilityIter
产出Iter 有要求设施  
特性：旅行：每个FacilityIter在建造时会随机挑选场上3~5个非自身的FacilityIter作为连接目标  
如场上FacilityIter不足则降低挑选量 之后与每个连接目标之间生成一条带权边  
身上携带三个InteractWith方法  
get_edges() => table\<EdgeInfo\>; connect(int, int) => bool; disconnect(int, int) => bool;  
调用get_edges方法后获得一个装有EdgeInfo的table, 其中EdgeInfo为一个table  
EdgeInfo形为 { target_x = int, target_y = int, edge_weight = int, enable = bool } 描述边信息  
调用 connect(int x, int y) 方法后会尝试连接到(x, y), 成功|已连接返回true, 不在表中返回false  
调用 disconnect(int x, int y) 方法后会尝试与(x, y)断联, 成功|已断连返回true, 不在表中返回false  
**注意** 假设有后建造的 A B 两个FacilityIter  
B在生成时若随机到A会同时向A的可连接表中添加到B的连接 即连接是无向图  
收获时会连锁收获所有连接在一起的FacilityIter

产量公式：基本产量 * 连锁收获棵数^3 / 所有激活边的权重之和  
设置要求：T where T based on FacilityQuartz  
设置消耗：Quartz  
收获后退化：-> FacilityQuartz  

## 占位 留给算法考察


## FacilityOpus
产出**特殊产出, 下述** 有要求设施  
特性：著作：场上最多存在一个FacilityOpus 在场上已有FacilityOpus时尝试设置会失败  
身上携带四个InteractWith方法与一个内部状态State  
start() => bool; eval() => bool; add(int, int) => void; remove(int, int) => void; State初始为Init  
FacilityOpus会执行标准round border B3/S23生命游戏规则  
如State == Init调用start的瞬间扫描整个场地作为初始状态(有建筑视为1 无建筑视为0 自身所在位置始终视为0)并设置State为Running 否则返回false并设置State为Halt  
如State == Running调用eval会模拟执行一步并检查场地上建筑分布是否满足要求 返回bool表示场地状态与执行一步后是否相符 如返回false或State不为Running设置State为Halt  
调用add方法和remove方法会将给定的坐标设置为1/0 无返回值  
每次eval后 FacilityOpus会为场上的每个建筑计算Age, 规则如下：  
如果设施是上一轮已存在的 其Age += 1  
如果设施是上一轮不存在的 其Age = 周围三个细胞Age取平均向下取整  
如果设施是通过add添加的 其Age = 0  
**注意** 调用eval后如果FacilityOpus周围一圈八格有建筑 FacilityOpus会直接进入Halt状态并记录当前周围八格的种类和Age 此次判定返回值为true  

产量公式：  
如果State != Halt 无产出, 无动作
如果State == Halt 对周围八格 如果种类与记录相匹配则收获之  
额外产出其产出物 量为(相应建筑物收获产量 * Age) * 记录下周围八格的建筑个数  
设置要求：Any  
设置消耗：All  
收获后退化：-> FacilityEmpty  