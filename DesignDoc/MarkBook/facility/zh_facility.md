# 设施速览

*Unattended* 中包含七种设施类型，按建造依赖逐级递进。每种设施具有独特的产出规则和算法机制。

## 设施列表

| 设施 | 产出资源 | 核心规则 | 详情 |
|------|---------|---------|------|
| Mana | Mana | 基础生长与收获，无特殊规则 | [→](chapter:FacilityMana) |
| Ether | Ether | 产量受相邻同类设施影响 | [→](chapter:FacilityEther) |
| Melodia | Melodia | 收获时属性不重复可获得递增加成 | [→](chapter:FacilityMelodia) |
| Chronos | Chronos | 在指定时间窗口内操作以触发增产 | [→](chapter:FacilityChronos) |
| Signum | Signum | 信号收发，产量取决于信号网络规模 | [→](chapter:FacilitySignum) |
| Iter | Iter | 无向图连接网络，通过调整边结构优化产出 | [→](chapter:FacilityIter) |
| Opus | Opus + 连锁 | 运行康威生命游戏，演化结果决定连锁产出 | [→](chapter:FacilityOpus) |

## 建造依赖

设施之间具有严格的建造依赖关系，由低到高依次为：

**Empty → Mana → Ether → Melodia → Chronos → Signum → Iter → Opus**

每种设施只能在指定的前置设施上建造，高级回退建造低级是合法的。

---

[返回首页](chapter:main)
