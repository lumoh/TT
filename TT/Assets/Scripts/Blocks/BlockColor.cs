using UnityEngine;

public enum BlockColor
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
    public static BlockColor PickRandom(int num)
    {        
        BlockColor[] colors = (BlockColor[])System.Enum.GetValues(typeof(BlockColor));
        num = Mathf.Clamp(num, 3, colors.Length - 1);
        return colors[Random.Range(1, num + 1)];
    }
}