# API Reference

All in-game interactions are performed by calling API functions from Lua scripts. Below is a categorized index.

---

## API Categories

- **[Thread Management](chapter:thread)** — Thread creation, status checks, suspension
- **[Game Operations](chapter:apifunc)** — Move, build, harvest, item usage, facility interaction

---

## Quick Reference

| Common API | Function | Category |
|-----------|----------|----------|
| `move(direction)` | Move in direction (1=right 2=up 3=left 4=down) | [Game Ops](chapter:apifunc) |
| `build(type)` | Build a facility (2=Mana, 4=Ether, ...) | [Game Ops](chapter:apifunc) |
| `harvest()` | Harvest facility at current position | [Game Ops](chapter:apifunc) |
| `can_harvest()` | Check harvest availability | [Game Ops](chapter:apifunc) |
| `use_item(type)` | Use item on current facility | [Game Ops](chapter:apifunc) |
| `get_item_count(type)` | Query held quantity of an item | [Game Ops](chapter:apifunc) |
| `interact_with(name, ...)` | Call facility's custom method | [Game Ops](chapter:apifunc) |
| `get_x_pos()` | Get current unit X coordinate | [Game Ops](chapter:apifunc) |
| `get_y_pos()` | Get current unit Y coordinate | [Game Ops](chapter:apifunc) |
| `new_thread(func)` | Create a new thread | [Threads](chapter:thread) |
| `check_thread(id)` | Check thread alive status | [Threads](chapter:thread) |
| `hangup_current_thread()` | Suspend current thread | [Threads](chapter:thread) |
| `get_current_thread()` | Get current thread ID | [Threads](chapter:thread) |

---

[Back to Home](chapter:main)
