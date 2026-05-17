# FacilitySignum

## 基本信息

| 项目 | 说明 |
|------|------|
| 产出资源 | Signum |
| 建造消耗 | Ether + Melodia |
| 收获后退化 | Ether |
| 建造前置 | 基于 Ether 的地块 |

## 核心规则

Signum 的机制围绕**信号传输**展开。每个 Signum 设施拥有两个属性：

- **高度（Height）**，取值范围 1~128
- **强度（Strength）**，取值范围 1~4

信号从 Signum 向四个方向发出，沿网格传播。当信号遇到第一个 `Height ≥ 自身 Height` 的 Signum 时，信号被接收并停止传播。其余类型设施的 Height 视为 0。

收获时，所有接收到当前设施信号的源 Signum 会被连锁收获，产量 = 基础产量 × 连锁个数² ×（接收到信号的 Strength 之和 + 自身 Strength）。

## 策略提示

- 信号传播受 Height 属性控制，调整 Height 可以改变信号传输网络的拓扑
- 如果需要重构信号链路，可以通过在中间位置建造更高 Height 的设施，或提前销毁阻碍传输的设施来重新规划信号流
- 可以通过 `interact_with("detach")` 将设施脱离网络，脱离后收获仅获得基础产量
- 考察玩家对信号传播规则和网络拓扑优化的理解

## 交互接口

| 方法 | 签名 | 说明 |
|------|------|------|
| `get_height` | `get_height() → int` | 返回设施当前的 Height 值（1~128） |
| `get_strength` | `get_strength() → int` | 返回设施当前的 Strength 值（1~4） |
| `detach` | `detach() → void` | 将设施脱离信号网络，不影响收发关系，收获仅获基础产量 |

---

[← 设施目录](chapter:facility)　|　[返回首页](chapter:main)
