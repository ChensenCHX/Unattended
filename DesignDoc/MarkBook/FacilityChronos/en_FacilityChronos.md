# FacilityChronos

## Basic Info

| Property | Value |
|----------|-------|
| Resource | Chronos |
| Build Cost | Melodia |
| Degenerates To | Ether |
| Build Prerequisite | Plot based on Ether |

## Core Rule

Chronos revolves around **timing precision**. Call `interact_with("start", tolerance)` to receive a target item type and a frame window. The player must call `use_item(targetItem)` on the facility within `[Framecount − Tolerance, Framecount + Tolerance]` frames. Success triggers a boosted yield state.

Query the current state via `interact_with("check")`. Boosted state yields far more than the base state.

## Strategy Tips

- Frame windows are typically long; you can schedule other tasks in the meantime and return near the window close
- This mechanic tests single/multi-threaded I/O task scheduling ability
- Calling `start` while already in boosted state has no effect
- Frame counting begins from the moment `start` is called

## Interaction Methods

| Method | Signature | Description |
|--------|-----------|-------------|
| `start` | `start(int tolerance) → ItemType, int` | Returns target item type and frame window value |
| `check` | `check() → string` | Returns current state: `"init"`, `"waiting"`, `"success"`, `"fail"` |

---

[← Facility Overview](chapter:facility)　|　[Back to Home](chapter:main)
