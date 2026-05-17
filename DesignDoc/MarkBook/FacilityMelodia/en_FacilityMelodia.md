# FacilityMelodia

## Basic Info

| Property | Value |
|----------|-------|
| Resource | Melodia |
| Build Cost | Ether |
| Degenerates To | Mana |
| Build Prerequisite | Plot based on Mana |

## Core Rule

Each Melodia facility holds a **Tone value**, ranging 0–31. When harvesting Melodia, if the current tone differs from the previous one, a progressively increasing bonus multiplier is granted. If the tone repeats, the multiplier resets. The multiplier cap is 16×.

## Strategy Tips

- Record historical tone sequences to avoid repeats that reset the bonus
- Query tones via `interact_with("get_tone")` to plan harvest order in advance
- When the queue reaches 32 entries or a repeat occurs, it is cleared automatically — no manual reset needed
- Tests understanding of data structures and sorting algorithms

## Interaction Methods

| Method | Signature | Description |
|--------|-----------|-------------|
| `get_tone` | `get_tone() → int` | Returns the facility's current tone (0–31) |
| `reset` | `reset() → void` | Clears the global tone history queue |

---

[← Facility Overview](chapter:facility)　|　[Back to Home](chapter:main)
