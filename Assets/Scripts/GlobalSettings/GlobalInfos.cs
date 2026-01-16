using System;
using System.Runtime.Serialization;
using Utils;

namespace GlobalSettings
{
    public class GlobalInfos : Singleton<GlobalInfos>
    {
        public const int MaxWorkspaceEdgeLength = 32;
        public int WorkspaceEdgeLength = 1;
    }
}