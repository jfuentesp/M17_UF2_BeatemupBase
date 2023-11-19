using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace streetsofraval { 
/// <summary>
/// The only thing we must do is create more abstract classes if we need more parameters 
/// to send. It's important that for each game event, there should be a generic listener
/// that matches with the same number of parameters.
/// </summary>
/// <typeparam name="T">Generic parameter</typeparam>
/*public abstract class GameEvent : ScriptableObject
{
    /// <summary>
    /// The list of listeners that this event will notify if it is raised
    /// </summary>
    private readonly List<GameEventListener> m_eventListeners =
        new List<GameEventListener>();

    public void Raise()
    {
        for (int i = m_eventListeners.Count - 1; i >= 0; i--)
        {
            m_eventListeners[i].OnEventRaised();
        }
    }

    public void RegisterListener(GameEventListener listener)
    {
        if (!m_eventListeners.Contains(listener))
            m_eventListeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener listener)
    {
        if (m_eventListeners.Contains(listener))
            m_eventListeners.Remove(listener);
    }
}*/


public abstract class GameEvent<T> : ScriptableObject
{
    /// <summary>
    /// The list of listeners that this event will notify if it is raised
    /// </summary>
    private readonly List<GameEventListener<T>> m_eventListeners =
        new List<GameEventListener<T>>();

    public void Raise(T parameter)
    {
        for (int i = m_eventListeners.Count - 1; i >= 0; i--)
        {
            m_eventListeners[i].OnEventRaised(parameter);
        }
    }

    public void RegisterListener(GameEventListener<T> listener)
    {
        if (!m_eventListeners.Contains(listener))
            m_eventListeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener<T> listener)
    {
        if (m_eventListeners.Contains(listener))
            m_eventListeners.Remove(listener);
    }
}

public abstract class GameEvent<T0, T1> : ScriptableObject
{
    /// <summary>
    /// The list of listeners that this event will notify if it is raised
    /// </summary>
    private readonly List<GameEventListener<T0, T1>> m_eventListeners =
        new List<GameEventListener<T0, T1>>();

    public void Raise(T0 parameter0, T1 parameter1)
    {
        for (int i = m_eventListeners.Count - 1; i >= 0; i--)
        {
            m_eventListeners[i].OnEventRaised(parameter0, parameter1);
        }
    }

    public void RegisterListener(GameEventListener<T0, T1> listener)
    {
        if (!m_eventListeners.Contains(listener))
            m_eventListeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener<T0, T1> listener)
    {
        if (m_eventListeners.Contains(listener))
            m_eventListeners.Remove(listener);
    }
}
}
