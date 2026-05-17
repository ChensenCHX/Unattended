# API 参考

本游戏中的所有世界交互均通过 Lua 脚本调用 API 接口完成。以下按功能分类索引。

---

## API 分类

- **[线程管理](chapter:thread)** —— 创建线程、检查线程状态、挂起等
- **[游戏操作](chapter:apifunc)** —— 移动、建造、收获、物品使用、设施交互等

---

## 快速查阅

| 常用接口 | 功能 | 所属类别 |
|---------|------|---------|
| `move(direction)` | 向指定方向移动（1=右 2=上 3=左 4=下） | [游戏操作](chapter:apifunc) |
| `build(type)` | 建造指定类型设施（2=Mana, 4=Ether, ...） | [游戏操作](chapter:apifunc) |
| `harvest()` | 收获当前位置设施 | [游戏操作](chapter:apifunc) |
| `can_harvest()` | 检查是否可收获 | [游戏操作](chapter:apifunc) |
| `use_item(type)` | 对当前设施使用物品 | [游戏操作](chapter:apifunc) |
| `get_item_count(type)` | 查询指定物品的持有数量 | [游戏操作](chapter:apifunc) |
| `interact_with(name, ...)` | 调用设施的自定义方法 | [游戏操作](chapter:apifunc) |
| `get_x_pos()` | 获取当前单位的 X 坐标 | [游戏操作](chapter:apifunc) |
| `get_y_pos()` | 获取当前单位的 Y 坐标 | [游戏操作](chapter:apifunc) |
| `new_thread(func)` | 创建新线程 | [线程管理](chapter:thread) |
| `check_thread(id)` | 检查线程存活状态 | [线程管理](chapter:thread) |
| `hangup_current_thread()` | 主动挂起当前线程 | [线程管理](chapter:thread) |
| `get_current_thread()` | 获取当前线程 ID | [线程管理](chapter:thread) |

---

[返回首页](chapter:main)
