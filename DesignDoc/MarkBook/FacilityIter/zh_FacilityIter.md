# FacilityIter

## 基本信息

| 项目 | 说明 |
|------|------|
| 产出资源 | Iter |
| 建造消耗 | Ether + Signum |
| 收获后退化 | Ether |
| 建造前置 | 基于 Signum 的地块 |

## 核心规则

FacilityIter 的机制围绕**图连通性优化**展开。建造时随机与场上其他 Iter 设施建立带权边的连接，构成无向图。产量 = 基础产量 × 连锁收获个数³ / 所有激活边的权重之和。

玩家可以通过以下接口主动调整图结构：
- `connect(x, y)`：建立到坐标 (x, y) 处 Iter 的连接
- `disconnect(x, y)`：断开与坐标 (x, y) 处 Iter 的连接

收获时会连锁收获所有直接或间接连接的 Iter 设施。

## 策略提示

- 边的权重直接影响产量，权重越大的边对总产出的拖累越大
- 需要权衡激活边的数量（增加连锁收获个数）与边权重之和（降低分母）的平衡
- 可以通过断连高权重边并重连到更优位置来优化图结构
- 考察玩家对图论、最小生成树和权重优化等概念的理解

## 交互接口

| 方法 | 签名 | 说明 |
|------|------|------|
| `get_edges` | `get_edges() → table` | 返回 `{x=[...], y=[...], weight=[...], state=[...]}` 四个并列数组 |
| `connect` | `connect(int x, int y) → bool` | 尝试连接到指定坐标的 Iter，成功/已连接返回 true |
| `disconnect` | `disconnect(int x, int y) → bool` | 尝试断开与指定坐标 Iter 的连接，成功/已断连返回 true |

---

[← 设施目录](chapter:facility)　|　[返回首页](chapter:main)
