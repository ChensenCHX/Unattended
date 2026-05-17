# クイックスタート

## 概要

*Unattended* はプログラミング自動化ゲームです。Lua スクリプトを書いてユニットを制御し、グリッド島に施設を建設・収穫します。戦略を最適化して資源出力を最大化することが目標です。

## 基本 Lua 構文

初期状態で以下の構文が使用可能です：

- **変数**：`local x = 10`
- **条件分岐**：`if ... then ... else ... end`
- **ループ**：`while ... do ... end` と `for i = 1, 10 do ... end`
- **関数**：`function foo() ... end`

## 最初の自動化スクリプト

以下は Mana 施設の建設と収穫をループする簡単な例です：

```lua
while true do
    -- 右に 1 マス移動
    move(1)

    -- Mana 施設を建設（タイプ番号 2）
    build(2)

    -- 成長完了を待つ
    while not can_harvest() do
        -- 空ループ、毎フレーム自動的に yield
    end

    -- 施設を収穫
    harvest()
end
```

## 主要 API 早見表

| API | 説明 | 詳細 |
|-----|------|------|
| `move(direction)` | 指定方向へ移動（1=右 2=上 3=左 4=下） | [API](chapter:api) |
| `build(type)` | 施設を建設（2=Mana, 4=Ether, ...） | [API](chapter:api) |
| `harvest()` | 現在位置の施設を収穫 | [API](chapter:api) |
| `can_harvest()` | 収穫可能か確認 | [API](chapter:api) |
| `use_item(type)` | 施設にアイテムを使用 | [API](chapter:api) |
| `get_item_count(type)` | アイテムの所持数を照会 | [API](chapter:api) |
| `interact_with(name, ...)` | 施設のカスタムメソッドを呼出 | [API](chapter:api) |

## 上達のヒント

1. まず [Mana 施設](chapter:FacilityMana) の基本ループに慣れる
2. [施設ルール](chapter:facility) と [アイテム効果](chapter:Item) を学ぶ
3. [テクノロジーツリー](chapter:upgrade) で言語機能をアンロック
4. [API リファレンス](chapter:api) で全関数の詳細を確認

---

[ホームに戻る](chapter:main)
