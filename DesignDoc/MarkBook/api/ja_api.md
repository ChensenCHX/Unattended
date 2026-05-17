# API リファレンス

全てのゲーム内操作は Lua スクリプトから API 関数を呼び出して実行します。以下はカテゴリ別の索引です。

---

## API カテゴリ

- **[スレッド管理](chapter:thread)** — スレッド作成、状態確認、サスペンド
- **[ゲーム操作](chapter:apifunc)** — 移動、建設、収穫、アイテム使用、施設インタラクション

---

## 早見表

| よく使う API | 機能 | カテゴリ |
|-------------|------|---------|
| `move(direction)` | 指定方向へ移動（1=右 2=上 3=左 4=下） | [ゲーム操作](chapter:apifunc) |
| `build(type)` | 施設を建設（2=Mana, 4=Ether, ...） | [ゲーム操作](chapter:apifunc) |
| `harvest()` | 現在位置の施設を収穫 | [ゲーム操作](chapter:apifunc) |
| `can_harvest()` | 収穫可能か確認 | [ゲーム操作](chapter:apifunc) |
| `use_item(type)` | 施設にアイテムを使用 | [ゲーム操作](chapter:apifunc) |
| `get_item_count(type)` | アイテムの所持数を照会 | [ゲーム操作](chapter:apifunc) |
| `interact_with(name, ...)` | 施設のカスタムメソッドを呼出 | [ゲーム操作](chapter:apifunc) |
| `get_x_pos()` | ユニットの X 座標を取得 | [ゲーム操作](chapter:apifunc) |
| `get_y_pos()` | ユニットの Y 座標を取得 | [ゲーム操作](chapter:apifunc) |
| `new_thread(func)` | 新規スレッドを作成 | [スレッド](chapter:thread) |
| `check_thread(id)` | スレッドの生存状態を確認 | [スレッド](chapter:thread) |
| `hangup_current_thread()` | 現在のスレッドをサスペンド | [スレッド](chapter:thread) |
| `get_current_thread()` | 現在のスレッド ID を取得 | [スレッド](chapter:thread) |

---

[ホームに戻る](chapter:main)
