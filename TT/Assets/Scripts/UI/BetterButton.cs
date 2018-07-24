using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BetterButton : Button 
{
    public UnityEvent onButtonUp = new UnityEvent();
    public UnityEvent onButtonDown = new UnityEvent();
    protected bool _down = false;

    protected override void Start()
    {
        base.Start();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(!_down)
        {
            _down = true;
            base.OnPointerDown(eventData);
            onButtonDown.Invoke();
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if(_down)
        {
            _down = false;
            base.OnPointerUp(eventData);
            onButtonUp.Invoke();
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if(_down)
        {
            _down = false;
            base.OnPointerExit(eventData);
            onButtonUp.Invoke();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        onButtonDown.RemoveAllListeners();
        onButtonUp.RemoveAllListeners();
    }
}