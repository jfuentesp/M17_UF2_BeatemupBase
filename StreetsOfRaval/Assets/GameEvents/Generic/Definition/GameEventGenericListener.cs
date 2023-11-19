using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace streetsofraval { 
/// <summary>
/// The only thing we must do is create more abstract classes if we need more parameters 
/// to send. It's important that for each listener, there should be a generic game event
/// that matches with the same number of parameters.
/// </summary>
/// <typeparam name="T">Generic parameter</typeparam>
/*public abstract class GameEventListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public GameEvent Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised()
    {
        Response.Invoke();
    }
}*/

public abstract class GameEventListener<T> : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public GameEvent<T> Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent<T> Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(T parameter)
    {
        Response.Invoke(parameter);
    }
}

public abstract class GameEventListener<T0, T1> : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public GameEvent<T0, T1> Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent<T0, T1> Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(T0 parameter0, T1 parameter1)
    {
        Response.Invoke(parameter0, parameter1);
    }
}
}
