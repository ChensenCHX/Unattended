using System;

namespace Workspace.Facilities
{
    [Flags]
    public enum FacilityType
    {
        Empty = 0b00000001,
        Mana  = 0b00000010,
        Ether = 0b00000100,
        
        EtherCanBuild = 0b00000110,
    }
}