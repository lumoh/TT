using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goals : MonoBehaviour 
{
    public List<Goal> GoalList;
    public bool Survival;

	// Use this for initialization
    void Start()
    {
        if(Survival)
        {
            gameObject.SetActive(false);
        }
        else
        {
            GameEventManager.RegisterForEvent(GameEventType.BREAK_BLOCK, handleBreakBlock);
            GameEventManager.RegisterForEvent(GameEventType.RESTART, handleRestart);
        }
	}

    private void handleBreakBlock(object param)
    {
        Block bo = (Block)param;
        if(bo != null)
        {
            GoalCheck(bo);
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

    public void GoalCheck(Block bo)
    {
        foreach(Goal goal in GoalList)
        {
            if(goal != null && !goal.Complete)
            {
                if(goal.Color == bo.Color && goal.Type == bo.Type)
                {
                    bo.SetActive(true);
                    bo.MySpriteRenderer.sortingOrder = 10;
                    bo.State = BlockState.COLLECTING;
                    bo.transform.SetParent(null);

                    Vector3 goalPos = goal.transform.position;
                    LeanTween.scale(bo.gameObject, Vector3.one * 0.6f, 0.8f).setEase(LeanTweenType.easeInBack);
                    LeanTween.move(bo.gameObject, goalPos, 0.8f).setOnComplete(() =>
                    {
                        Destroy(bo.gameObject);
                        if(goal.Amount > 0)
                        {
                            goal.Collect();
                        }

                        if(goal.Complete)
                        {
                            if(GoalsComplete())
                            {
                                GameEventManager.TriggerEvent(GameEventType.GAME_WON);
                            }
                        }
                    }).setEase(LeanTweenType.easeInBack);
                }
            }
        }
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

    void OnDestroy()
    {
        LeanTween.cancel(gameObject);
        GameEventManager.UnRegisterForEvent(GameEventType.BREAK_BLOCK, handleBreakBlock);
        GameEventManager.UnRegisterForEvent(GameEventType.RESTART, handleRestart);
    }
}
