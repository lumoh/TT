using UnityEngine;

public enum BoardObjectColor
{
    NONE = 0,
    RED = 1,
    TEAL = 2,
    YELLOW = 3,
    GREEN = 4,
    BLUE = 5,
    PURPLE = 6
}

public class ColorUtil
{   
    public static BoardObjectColor PickRandom(int num)
    {        
        BoardObjectColor[] colors = (BoardObjectColor[])System.Enum.GetValues(typeof(BoardObjectColor));
        num = Mathf.Clamp(num, 3, colors.Length - 1);
        return colors[Random.Range(1, num + 1)];
    }
}