using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElapsedTime : MonoBehaviour 
{
    private Text _text;
    private float _startTime;
    private bool _update;

	void Awake() 
    {
        _update = true;
        GameEventManager.RegisterForEvent(GameEventType.RESTART, handleRestart);
        GameEventManager.RegisterForEvent(GameEventType.GAME_LOST, handleEnd);
        GameEventManager.RegisterForEvent(GameEventType.GAME_WON, handleEnd);
        _text = GetComponent<Text>();
        _startTime = Time.time;
        setText();
	}

    void Update()
    {
        if(_update)
        {
            setText();
        }
    }

    private void setText()
    {
        if(_text != null)
        {
            float elapsedTime = Time.time - _startTime;

            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime - minutes * 60);
            string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);
            _text.text = formattedTime;
        }
    }

    private void handleRestart(object param)
    {
        _update = true;
        _startTime = Time.time;
        setText();
    }

    private void handleEnd(object param)
    {
        _update = false;
    }

    void OnDestroy()
    {
        GameEventManager.UnRegisterForEvent(GameEventType.RESTART, handleRestart);
        GameEventManager.UnRegisterForEvent(GameEventType.GAME_LOST, handleEnd);
        GameEventManager.UnRegisterForEvent(GameEventType.GAME_WON, handleEnd);
    }
}
