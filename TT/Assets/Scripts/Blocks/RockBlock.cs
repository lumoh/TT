﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBlock : Block 
{
    public override void Start() 
    {
        base.Start();
        GameEventManager.RegisterForEvent(GameEventType.BREAK_BLOCK, handleBreak);
    }

    private void handleBreak(object param)
    {
        Block bo = (Block)param;
        if(bo != null && bo.Type == BlockType.Block && isAdjacent(bo.X, bo.Y))
        {
            Break(0f);
        }
    }

    private bool isAdjacent(int x, int y)
    {
        bool isAdj = false;
        if((Mathf.Abs(x - X) == 1 && Mathf.Abs(y - Y) == 0) || (Mathf.Abs(x - X) == 0 && Mathf.Abs(y - Y) == 1))
        {
            isAdj = true;
        }
        return isAdj;
    }
	
    public override void OnDestroy()
    {
        base.OnDestroy();
        GameEventManager.UnRegisterForEvent(GameEventType.BREAK_BLOCK, handleBreak);
    }
}
