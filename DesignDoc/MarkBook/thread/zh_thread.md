# 线程管理

## 概述

在 Lua 中通过 `new_thread` 创建新线程，每个线程绑定一个可操作单位。多线程允许你同时控制多个单位并行工作。

---

## `new_thread(func) → int`

创建一个新线程并执行 `func`。新线程在调用线程的所在位置生成一个新的可操作单位。

- **参数**：`func` — 要在线程中执行的 Lua 函数
- **返回值**：新线程的 ID（整数），失败返回 `false`

```lua
local thread_id = new_thread(function()
    while true do
        -- 新线程的执行逻辑
    end
end)
```

---

## `check_thread(id) → bool`

检查指定线程是否仍在运行。

- **参数**：`id` — 线程 ID
- **返回值**：存活返回 `true`，已终止返回 `false`

---

## `hangup_current_thread()`

主动挂起当前线程，将执行权交还给游戏引擎。当前线程将在下一帧恢复执行。

---

## `get_current_thread() → int`

获取当前正在执行的线程 ID。

---

## `get_current_frame_count() → int`

获取当前游戏运行的总帧数。可用于实现计时逻辑。

---

## `atomic_compare_and_swap_at(table, key, old, new) → old`

在 Lua 表上执行原子化的比较并交换操作，用于多线程间的安全状态同步。

- **参数**：`table` — 目标表，`key` — 键，`old` — 期望的旧值，`new` — 新值
- **返回值**：操作前的旧值

---

[← API 目录](chapter:api)　|　[返回首页](chapter:main)
