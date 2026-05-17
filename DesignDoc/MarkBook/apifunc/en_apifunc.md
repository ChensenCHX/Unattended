# Game Operation API

All functions below are callable directly from Lua scripts.

---

## `move(direction)`

Moves the unit bound to the current thread one cell in the given direction.

- **Parameters**: `direction` — Direction number (1=right 2=up 3=left 4=down)
- **Note**: Movement takes time; the unit cannot perform other actions while moving. Coordinates are auto-wrapped on the toroidal map.

---

## `build(type)`

Builds a facility of the specified type at the unit's current coordinates.

- **Parameters**: `type` — Facility type number (1=Empty, 2=Mana, 4=Ether, 8=Melodia, 16=Chronos, 32=Signum, 64=Iter, 128=Opus)
- **Note**: Build prerequisite conditions must be met. Only one Opus may exist at a time.

---

## `harvest()`

Harvests the facility at the unit's current coordinates, obtaining the corresponding resource.

- **Note**: Only harvestable when `can_harvest()` returns `true`. After harvesting, the facility degenerates to its predecessor type.

---

## `can_harvest() → bool`

Queries whether the facility at the current coordinates is ready to harvest.

- **Returns**: `true` if harvestable, `false` otherwise

---

## `use_item(type)`

Uses the specified item on the facility at the current coordinates.

- **Parameters**: `type` — Item type number (1=Mana, 2=Ether, 3=Melodia, 4=Chronos, 5=Signum, 6=Iter, 7=Opus)
- **Note**: Different facilities respond differently.

---

## `get_item_count(type) → number`

Queries the current held quantity of the specified item.

- **Parameters**: `type` — Item type number (1=Mana, 2=Ether, ...)
- **Returns**: Held count of the item

---

## `interact_with(name, ...) → ...`

Calls a facility-specific interaction method. Available methods, parameters, and return values depend on the facility type.

- **Parameters**: `name` — Method name (string); subsequent args per method signature
- **Returns**: May be boolean, integer, or nil

---

## `get_x_pos() → int`

Returns the current unit's X coordinate.

---

## `get_y_pos() → int`

Returns the current unit's Y coordinate.

---

[← API Directory](chapter:api)　|　[Back to Home](chapter:main)
