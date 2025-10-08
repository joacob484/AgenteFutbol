using System;

namespace AF.Core
{
    public enum Position { GK, DF, MF, FW }
    public enum Region { SouthAmerica, Europe, Africa, Asia, NorthAmerica }
    public enum FacilityType { Stadium, Academy, Marketing, Medical }

    [Flags]
    public enum Personality
    {
        None = 0,
        Professional = 1 << 0,
        Ambitious    = 1 << 1,
        Temperamental= 1 << 2,
        Loyal        = 1 << 3
    }
}
