﻿using System;

public enum BlockState
{
    NONE = 0,
    SETTLED = 1,
    SWAPPING = 2,
    FALLING = 3,
    BREAKING = 4,
    COLLECTING = 5
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
