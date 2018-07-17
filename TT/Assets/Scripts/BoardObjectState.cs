using System;

public enum BoardObjectState
{
    NONE = 0,
    SETTLED = 1,
    SWAPPING = 2,
    FALLING = 3,
    BREAKING = 4
}

public enum FallDir
{
    NORTH,
    SOUTH,
    EAST,
    WEST,
    NORTHWEST,
    NORTHEAST,
    SOUTHWEST,
    SOUTHEAST
}
