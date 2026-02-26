using System;
using System.Runtime.Serialization;
using Items;
using UnityEngine;
using Utils;
using Workspace.Facilities;

namespace GlobalSettings
{
    public static class GlobalConsts
    {
        public static readonly BoundsInt CameraBounds = new BoundsInt(-16, 1, -16, 48, 24, 48);
        public const float BotStanderYAxisValue = 1.25f;
        
        public const int MaxWorkspaceEdgeLength = 32;
        public const float BasicMoveTime = 1.0f;

        public const float ManaGrowTimeUpperBound = 0.5f;
        public const float ManaGrowTimeLowerBound = 0.2f;
        public const float EtherGrowTimeUpperBound = 8.0f;
        public const float EtherGrowTimeLowerBound = 5.0f;
        public const float MelodiaGrowTimeUpperBound = 4.8f;
        public const float MelodiaGrowTimeLowerBound = 3.2f;
        public const float ChronosGrowTimeUpperBound = 3.8f;
        public const float ChronosGrowTimeLowerBound = 2.2f;
        public const float SignumGrowTimeUpperBound = 3.2f;
        public const float SignumGrowTimeLowerBound = 2.4f;
        public const float IterGrowTimeUpperBound = 4.2f;
        public const float IterGrowTimeLowerBound = 3.5f;
        public const float OpusGrowTimeUpperBound = 8.9f;
        public const float OpusGrowTimeLowerBound = 7.2f;

        public const int ChronosSyncFrameUpperBound = 512;
        public const int ChronosSyncFrameLowerBound = 128;
        
        public const int SignumHeightUpperBound = 1;
        public const int SignumHeightLowerBound = 128 + 1;
        public const int SignumStrengthUpperBound = 1;
        public const int SignumStrengthLowerBound = 4 + 1;
        
        public const int IterEdgeCountUpperBound = 4 + 1;
        public const int IterEdgeCountLowerBound = 1;
        public const int IterEdgeWeightUpperBound = 4 + 1;
        public const int IterEdgeWeightLowerBound = 1;
        
        public const int OpusGenerateCountUpperBound = 32;
        public const int OpusGenerateCountLowerBound = 16;

    }
    public class GlobalInfos : Singleton<GlobalInfos>
    {
        public int WorkspaceEdgeLength = 8;
        public float MoveTime = GlobalConsts.BasicMoveTime * 0.05f;

        public int ManaBaseYield = 1;
        public int EtherBaseYield = 1;
        public int MelodiaBaseYield = 1;
        public int ChronosBaseYield = 1;
        public int SignumBaseYield = 1;
        public int IterBaseYield = 1;
        public int OpusBaseYield = 1;

        public event Action<double> OnManaCountChange;
        private double manaCount;
        public double ManaCount { get => manaCount; set { manaCount = value; OnManaCountChange?.Invoke(value); } }
        public event Action<double> OnEtherCountChange;
        private double etherCount;
        public double EtherCount { get => etherCount; set { etherCount = value; OnEtherCountChange?.Invoke(value); } }
        public event Action<double> OnMelodiaCountChange;
        private double melodiaCount;
        public double MelodiaCount { get => melodiaCount; set { melodiaCount = value; OnMelodiaCountChange?.Invoke(value); } }
        public event Action<double> OnChronosCountChange;
        private double chronosCount;
        public double ChronosCount { get => chronosCount; set { chronosCount = value; OnChronosCountChange?.Invoke(value); } }
        public event Action<double> OnSignumCountChange;
        private double signumCount;
        public double SignumCount { get => signumCount; set { signumCount = value; OnSignumCountChange?.Invoke(value); } }
        public event Action<double> OnIterCountChange;
        private double iterCount;
        public double IterCount { get => iterCount; set { iterCount = value; OnIterCountChange?.Invoke(value); } }
        public event Action<double> OnOpusCountChange;
        private double opusCount;
        public double OpusCount { get => opusCount; set { opusCount = value; OnOpusCountChange?.Invoke(value); } }
        
        public bool TryConsumeItem(ItemType type, int count) => type switch
        {
            ItemType.None           => false,
            ItemType.Mana           => ManaCount >= count && (ManaCount -= count) >= 0,
            ItemType.Ether          => EtherCount >= count && (EtherCount -= count) >= 0,
            ItemType.Melodia        => MelodiaCount >= count && (MelodiaCount -= count) >= 0,
            ItemType.Chronos        => ChronosCount >= count && (ChronosCount -= count) >= 0,
            ItemType.Signum         => SignumCount >= count && (SignumCount -= count) >= 0,
            ItemType.Iter           => IterCount >= count && (IterCount -= count) >= 0,
            ItemType.Opus           => OpusCount >= count && (OpusCount -= count) >= 0,
            ItemType.ItemTypeCount  => false,
            _                       => throw new NotImplementedException(),
        };
        public void SubscribeItemCountEventByType(ItemType type, Action<double> action)
        {
            switch(type)
            {
                case ItemType.None: throw new ArgumentOutOfRangeException();
                case ItemType.Mana: OnManaCountChange += action; break;
                case ItemType.Ether: OnEtherCountChange += action; break;
                case ItemType.Melodia: OnMelodiaCountChange += action; break;
                case ItemType.Chronos: OnChronosCountChange += action; break;
                case ItemType.Signum: OnSignumCountChange += action; break;
                case ItemType.Iter: OnIterCountChange += action; break;
                case ItemType.Opus: OnOpusCountChange += action; break;
                case ItemType.ItemTypeCount: throw new ArgumentOutOfRangeException();
                default: throw new NotImplementedException();
            }
        }
        public void UnsubscribeItemCountEventByType(ItemType type, Action<double> action)
        {
            switch(type)
            {
                case ItemType.None: throw new ArgumentOutOfRangeException();
                case ItemType.Mana: OnManaCountChange -= action; break;
                case ItemType.Ether: OnEtherCountChange -= action; break;
                case ItemType.Melodia: OnMelodiaCountChange -= action; break;
                case ItemType.Chronos: OnChronosCountChange -= action; break;
                case ItemType.Signum: OnSignumCountChange -= action; break;
                case ItemType.Iter: OnIterCountChange -= action; break;
                case ItemType.Opus: OnOpusCountChange -= action; break;
                case ItemType.ItemTypeCount: throw new ArgumentOutOfRangeException();
                default: throw new NotImplementedException();
            }
        }
        public double GetItemCountByType(ItemType type) => type switch
        {
            ItemType.None           => 0,
            ItemType.Mana           => ManaCount,
            ItemType.Ether          => EtherCount,
            ItemType.Melodia        => MelodiaCount,
            ItemType.Chronos        => ChronosCount,
            ItemType.Signum         => SignumCount,
            ItemType.Iter           => IterCount,
            ItemType.Opus           => OpusCount,
            ItemType.ItemTypeCount  => 0,
            _                       => throw new NotImplementedException(),
        };
        public void SetItemCountByType(ItemType type, double count)
        {
            switch (type)
            {
                case ItemType.None:             return;
                case ItemType.Mana:             ManaCount = count; return;
                case ItemType.Ether:            EtherCount = count; return;
                case ItemType.Melodia:          MelodiaCount = count; return;
                case ItemType.Chronos:          ChronosCount = count; return;
                case ItemType.Signum:           SignumCount = count; return;
                case ItemType.Iter:             IterCount = count; return;
                case ItemType.Opus:             OpusCount = count; return;
                case ItemType.ItemTypeCount:    return;
                default:                        throw new NotImplementedException();
            }
        }
        public static ItemType FacilityTypeToItemType(FacilityType type) => type switch
        {
            FacilityType.Empty      => ItemType.None,
            FacilityType.Mana       => ItemType.Mana,
            FacilityType.Ether      => ItemType.Ether,
            FacilityType.Melodia    => ItemType.Melodia,
            FacilityType.Chronos    => ItemType.Chronos,
            FacilityType.Signum     => ItemType.Signum,
            FacilityType.Iter       => ItemType.Iter,
            FacilityType.Opus       => ItemType.Opus,
            _                       => ItemType.None,
        };
        public static FacilityType ItemTypeToFacilityType(ItemType type) => type switch
        {
            ItemType.None           => FacilityType.Empty,
            ItemType.Mana           => FacilityType.Mana,
            ItemType.Ether          => FacilityType.Ether,
            ItemType.Melodia        => FacilityType.Melodia,
            ItemType.Chronos        => FacilityType.Chronos,
            ItemType.Signum         => FacilityType.Signum,
            ItemType.Iter           => FacilityType.Iter,
            ItemType.Opus           => FacilityType.Opus,
            ItemType.ItemTypeCount  => FacilityType.Empty,
            _                       => FacilityType.Empty,
        };
    }

    public delegate void PropertyChange();
}