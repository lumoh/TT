using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElapsedTime : MonoBehaviour 
{
    private Text _text;
    private float _startTime;
    private bool _update;

    private int _speedUpCount;

	void Awake() 
    {
        GameEventManager.RegisterForEvent(GameEventType.RESTART, handleRestart);
        GameEventManager.RegisterForEvent(GameEventType.GAME_LOST, handleEnd);
        GameEventManager.RegisterForEvent(GameEventType.GAME_WON, handleEnd);

        _speedUpCount = 0;
        _update = true;
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

        int elapsedSeconds = Mathf.FloorToInt(Time.time - _startTime);
        if(elapsedSeconds > _speedUpCount)
        {
            _speedUpCount++;
            GameEventManager.TriggerEvent(GameEventType.NATURAL_SPEEDUP);
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
        _speedUpCount = 0;
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
