using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTracker : MonoBehaviour 
{
    private Text _text;
    private int _score = 0;

    void Awake() 
    {
        GameEventManager.RegisterForEvent(GameEventType.RESTART, handleRestart);
        GameEventManager.RegisterForEvent(GameEventType.BREAK_BLOCK, handleBreak);

        _score = 0;
        _text = GetComponent<Text>();
        setText();
    }

    private void handleBreak(object param)
    {
        _score += 100;
        setText();
    }

    private void handleRestart(object param)
    {
        _score = 0;
        setText();
    }

    private void setText()
    {
        if(_text != null)
        {
            _text.text = _score.ToString();
        }
    }

    void OnDestroy()
    {
        GameEventManager.UnRegisterForEvent(GameEventType.RESTART, handleRestart);
        GameEventManager.UnRegisterForEvent(GameEventType.BREAK_BLOCK, handleBreak);
    }
}
