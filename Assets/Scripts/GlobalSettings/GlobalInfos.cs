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
    }
    public class GlobalInfos : Singleton<GlobalInfos>
    {
        public int WorkspaceEdgeLength = 16;
        public float MoveTime = GlobalConsts.BasicMoveTime;
        
    }
}