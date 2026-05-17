# FacilitySignum

## Basic Info

| Property | Value |
|----------|-------|
| Resource | Signum |
| Build Cost | Ether + Melodia |
| Degenerates To | Ether |
| Build Prerequisite | Plot based on Ether |

## Core Rule

Signum revolves around **signal transmission**. Each Signum has two attributes:

- **Height**, range 1–128
- **Strength**, range 1–4

Signals are emitted in four directions along the grid. A signal is received by the first Signum encountered whose Height ≥ the emitter's Height, at which point the signal stops propagating. Other facility types have an effective Height of 0.

Harvesting triggers a chain harvest of all source Signum facilities whose signals were received. Yield = base yield × chain count² × (sum of received Strengths + own Strength).

## Strategy Tips

- Adjust Height to reshape signal network topology
- Destroy intermediate facilities to reroute blocked signal paths
- Call `interact_with("detach")` to isolate a facility from the network (base yield only after detach)
- Tests understanding of signal propagation rules and network topology

## Interaction Methods

| Method | Signature | Description |
|--------|-----------|-------------|
| `get_height` | `get_height() → int` | Returns current Height (1–128) |
| `get_strength` | `get_strength() → int` | Returns current Strength (1–4) |
| `detach` | `detach() → void` | Detaches from the signal network; base yield only |

---

[← Facility Overview](chapter:facility)　|　[Back to Home](chapter:main)
