# FacilityIter

## Basic Info

| Property | Value |
|----------|-------|
| Resource | Iter |
| Build Cost | Ether + Signum |
| Degenerates To | Ether |
| Build Prerequisite | Plot based on Signum |

## Core Rule

FacilityIter revolves around **graph connectivity optimization**. Upon construction, it randomly establishes weighted edges with other Iter facilities, forming an undirected graph. Yield = base yield × chain harvest count³ / sum of all active edge weights.

Players can adjust the graph via:
- `connect(x, y)`: establish a connection to Iter at (x, y)
- `disconnect(x, y)`: remove connection to Iter at (x, y)

Harvesting triggers a chain harvest of all directly or indirectly connected Iter facilities.

## Strategy Tips

- Edge weight directly affects yield; higher-weight edges drag down total output
- Balance between more active edges (higher chain count) and lower weight sum (smaller denominator)
- Disconnect high-weight edges and reconnect to more optimal positions
- Tests graph theory concepts: connectivity, edge weight optimization, spanning trees

## Interaction Methods

| Method | Signature | Description |
|--------|-----------|-------------|
| `get_edges` | `get_edges() → table` | Returns `{x=[...], y=[...], weight=[...], state=[...]}` — four parallel arrays |
| `connect` | `connect(int x, int y) → bool` | Attempt to connect to Iter at coordinates; returns true on success |
| `disconnect` | `disconnect(int x, int y) → bool` | Attempt to disconnect from Iter at coordinates; returns true on success |

---

[← Facility Overview](chapter:facility)　|　[Back to Home](chapter:main)
