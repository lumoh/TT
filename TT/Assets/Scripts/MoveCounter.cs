using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveCounter : MonoBehaviour 
{
    private Text _text;
    private int _moves = 0;

    void Awake() 
    {
        _moves = 0;
        GameEventManager.RegisterForEvent(GameEventType.RESTART, handleRestart);
        GameEventManager.RegisterForEvent(GameEventType.MOVE, handleMove);
        _text = GetComponent<Text>();
        setText();
    }

    private void handleMove(object param)
    {
        _moves++;
        setText();
    }

    private void handleRestart(object param)
    {
        _moves = 0;
        setText();
    }

    private void setText()
    {
        if(_text != null)
        {
            _text.text = _moves.ToString();
        }
    }

    void OnDestroy()
    {
        GameEventManager.UnRegisterForEvent(GameEventType.RESTART, handleRestart);
        GameEventManager.UnRegisterForEvent(GameEventType.MOVE, handleMove);
    }
}
