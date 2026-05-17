# FacilitySignum

## 基本情報

| 項目 | 値 |
|------|-----|
| 資源 | Signum |
| 建設コスト | Ether + Melodia |
| 退化先 | Ether |
| 建設前提 | Ether ベースの区画 |

## コアルール

Signum は**信号伝送**を中心に設計されています。各 Signum は 2 つの属性を持ちます：

- **Height**（高さ）、範囲 1～128
- **Strength**（強度）、範囲 1～4

信号は 4 方向にグリッド沿いに発信されます。信号は自身の Height 以上の Height を持つ最初の Signum に受信され、伝播を停止します。他施設種別の Height は 0 とみなされます。

収穫時、信号を受信した全ての送信元 Signum が連鎖収穫されます。収量 = 基本収量 × 連鎖数² ×（受信 Strength の合計 + 自身の Strength）。

## 戦略のヒント

- Height を調整して信号ネットワークのトポロジを再構築する
- 中間施設を破壊してブロックされた信号経路を再ルーティングする
- `interact_with("detach")` で施設をネットワークから分離可能（分離後は基本収量のみ）
- 信号伝播ルールとネットワークトポロジの理解が試される

## インタラクションメソッド

| メソッド | シグネチャ | 説明 |
|----------|-----------|------|
| `get_height` | `get_height() → int` | 現在の Height を返す（1～128） |
| `get_strength` | `get_strength() → int` | 現在の Strength を返す（1～4） |
| `detach` | `detach() → void` | 信号ネットワークから分離、基本収量のみ |

---

[← 施設概要](chapter:facility)　|　[ホームに戻る](chapter:main)
