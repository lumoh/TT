using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElapsedTime : MonoBehaviour 
{
    private Text _text;
    private float _startTime;

	void Awake() 
    {
        GameEventManager.RegisterForEvent(GameEventType.RESTART, handleRestart);
        _text = GetComponent<Text>();
        _startTime = Time.time;
        setText();
	}

    void Update()
    {
        setText();
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
        _startTime = Time.time;
        setText();
    }

    void OnDestroy()
    {
        GameEventManager.UnRegisterForEvent(GameEventType.RESTART, handleRestart);
    }
}
