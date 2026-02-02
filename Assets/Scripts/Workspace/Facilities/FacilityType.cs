using System;

namespace Workspace.Facilities
{
    [Flags]
    public enum FacilityType
    {
        Empty               = 0b00000001,
        Mana                = 0b00000010,
        Ether               = 0b00000100,
        Melodia             = 0b00001000,
        Chronos             = 0b00010000,
        Signum              = 0b00100000,
        Iter                = 0b01000000,
        Opus                = 0b10000000,
        
        EmptyCanBuild       = 0b11111111,
        ManaCanBuild        = 0b11111111,
        EtherCanBuild       = 0b11111110,
        MelodiaCanBuild     = 0b11111110,
        ChronosCanBuild     = 0b11111100,
        SignumCanBuild      = 0b11111100,
        IterCanBuild        = 0b11100000,
        OpusCanBuild        = 0b11111111,
    }
}