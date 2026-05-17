# スレッド管理

## 概要

Lua で `new_thread` を使用して新しいスレッドを作成します。各スレッドは 1 つのユニットに紐付けられます。マルチスレッドにより複数ユニットの同時制御が可能です。

---

## `new_thread(func) → int`

`func` を実行する新しいスレッドを作成します。呼出元スレッドの現在位置に新規ユニットが生成されます。

- **引数**：`func` — 新スレッドで実行する Lua 関数
- **戻り値**：スレッド ID（整数）、失敗時は `false`

```lua
local thread_id = new_thread(function()
    while true do
        -- 新スレッドのロジック
    end
end)
```

---

## `check_thread(id) → bool`

指定スレッドが生存中か確認します。

- **引数**：`id` — スレッド ID
- **戻り値**：生存中は `true`、終了済は `false`

---

## `hangup_current_thread()`

現在のスレッドを自発的にサスペンドし、実行権をゲームエンジンに譲渡します。スレッドは次フレームで再開されます。

---

## `get_current_thread() → int`

現在実行中のスレッド ID を返します。

---

## `get_current_frame_count() → int`

ゲーム開始からの総フレーム数を返します。タイミングロジックに有用です。

---

## `atomic_compare_and_swap_at(table, key, old, new) → old`

Lua テーブル上でアトミックな比較交換操作を実行し、スレッド間の安全な状態同期を実現します。

- **引数**：`table` — 対象テーブル、`key` — キー、`old` — 期待旧値、`new` — 新値
- **戻り値**：操作前の値

---

[← API 目次](chapter:api)　|　[ホームに戻る](chapter:main)
