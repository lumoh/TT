using UnityEngine;

public enum BoardObjectColor
{
    NONE = 0,
    RED = 1,
    ORANGE = 2,
    YELLOW = 3,
    GREEN = 4,
    BLUE = 5,
    PURPLE = 6
}

public class ColorUtil
{   
    public static BoardObjectColor PickRandom()
    {
        BoardObjectColor[] colors = (BoardObjectColor[])System.Enum.GetValues(typeof(BoardObjectColor));
        return colors[Random.Range(3, colors.Length)];
    }
}