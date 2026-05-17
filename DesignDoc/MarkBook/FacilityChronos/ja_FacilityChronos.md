# FacilityChronos

## 基本情報

| 項目 | 値 |
|------|-----|
| 資源 | Chronos |
| 建設コスト | Melodia |
| 退化先 | Ether |
| 建設前提 | Ether ベースの区画 |

## コアルール

Chronos は**タイミング精度**を中心に設計されています。`interact_with("start", tolerance)` を呼ぶと、ターゲットアイテム種別とフレーム枠が返されます。プレイヤーは `[Framecount − Tolerance, Framecount + Tolerance]` フレーム内に `use_item(targetItem)` を実行する必要があります。成功すると増産状態になります。

`interact_with("check")` で現在の状態を照会可能です。増産状態の収量は基本状態より大幅に高くなります。

## 戦略のヒント

- フレーム枠は通常長めです。待機中に他のタスクを実行し、枠が近づいたら戻って操作を確定させましょう
- シングル/マルチスレッドの I/O タスクスケジューリング能力が試されます
- 増産状態での再 `start` 呼出は無効です
- フレームカウントは `start` 呼出時点から開始されます

## インタラクションメソッド

| メソッド | シグネチャ | 説明 |
|----------|-----------|------|
| `start` | `start(int tolerance) → ItemType, int` | ターゲットアイテム種別とフレーム枠値を返す |
| `check` | `check() → string` | 現在の状態を返す：`"init"`, `"waiting"`, `"success"`, `"fail"` |

---

[← 施設概要](chapter:facility)　|　[ホームに戻る](chapter:main)
