using System;
using System.Collections.Generic;

/// <summary>
/// All game event types
/// </summary>
public enum GameEventType
{
    GAME_WON,
    GAME_LOST,
    BREAK_BLOCK,
    RESTART,
    MOVE,
    SPEEDUP_DOWN,
    SPEEDUP_UP
};

/// <summary>
/// World Event Manager.
/// Global event system with null target checks and auto removal, and single fire events that do not need unregistering.
/// </summary>
public static class GameEventManager
{
    private class WEContainer
	{
		/// <summary>
		/// Registered callbacks
		/// </summary>
        public SGNEvent<Object> WorldEvent = new SGNEvent<Object>();

		/// <summary>
		/// Single fire callbacks
		/// </summary>
        public SGNEvent<Object> SingleWorldEvent = new SGNEvent<Object>();

		/// <summary>
		/// Trigger this container's events
		/// </summary>
		public void Trigger(object param = null)
		{
			if (WorldEvent != null)
			{
                WorldEvent.Invoke(param);
			}

			if (SingleWorldEvent != null)
			{
				Action<Object>[] cbArray = SingleWorldEvent.GetInvocationList();
                SingleWorldEvent.Clear();

				// manually call cached events, as we have to pre-clear the event container in anticipation of new events being added.
                if (cbArray != null && cbArray.Length > 0)
				{
                    for(int i=0; i<cbArray.Length; i++)
					{
                        bool valid = cbArray[i] != null;
                        if (valid)
                        {
                            bool isStatic = (cbArray[i].Method.Attributes & System.Reflection.MethodAttributes.Static) == System.Reflection.MethodAttributes.Static;
                            if (!isStatic && (cbArray[i].Target == null || cbArray[i].Target.Equals(null)))
                            {
                                valid = false;
                            }
                        }

                        if (valid)
                        {                            
                            cbArray[i](param);
                        }
					}
				}
			}
		}

        /// <summary>
        /// Scan event for null targets
        /// </summary>
        /// <returns>The cleanup.</returns>
        public bool CheckNullTargets()
        {
            bool hasNull = false;
            if (WorldEvent != null)
            {
                Action<Object>[] cbArray = WorldEvent.GetInvocationList();
                if (cbArray != null && cbArray.Length > 0)
                {
                    for(int i=0; i<cbArray.Length; i++)
                    {
                        bool valid = cbArray[i] != null;
                        if (valid)
                        {
                            bool isStatic = (cbArray[i].Method.Attributes & System.Reflection.MethodAttributes.Static) == System.Reflection.MethodAttributes.Static;
                            if (!isStatic && (cbArray[i].Target == null || cbArray[i].Target.Equals(null)))
                            {
                                valid = false;
                            }
                        }

                        if (!valid)
                        {
                            hasNull = true;
                            WorldEvent.Clean();
                            break;
                        }
                    }
                }
            }
            return hasNull;
        }

        /// <summary>
		/// Determines whether this container instance is empty
		/// </summary>
		/// <returns><c>true</c> if this instance is empty the specified wec; otherwise, <c>false</c>.</returns>
		/// <param name="wec">Wec.</param>
		public bool IsEmpty()
		{
			bool result = false;
            if (WorldEvent == null || WorldEvent.Count == 0)
			{
                if (SingleWorldEvent == null || SingleWorldEvent.Count == 0)
				{
					result = true;
				}
			}
			return result;
		}

	};
	
	/// <summary>
	/// The _world events.
	/// </summary>
	private static Dictionary<GameEventType, WEContainer> _worldEvents = new Dictionary<GameEventType, WEContainer>(); 

	/***/

	/// <summary>
	/// Register for an event
	/// </summary>
	/// <param name="eventType">Event type.</param>
	/// <param name="eventCB">Event C.</param>
	public static void RegisterForEvent(GameEventType eventType, Action<Object> eventCB)
	{
		if (!_worldEvents.ContainsKey(eventType))
		{
			_worldEvents.Add(eventType, new WEContainer());
		}

		if (_worldEvents.ContainsKey(eventType))
		{
			var wec = _worldEvents[eventType];
			if (wec != null)
			{
                wec.WorldEvent.Register(eventCB);
			}
		}
	}
	
	/// <summary>
	/// Unregister for an event
	/// </summary>
	/// <param name="eventType">Event type.</param>
    public static void UnRegisterForEvent(GameEventType eventType, Action<Object> eventCB)
	{
		if (_worldEvents.ContainsKey(eventType))
		{
			var wec = _worldEvents[eventType];
			if (wec != null)
			{
                wec.WorldEvent.UnRegister(eventCB);
                wec.SingleWorldEvent.UnRegister(eventCB);
				if (wec.IsEmpty())
				{
					_worldEvents.Remove(eventType);
				}
			}
		}
	}

	/// <summary>
	/// Register for an event for a single callback only, unregisters automatically.
	/// </summary>
	/// <param name="eventType">Event type.</param>
	/// <param name="eventCB">Event C.</param>
    public static void QueueForEvent(GameEventType eventType, Action<Object> eventCB)
	{
		if (!_worldEvents.ContainsKey(eventType))
		{
			_worldEvents.Add(eventType, new WEContainer());
		}

		if (_worldEvents.ContainsKey(eventType))
		{
			var wec = _worldEvents[eventType];
			if (wec != null)
			{
                wec.SingleWorldEvent.Register(eventCB);
			}
		}
	}

	/// <summary>
	/// Fire off an event
	/// </summary>
	/// <param name="eventType">Event type.</param>
	public static void TriggerEvent(GameEventType eventType, object param = null)
	{
		if (_worldEvents.ContainsKey(eventType))
		{
			var wec = _worldEvents[eventType];
			if (wec != null)
			{
				wec.Trigger(param);
			}
		}
	}

    /// <summary>
    /// Check null targets and warn.
    /// </summary>
    public static void CheckTargetsAndWarn()
    {
        if (_worldEvents != null)
        {
            foreach(var entry in _worldEvents)
            {
                var wec = entry.Value;
                if (wec != null && wec.CheckNullTargets())
                {
                    UnityEngine.Debug.Log("WEM: Null target found on " + entry.Key.ToString());
                }
            }
        }
    }
}
