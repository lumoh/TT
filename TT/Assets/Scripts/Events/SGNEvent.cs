using System;
using System.Collections.Generic;

/// <summary>
/// Event class to reduce boilerplate register-unregister code.  No args version.
/// </summary>
public class SGNEvent
{
    /// <summary>
    /// Event callbacks
    /// </summary>
    private List<Action> _callbacks = null;

    /// <summary>
    /// Cache of the above list as an array
    /// </summary>
    private Action[] _invocationList = null;

    /// <summary>
    /// Return the number of registered callbacks
    /// </summary>
    /// <value>The count.</value>
    public int Count
    {
        get
        {
            int count = 0;
            if (_callbacks != null)
            {
                count = _callbacks.Count;
            }
            return count;
        }
    }

    /// <summary>
    /// Registers for the event.
    /// </summary>
    /// <param name="handler">The event handler</param>
    public void Register(Action handler)
    {
        if (_callbacks == null)
        {
            _callbacks = new List<Action>();
        }

        if (!_callbacks.Contains(handler))
        {
            _callbacks.Add(handler);
            _invocationList = null;
        }
    }
    
    /// <summary>
    /// Unregisters for the event.
    /// </summary>
    /// <param name="handler">The event handler</param>
    public void UnRegister(Action handler)
    {
        if (_callbacks != null && _callbacks.Contains(handler))
        {
            _callbacks.Remove(handler);
            _invocationList = null;
        }
    }
    
    /// <summary>
    /// Invokes the event.
    /// </summary>
    public void Invoke()
    {
        if (_callbacks != null)
        {
            bool rebuild = false;
            Action[] cbArray = GetInvocationList();
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
                    cbArray[i]();
                }
                else
                {
                    rebuild = true;
                }
            }

            // rebuild the callback list removing the null entries
            if (rebuild)
            {
                Clean();
            }
        }
    }

    /// <summary>
    /// Rebuilds the invocation list removing null targets.
    /// </summary>
    public void Clean()
    {
        if (_callbacks != null)
        {
            var cbArray = GetInvocationList();
            _callbacks.Clear();
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
                    _callbacks.Add(cbArray[i]);
                }
            }
            _invocationList = null;
        }
    }

    /// <summary>
    /// Return an array of all registered actions
    /// </summary>
    /// <returns>The invocation list.</returns>
    public Action[] GetInvocationList()
    {
        if (_invocationList == null && _callbacks != null)
        {
            _invocationList = _callbacks.ToArray();
        }
        return _invocationList;
    }

    /// <summary>
    /// Clears delegates from this instance.
    /// </summary>
    public void Clear()
    {
        if (_callbacks != null)
        {
            _callbacks.Clear();
        }
        _invocationList = null;
    }
}


/// <summary>
/// Event class to reduce boilerplate register-unregister code.  Takes one argument.
/// </summary> 
public class SGNEvent<T>
{
    /// <summary>
    /// Event callbacks
    /// </summary>
    private List<Action<T>> _callbacks = null;

    /// <summary>
    /// Cache of the above list as an array
    /// </summary>
    private Action<T>[] _invocationList = null;

    /// <summary>
    /// Return the number of registered callbacks
    /// </summary>
    /// <value>The count.</value>
    public int Count
    {
        get
        {
            int count = 0;
            if (_callbacks != null)
            {
                count = _callbacks.Count;
            }
            return count;
        }
    }

    /// <summary>
    /// Registers for the event.
    /// </summary>
    /// <param name="handler">The event handler</param>
    public void Register(Action<T> handler)
    {
        if (_callbacks == null)
        {
            _callbacks = new List<Action<T>>();
        }

        if (!_callbacks.Contains(handler))
        {
            _callbacks.Add(handler);
            _invocationList = null;
        }
    }
    
    /// <summary>
    /// Unregisters for the event.
    /// </summary>
    /// <param name="handler">The event handler</param>
    public void UnRegister(Action<T> handler)
    {
        if (_callbacks != null && _callbacks.Contains(handler))
        {
            _callbacks.Remove(handler);
            _invocationList = null;
        }
    }
    
    /// <summary>
    /// Invokes the event
    /// </summary>
    /// <param name="argument">The type argument.</param>
    public void Invoke(T argument)
    {
        if (_callbacks != null)
        {
            bool rebuild = false;
            Action<T>[] cbArray = GetInvocationList();
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
                    cbArray[i](argument);
                }
                else
                {
                    rebuild = true;
                }
            }

            // rebuild the callback list removing the null entries
            if (rebuild)
            {
                Clean();
            }
        }
    }

    /// <summary>
    /// Rebuilds the invocation list removing null targets.
    /// </summary>
    public void Clean()
    {
        if (_callbacks != null)
        {
            var cbArray = GetInvocationList();
            _callbacks.Clear();
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
                    _callbacks.Add(cbArray[i]);
                }
            }
            _invocationList = null;
        }
    }

    /// <summary>
    /// Return an array of all registered actions
    /// </summary>
    /// <returns>The invocation list.</returns>
    public Action<T>[] GetInvocationList()
    {
        if (_invocationList == null && _callbacks != null)
        {
            _invocationList = _callbacks.ToArray();
        }
        return _invocationList;
    }

    /// <summary>
    /// Clears delegates from this instance.
    /// </summary>
    public void Clear()
    {
        if (_callbacks != null)
        {
            _callbacks.Clear();
        }
        _invocationList = null;
    }
}

/// <summary>
/// Event class to reduce boilerplate register-unregister code.  Takes two arguments.
/// </summary> 
public class SGNEvent<T, U>
{
    /// <summary>
    /// Event callbacks
    /// </summary>
    private List<Action<T,U>> _callbacks = null;

    /// <summary>
    /// Cache of the above list as an array
    /// </summary>
    private Action<T,U>[] _invocationList = null;

    /// <summary>
    /// Return the number of registered callbacks
    /// </summary>
    /// <value>The count.</value>
    public int Count
    {
        get
        {
            int count = 0;
            if (_callbacks != null)
            {
                count = _callbacks.Count;
            }
            return count;
        }
    }

    /// <summary>
    /// Registers for the event.
    /// </summary>
    /// <param name="handler">The event handler</param>
    public void Register(Action<T,U> handler)
    {
        if (_callbacks == null)
        {
            _callbacks = new List<Action<T,U>>();
        }

        if (!_callbacks.Contains(handler))
        {
            _callbacks.Add(handler);
            _invocationList = null;
        }
    }
    
    /// <summary>
    /// Unregisters for the event.
    /// </summary>
    /// <param name="handler">The event handler</param>
    public void UnRegister(Action<T,U> handler)
    {
        if (_callbacks != null && _callbacks.Contains(handler))
        {
            _callbacks.Remove(handler);
            _invocationList = null;
        }
    }
    
    /// <summary>
    /// Invokes the event
    /// </summary>
    /// <param name="argument">The type argument.</param>
    public void Invoke(T argument, U argument2)
    {
        if (_callbacks != null)
        {
            bool rebuild = false;
            Action<T,U>[] cbArray = GetInvocationList();
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
                    cbArray[i](argument, argument2);
                }
                else
                {
                    rebuild = true;
                }
            }

            if (rebuild)
            {
                Clean();
            }
        }
    }

    /// <summary>
    /// Rebuilds the invocation list removing null targets.
    /// </summary>
    public void Clean()
    {
        if (_callbacks != null)
        {
            var cbArray = GetInvocationList();
            _callbacks.Clear();
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
                    _callbacks.Add(cbArray[i]);
                }
            }
            _invocationList = null;
        }
    }

    /// <summary>
    /// Return an array of all registered actions
    /// </summary>
    /// <returns>The invocation list.</returns>
    public Action<T,U>[] GetInvocationList()
    {
        if (_invocationList == null && _callbacks != null)
        {
            _invocationList = _callbacks.ToArray();
        }
        return _invocationList;
    }


    /// <summary>
    /// Clears delegates from this instance.
    /// </summary>
    public void Clear()
    {
        if (_callbacks != null)
        {
            _callbacks.Clear();
        }
        _invocationList = null;
    }
}

/// <summary>
/// Event class to reduce boilerplate register-unregister code.  Takes three arguments.
/// </summary> 
public class SGNEvent<T, U, V>
{
    /// <summary>
    /// Event callbacks
    /// </summary>
    private List<Action<T,U,V>> _callbacks = null;

    /// <summary>
    /// Cache of the above list as an array
    /// </summary>
    private Action<T,U,V>[] _invocationList = null;

    /// <summary>
    /// Return the number of registered callbacks
    /// </summary>
    /// <value>The count.</value>
    public int Count
    {
        get
        {
            int count = 0;
            if (_callbacks != null)
            {
                count = _callbacks.Count;
            }
            return count;
        }
    }

    /// <summary>
    /// Registers for the event.
    /// </summary>
    /// <param name="handler">The event handler</param>
    public void Register(Action<T,U,V> handler)
    {
        if (_callbacks == null)
        {
            _callbacks = new List<Action<T,U,V>>();
        }

        if (!_callbacks.Contains(handler))
        {
            _callbacks.Add(handler);
            _invocationList = null;
        }
    }
    
    /// <summary>
    /// Unregisters for the event.
    /// </summary>
    /// <param name="handler">The event handler</param>
    public void UnRegister(Action<T,U,V> handler)
    {
        if (_callbacks != null && _callbacks.Contains(handler))
        {
            _callbacks.Remove(handler);
            _invocationList = null;
        }
    }
    
    /// <summary>
    /// Invokes the event
    /// </summary>
    /// <param name="argument">The type argument.</param>
    public void Invoke(T argument, U argument2, V argument3)
    {
        if (_callbacks != null)
        {
            bool rebuild = false;
            Action<T,U,V>[] cbArray = GetInvocationList();
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
                    cbArray[i](argument, argument2, argument3);
                }
                else
                {
                    rebuild = true;
                }
            }

            // rebuild the callback list removing the null entries
            if (rebuild)
            {
                Clean();
            }
        }
    }

    /// <summary>
    /// Rebuilds the invocation list removing null targets.
    /// </summary>
    public void Clean()
    {
        if (_callbacks != null)
        {
            var cbArray = GetInvocationList();
            _callbacks.Clear();
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
                    _callbacks.Add(cbArray[i]);
                }
            }
            _invocationList = null;
        }
    }

    /// <summary>
    /// Return an array of all registered actions
    /// </summary>
    /// <returns>The invocation list.</returns>
    public Action<T,U,V>[] GetInvocationList()
    {
        if (_invocationList == null && _callbacks != null)
        {
            _invocationList = _callbacks.ToArray();
        }
        return _invocationList;
    }

    /// <summary>
    /// Clears delegates from this instance.
    /// </summary>
    public void Clear()
    {
        if (_callbacks != null)
        {
            _callbacks.Clear();
        }
        _invocationList = null;
    }
}
