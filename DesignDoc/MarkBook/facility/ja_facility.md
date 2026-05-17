# 施設概要

*Unattended* には 7 種の施設があり、段階的な建設チェーンを形成しています。各施設は独自の生産ルールとアルゴリズム機構を持ちます。

## 施設一覧

| 施設 | 資源 | コアルール | 詳細 |
|------|------|-----------|------|
| Mana | Mana | 基本成長と収穫、特殊ルールなし | [→](chapter:FacilityMana) |
| Ether | Ether | 隣接する同種施設の数が収量に影響 | [→](chapter:FacilityEther) |
| Melodia | Melodia | 属性値の非重複が増加ボーナス | [→](chapter:FacilityMelodia) |
| Chronos | Chronos | 時間枠内の操作で増産をトリガー | [→](chapter:FacilityChronos) |
| Signum | Signum | 信号伝送、収量はネットワーク規模に依存 | [→](chapter:FacilitySignum) |
| Iter | Iter | 無向加重グラフ、辺の調整で最適化 | [→](chapter:FacilityIter) |
| Opus | Opus + 連鎖 | コンウェイのライフゲーム、進化が連鎖収穫 | [→](chapter:FacilityOpus) |

## 建設依存関係

施設は厳格な建設チェーンに従います：

**Empty → Mana → Ether → Melodia → Chronos → Signum → Iter → Opus**

各施設は指定された前提施設の上にのみ建設可能です。上位施設に下位施設を建設することは許可されています。

---

[ホームに戻る](chapter:main)
