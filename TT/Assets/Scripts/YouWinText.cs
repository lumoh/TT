using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YouWinText : MonoBehaviour 
{    
    public YouWinText()
    {
        GameEventManager.RegisterForEvent(GameEventType.GAME_WON, handleWin);
    }

    private void handleWin(object param)
    {
        gameObject.SetActive(true);
    }
}
