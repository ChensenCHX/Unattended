# Item Effects

The game includes seven item types, one per facility type. In addition to being consumed for tech tree unlocks, each item has passive buffs and/or active usage effects.

## Item Effect Summary

| Item | Passive (while held) | Active (`use_item`) |
|------|----------------------|---------------------|
| **Mana** | None | — |
| **Ether** | None | — |
| **Melodia** | Auto-consumed to boost instruction execution per frame | — |
| **Chronos** | None | Used on a facility to accelerate its growth |
| **Signum** | Auto-consumed to raise the per-frame instruction cap | — |
| **Iter** | Auto-consumed to reduce unit movement time | — |
| **Opus** | None | Used on a facility to instantly complete its growth |

## Notes

- Passive effects activate automatically while holding the resource; consumption rate depends on unlocked tech levels
- Confirm the target facility at your unit's current coordinates before calling `use_item(type)`
- Failed item usage (facility rejects the item or insufficient quantity) does not consume the item

---

[Back to Home](chapter:main)
