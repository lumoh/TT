using UnityEngine;

public enum BlockColor
{
    None = 0,
    Red = 1,
    Teal = 2,
    Yellow = 3,
    Green = 4,
    Blue = 5,
    Purple = 6
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