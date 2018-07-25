using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row : Block 
{
    protected override void customBreakEffect()
    {
        if(MyBoard != null)
        {
            float dist = 1f;
            for(int x = X + 1; x < MyBoard.Width; x++)
            {
                Block b = MyBoard.GetBoardObject(x, Y);
                if(b != null)
                {
                    float delay = 1.2f + (dist * 0.1f);
                    b.Break(delay);
                }
                dist++;
            }

            dist = 1f;
            for(int x = X - 1; x >= 0; x--)
            {
                Block b = MyBoard.GetBoardObject(x, Y);
                if(b != null)
                {
                    float delay = 1.2f + (dist * 0.1f);
                    b.Break(delay);
                }
                dist++;
            }
        }
    }
}
