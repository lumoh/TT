using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TryAgainButton : MonoBehaviour 
{    
    public TryAgainButton()
    {
        GameEventManager.RegisterForEvent(GameEventType.GAME_WON, show);
        GameEventManager.RegisterForEvent(GameEventType.GAME_LOST, show);
    }

    private void show(object param)
    {
        gameObject.SetActive(true);
    }

    public void Pressed(bool active)
    {
        gameObject.SetActive(active);
        GameEventManager.TriggerEvent(GameEventType.RESTART);
    }
}
