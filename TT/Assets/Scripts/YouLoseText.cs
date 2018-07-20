using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YouLoseText : MonoBehaviour 
{
    public YouLoseText() 
    {
        GameEventManager.RegisterForEvent(GameEventType.GAME_LOST, handleLoss);
        GameEventManager.RegisterForEvent(GameEventType.RESTART, handleRestart);
    }

    private void handleLoss(object param)
    {
        gameObject.SetActive(true);
    }

    private void handleRestart(object param)
    {
        gameObject.SetActive(false);
    }
}
