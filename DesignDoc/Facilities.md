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

## FacilityMana
产出Mana 普通设施 无特殊要求  
特性：无  
  
产量公式：无  
设置要求: Any  
设置消耗：None  
收获后退化：-> FacilityEmpty

## FacilityEther
产出Ether 有要求设施  
特性：共振：生长时长 *= 2^(周围相同设施数)  
  
产量公式：基本产量 * 2^(4-周围相同设施数)  
设置要求：T where T based on FacilityMana  
设置消耗：Mana  
收获后退化：-> FacilityMana  

## FacilityMelodia
产出Melodia 有要求设施  
特性：旋律：每个FacilityMelodia身上会携带一个Tone  
在收获Melodia时如果与之前的Tone不同，会获得额外产出量  
FacilityMelodia会持有一个全局队列来保存之前收获的音符种类  
如果队列Count < 32则检查是否有重复 无重复则insert 有重复则清空再insert  
如果队列Count >= 32则先Dequeue直到回到< 32的情况  
身上携带两个InteractWith方法 签名为 get_tone() => int; reset() => void;  
get_tone调用后返回设施上携带的Tone的值 Tone取值范围为[0, 31]  


产量公式：基本产量 * min(16, 连续不同次数)  
设置要求：T where T based on FacilityMana  
设置消耗：Ether  
收获后退化：-> FacilityMana  

## FacilityChronos
产出Chronos 有要求设施  
特性：时间：每个FacilityChronos身上会携带一个随机的Delay(int), 表示帧数区间  
身上携带两个InteractWith方法 签名为 start(int) => Item.Type, int 与 check() => string  
调用check后返回的信息为是否处于增产状态(init, waiting, success, fail)  
调用start时传入一个整型值Tolerance, 返回的结果是一个物品类型Require与一个整数值Framecount  
玩家需要在 [Framecount-Time, Framecount+Time] 的帧数区间内对设施UseItem(Require) 且有足够的物品供消耗  
如成功则设施进入增产状态 增产状态下再调用start方法无意义  
  
产量公式：check() == true ? floor(基本产量 * 16 / min(16, sqrt(abs(Tolerance + 1)))) : 基本产量  
设置要求：T where T based on FacilityEther  
设置消耗：Melodia  
收获后退化：-> FacilityEther  

## FacilitySignum
产出Signum 有要求设施  
特性：信号：每个FacilitySignum会向四向发出信号 产出量由接收到信号量决定  
身上携带三个InteractWith方法与两个内部量Height[1, 128] Strength[1, 4]  
get_height() => int; 调用后获得设施的内部量Height的值  
get_strength() => int; 调用后获得设施的内部量Strength的值
detach() => void; 调用后将该建筑脱离传输 不再影响其他收发 收获时只获得基础产量  
信号的传输规则：所有非FacilitySignum的设施的Height视为0 信号传输到第一个Height >= 自身Height的FacilitySignum后被接收不再传输  
收获时会连锁收获所有接收到的信号的源FacilityIter

产量公式：基本产量 * 连锁收获个数^2 * 接收到信号的Strength之和
设置要求：T where T based on FacilityEther  
设置消耗：Ether, Melodia  
收获后退化：-> FacilityEther  

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

产量公式：基本产量 * 连锁收获个数^3 / 所有激活边的权重之和  
设置要求：T where T based on FacilitySignum  
设置消耗：Ether, Signum  
收获后退化：-> FacilityEther  

## FacilityOpus
产出Opus与**特殊产出, 下述** 有要求设施  
特性：著作：场上最多存在一个FacilityOpus 在场上已有FacilityOpus时尝试设置会失败  
身上携带三个InteractWith方法与一个内部状态State  
start() => table?; eval() => bool; add(int, int) => bool; State初始为Init  
FacilityOpus会执行标准round border B3/S23生命游戏规则  
如State == Init调用start 设施会生成一个初始状态表返回给玩家(应有建筑为1 应无建筑为0 自身所在位置及周围一圈一定为0)并设置State为Running 否则返回nil并设置State为Halt  
如State == Running调用eval会模拟执行一步并检查场地上建筑分布是否满足要求(不考虑自身 自身所在位置一定为0) 返回bool表示场地状态与执行一步后是否相符 如返回false或State不为Running设置State为Halt  
调用add方法会将给定的坐标设置为1 消耗1 Opus 返回是否成功(Opus不足会失败)  
每次eval后 FacilityOpus会为场上的每个建筑计算Age, 规则如下：  
如果设施是上一轮已存在的 其Age += 1  
如果设施是上一轮不存在的 其Age = 周围三个细胞Age取平均向下取整  
如果设施是通过add添加的 其Age = 0  
**注意** 调用eval后如果FacilityOpus周围一圈八格有建筑 FacilityOpus会直接进入Halt状态并记录当前周围八格的种类和Age 此次判定返回值为true  
视觉效果: 设施会标记下轮期望的形状 相应地块上会有黄色的光效 如地块上已有设施则更换为绿色光效 若不应有建筑的地块有建筑则会有红色光效

产量公式：  
如果State != Halt 产出 基础产量的Opus, 无动作  
如果State == Halt 产出 基础产量 * 周围八格记录下的建筑数量的Opus 对周围八格 如果种类与记录相匹配则收获之  
额外产出它们的产出物 量为(相应建筑物收获产量 * Age) * 记录下周围八格的建筑个数   
设置要求：Any  
设置消耗：All(Expt Opus)  
收获后退化：-> FacilityEmpty  