# 快速入门

## 概述

*Unattended* 是一款编程自动化游戏。你需要编写 Lua 脚本来控制可操作单位在小岛上完成设施建造和资源收获，通过不断优化策略来最大化资源产出。

## 基础 Lua 语法

游戏初期开放以下 Lua 语法：

- **变量赋值**：`local x = 10`
- **条件判断**：`if ... then ... else ... end`
- **循环**：`while ... do ... end` 和 `for i = 1, 10 do ... end`
- **函数定义与调用**：`function foo() ... end`

## 第一个自动化脚本

下面是一个简单的自动化示例，循环建造并收获 Mana 设施：

```lua
while true do
    -- 向右移动一格
    move(1)

    -- 建造 Mana 设施（类型编号 2）
    build(2)

    -- 等待设施生长完成
    while not can_harvest() do
        -- 空循环等待，每帧自动挂起
    end

    -- 收获设施
    harvest()
end
```

## 关键接口速查

| 接口 | 说明 | 详见 |
|------|------|------|
| `move(direction)` | 向指定方向移动（1=右 2=上 3=左 4=下） | [API](chapter:api) |
| `build(type)` | 建造指定类型设施（1=Empty, 2=Mana, 4=Ether, ...） | [API](chapter:api) |
| `harvest()` | 收获当前位置设施 | [API](chapter:api) |
| `can_harvest()` | 检查是否可收获 | [API](chapter:api) |
| `use_item(type)` | 对当前设施使用物品（1=Mana, 2=Ether, ...） | [API](chapter:api) |
| `get_item_count(type)` | 查询指定物品的持有数量 | [API](chapter:api) |
| `interact_with(name, ...)` | 调用设施的自定义方法 | [API](chapter:api) |

## 提升建议

1. 先熟悉 [Mana 设施](chapter:FacilityMana) 的基本建造—收获循环
2. 了解各 [设施规则](chapter:facility) 和 [物品效果](chapter:Item)
3. 通过 [科技树](chapter:upgrade) 解锁更多语言特性
4. 查阅 [API 参考](chapter:api) 获取完整函数说明

---

[返回首页](chapter:main)
