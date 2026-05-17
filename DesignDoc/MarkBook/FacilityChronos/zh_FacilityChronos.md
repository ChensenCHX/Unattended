# FacilityChronos

## 基本信息

| 项目 | 说明 |
|------|------|
| 产出资源 | Chronos |
| 建造消耗 | Melodia |
| 收获后退化 | Ether |
| 建造前置 | 基于 Ether 的地块 |

## 核心规则

Chronos 的核心机制围绕**时序精准度**展开。调用 `interact_with("start", tolerance)` 后，设施返回一个目标物品类型和一个帧数窗口。玩家需要在 `[Framecount - Tolerance, Framecount + Tolerance]` 帧数区间内对该设施执行 `use_item(targetItem)`，成功则触发增产状态。

增产状态下产量大幅提升，普通状态仅获得基础产量。查询设施是否处于增产状态可以调用 `interact_with("check")`。

## 策略提示

- 帧数窗口通常较长，等候期间可以调度可操作单位先执行其他任务，在窗口临近时返回提交物品
- 本机制考察玩家对单线程/多线程环境下多 I/O 任务的调度能力
- 增产状态下再次调用 `start` 无意义
- 帧数计数从调用 `start` 时刻开始

## 交互接口

| 方法 | 签名 | 说明 |
|------|------|------|
| `start` | `start(int tolerance) → ItemType, int` | 返回目标物品类型和帧数窗口值 |
| `check` | `check() → string` | 返回当前状态：`"init"`, `"waiting"`, `"success"`, `"fail"` |

---

[← 设施目录](chapter:facility)　|　[返回首页](chapter:main)
