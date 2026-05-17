# FacilityOpus

## Basic Info

| Property | Value |
|----------|-------|
| Resource | Opus + chain yields |
| Build Cost | All resources except Opus |
| Degenerates To | Empty |
| Build Prerequisite | Any plot |
| Special Limit | Only one Opus may exist on the workspace at a time |

## Core Rule

FacilityOpus runs the standard **B3/S23 Conway's Game of Life** on the workspace grid.

### Operation Flow

1. **Initialize**: Call `interact_with("start")` to generate an initial cell state table. The facility itself and its surrounding 8 cells are always 0.
2. **Edit Cells**: Use `interact_with("add", x, y)` and `interact_with("remove", x, y)` to set cell states, each costing 1 Opus.
3. **Evolve & Validate**: Call `interact_with("eval")` to advance one step and verify that the actual facility layout matches the evolution result.

### Termination & Rewards

When the 8 cells surrounding Opus contain buildings, the facility enters its terminal state. The types and ages of those buildings are recorded, and harvesting yields chain rewards scaled by their ages.

## Strategy Tips

- Understand B3/S23: live cell with 2 or 3 neighbors survives; dead cell with exactly 3 neighbors becomes alive
- Cell age starts at 0; cells added via `add` begin at age 0
- Cells appearing through evolution inherit age as the floor average of their three neighbors
- Tests understanding of cellular automata and iterative evolution

## Interaction Methods

| Method | Signature | Description |
|--------|-----------|-------------|
| `start` | `start() → table?` | Generates initial state table (1 = building, 0 = empty) |
| `eval` | `eval() → bool` | Executes one GOL step and verifies layout |
| `add` | `add(int x, int y) → bool` | Sets cell at coordinates to 1; costs 1 Opus |
| `remove` | `remove(int x, int y) → bool` | Sets cell at coordinates to 0; costs Opus |

---

[← Facility Overview](chapter:facility)　|　[Back to Home](chapter:main)
