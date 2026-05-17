# Facility Overview

*Unattended* includes seven facility types arranged in a progressive build chain. Each facility has unique resource rules and algorithmic mechanics.

## Facility List

| Facility | Resource | Core Rule | Details |
|----------|----------|-----------|---------|
| Mana | Mana | Basic growth and harvest, no special rules | [→](chapter:FacilityMana) |
| Ether | Ether | Yield affected by adjacent same-type facilities | [→](chapter:FacilityEther) |
| Melodia | Melodia | Non-repeating attributes grant increasing bonus | [→](chapter:FacilityMelodia) |
| Chronos | Chronos | Trigger bonus by operating within a time window | [→](chapter:FacilityChronos) |
| Signum | Signum | Signal transmission; yield depends on network size | [→](chapter:FacilitySignum) |
| Iter | Iter | Undirected weighted graph; optimize by adjusting edges | [→](chapter:FacilityIter) |
| Opus | Opus + chain | Conway's Game of Life; evolution triggers chain yield | [→](chapter:FacilityOpus) |

## Build Dependency

Facilities follow a strict build chain from low to high:

**Empty → Mana → Ether → Melodia → Chronos → Signum → Iter → Opus**

Each facility can only be built on its designated predecessor. Building a lower-tier facility on a higher one is allowed.

---

[Back to Home](chapter:main)
