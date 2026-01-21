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

## 待定 至少再出三个 考察数据结构，算法和多线程同步