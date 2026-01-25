using System;
using System.Runtime.Serialization;
using Utils;

namespace GlobalSettings
{
    public static class GlobalConsts
    {
        public const float BotStanderYAxisValue = 1.25f;
        
        public const int MaxWorkspaceEdgeLength = 32;
        public const float BasicMoveTime = 1.0f;

        public const float ManaGrowTimeUpperBound = 0.2f;
        public const float ManaGrowTimeLowerBound = 0.5f;
        public const float EtherGrowTimeUpperBound = 8.0f;
        public const float EtherGrowTimeLowerBound = 5.0f;

    }
    public class GlobalInfos : Singleton<GlobalInfos>
    {
        public int WorkspaceEdgeLength = 32;
        public float MoveTime = GlobalConsts.BasicMoveTime;

        public int ManaBaseYield = 1;
        public int EtherBaseYield = 1;

        public double ManaCount = 0.0;
        public double EtherCount = 0.0;
        
        
    }
}