using System;

namespace Unchord
{
    [Flags]
    public enum SpawnPositionFlag : uint
    {
        None = 0x00000000,

        OutOfL = 0x00000001,
        OutOfT = 0x00000002,
        OutOfR = 0x00000004,
        OutOfB = 0x00000008,

        QuarterOfL = 0x00000010,
        QuarterOfT = 0x00000020,
        QuarterOfR = 0x00000040,
        QuarterOfB = 0x00000080,

        HalfOfL = 0x00000100,
        HalfOfT = 0x00000200,
        HalfOfR = 0x00000400,
        HalfOfB = 0x00000800,

        OriginOfMap = 0x00001000,
        RandomOfMap = 0x00002000
    }
}