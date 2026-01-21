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
设置消耗：Melody  
收获后退化：-> FacilityQuartz  

## 待定 至少再出两个 考察算法和多线程同步