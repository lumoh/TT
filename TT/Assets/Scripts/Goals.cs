using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goals : MonoBehaviour 
{
    public List<Goal> GoalList;

	// Use this for initialization
    void Start()
    {
        GameEventManager.RegisterForEvent(GameEventType.BREAK_BLOCK, handleBreakBlock);
        GameEventManager.RegisterForEvent(GameEventType.RESTART, handleRestart);
	}

    private void handleBreakBlock(object param)
    {
        BoardObject bo = (BoardObject)param;
        if(bo != null)
        {
            GoalCheck(bo);
        }

        if(GoalsComplete())
        {
            GameEventManager.TriggerEvent(GameEventType.GAME_WON);
        }
    }

    private void handleRestart(object param)
    {
        foreach(Goal goal in GoalList)
        {
            if(goal != null)
            {
                goal.Reset();
            }
        }
    }

    public bool GoalCheck(BoardObject bo)
    {
        foreach(Goal goal in GoalList)
        {
            if(goal != null && !goal.Complete)
            {
                if(goal.Color == bo.Color && goal.Type == bo.Type)
                {
                    goal.Collect();
                    if(goal.Amount == 0)
                    {
                        goal.Complete = true;
                    }
                    return true;
                }
            }
        }
        return false;
    }

    public bool GoalsComplete()
    {
        bool complete = true;
        foreach(Goal goal in GoalList)
        {
            if(goal != null && !goal.Complete)
            {
                complete = false;
                break;
            }
        }
        return complete;
    }
}
