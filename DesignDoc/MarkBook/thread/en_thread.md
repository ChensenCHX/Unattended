# Thread Management

## Overview

Use `new_thread` to create new threads in Lua. Each thread is bound to a unit. Multi-threading allows simultaneous control of multiple units.

---

## `new_thread(func) → int`

Creates a new thread executing `func`. A new unit is spawned at the calling thread's current position.

- **Parameters**: `func` — Lua function to execute in the new thread
- **Returns**: Thread ID (integer), or `false` on failure

```lua
local thread_id = new_thread(function()
    while true do
        -- New thread logic here
    end
end)
```

---

## `check_thread(id) → bool`

Checks whether a thread is still alive.

- **Parameters**: `id` — Thread ID
- **Returns**: `true` if alive, `false` if terminated

---

## `hangup_current_thread()`

Voluntarily suspends the current thread, yielding execution to the game engine. The thread resumes on the next frame.

---

## `get_current_thread() → int`

Returns the ID of the currently executing thread.

---

## `get_current_frame_count() → int`

Returns the total frame count since game start. Useful for timing logic.

---

## `atomic_compare_and_swap_at(table, key, old, new) → old`

Performs an atomic compare-and-swap on a Lua table for safe cross-thread state synchronization.

- **Parameters**: `table` — target table, `key` — key, `old` — expected old value, `new` — new value
- **Returns**: The value before the operation

---

[← API Directory](chapter:api)　|　[Back to Home](chapter:main)
