# Tech Tree

The tech tree allows you to spend accumulated resources for permanent upgrades. Tech nodes have progressive dependency relationships.

## Tech Dimensions

| Dimension | Levels | Core Effect |
|-----------|--------|-------------|
| Language Features | 1–8 | Unlocks Lua language features progressively: tables, closures, anonymous functions, coroutines, metatables |
| Movement Speed | 1–5 | Reduces unit travel time between grid cells |
| Workspace Size | 1–5 | Expands the grid map edge length, providing more buildable area |
| Thread Count | 1–5 | Increases the maximum number of parallel threads |
| Instruction Execution | 1–5 | Increases max instructions per thread per frame |
| Multi-threading | Standalone | Enables creation of multiple concurrent threads |

## Unlock Requirements

Each tech node requires:
1. All prerequisite dependency nodes must be unlocked
2. Pay the resource cost

Higher-level nodes may require multiple resource types. Clicking a pending node deducts resources and applies the effect immediately.

---

[Back to Home](chapter:main)
