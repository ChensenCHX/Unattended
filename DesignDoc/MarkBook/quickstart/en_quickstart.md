# Quick Start

## Overview

*Unattended* is a programming automation game. You write Lua scripts to control units that build facilities and harvest resources on a grid island. Your goal is to maximize resource output by optimizing your strategies.

## Basic Lua Syntax

The following syntax is available from the start:

- **Variables**: `local x = 10`
- **Conditionals**: `if ... then ... else ... end`
- **Loops**: `while ... do ... end` and `for i = 1, 10 do ... end`
- **Functions**: `function foo() ... end`

## Your First Automation Script

Here is a simple script that loops through building and harvesting a Mana facility:

```lua
while true do
    -- Move right one cell
    move(1)

    -- Build a Mana facility (type number 2)
    build(2)

    -- Wait for the facility to finish growing
    while not can_harvest() do
        -- Empty loop; the thread auto-yields each frame
    end

    -- Harvest the facility
    harvest()
end
```

## Key API Quick Reference

| API | Description | Details |
|-----|-------------|---------|
| `move(direction)` | Move unit in direction (1=right 2=up 3=left 4=down) | [API](chapter:api) |
| `build(type)` | Build a facility (1=Empty, 2=Mana, 4=Ether, ...) | [API](chapter:api) |
| `harvest()` | Harvest facility at current position | [API](chapter:api) |
| `can_harvest()` | Check if harvest is available | [API](chapter:api) |
| `use_item(type)` | Use an item (1=Mana, 2=Ether, ...) | [API](chapter:api) |
| `get_item_count(type)` | Query held quantity of an item | [API](chapter:api) |
| `interact_with(name, ...)` | Call a facility's custom interaction | [API](chapter:api) |

## Tips for Progression

1. Start with the [Mana facility](chapter:FacilityMana) basic build–harvest loop
2. Learn about [facility rules](chapter:facility) and [item effects](chapter:Item)
3. Unlock more language features via the [tech tree](chapter:upgrade)
4. Check the [API reference](chapter:api) for complete function docs

---

[Back to Home](chapter:main)
