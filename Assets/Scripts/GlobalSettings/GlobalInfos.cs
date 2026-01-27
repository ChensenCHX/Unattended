using System;
using System.Runtime.Serialization;
using Items;
using Utils;

namespace GlobalSettings
{
    public static class GlobalConsts
    {
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
        
        public const int ChronosSyncFrameUpperBound = 512;
        public const int ChronosSyncFrameLowerBound = 128;
    }
    public class GlobalInfos : Singleton<GlobalInfos>
    {
        public int WorkspaceEdgeLength = 32;
        public float MoveTime = GlobalConsts.BasicMoveTime;

        public int ManaBaseYield = 1;
        public int EtherBaseYield = 1;
        public int MelodiaBaseYield = 1;
        public int ChronosBaseYield = 1;
        public int SignumBaseYield = 1;
        public int IterBaseYield = 1;
        public int OpusBaseYield = 1;

        public double ManaCount = 10.0;
        public double EtherCount = 10.0;
        public double MelodiaCount = 10.0;
        public double ChronosCount = 10.0;
        public double SignumCount = 0.0;
        public double IterCount = 0.0;
        public double OpusCount = 0.0;

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
    }
}