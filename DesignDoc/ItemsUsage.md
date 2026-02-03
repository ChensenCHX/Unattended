# 物品作用设计文档  

## 写在前面
这里只描述特殊作用和常规的use_item()产生的影响
不包括建筑特性中的特殊用例 也不包括技术解锁  
如建筑特性特殊用例中有相同的部分会发生覆盖  

## ItemMana
无  

## FacilityEther
无  

## FacilityMelodia
持有时会自动消耗, 增加使魔的每帧指令执行量  
例: 上限100, 当前解锁技术50, 消耗5每使魔每帧提高到100  

## FacilityChronos
对建筑use_item后会使得建筑生长加速  

## FacilitySignum
持有时会自动消耗, 提高使魔的每帧指令执行上限  
例: 上限100, 消耗10每使魔每帧提高到200(有第二上限)  

## FacilityIter
持有时会自动消耗, 缩短使魔的移动时间  

## FacilityOpus
对建筑use_item后会使得建筑直接生长完成  