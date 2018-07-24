using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUp : BetterButton 
{
	// Use this for initialization
	protected override void Start() 
    {
        onButtonDown.AddListener(() =>
        {
            GameEventManager.TriggerEvent(GameEventType.SPEEDUP_DOWN);
        });

        onButtonUp.AddListener(() =>
        {
            GameEventManager.TriggerEvent(GameEventType.SPEEDUP_UP);
        });

        base.Start();
	}
}
