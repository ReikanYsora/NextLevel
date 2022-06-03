using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class EventSystem : MonoBehaviour
{
	#region ATTRIBUTES
	protected EventType eventType;
	public string eventName;
	#endregion

	#region EVENTS
	public delegate void EventSystemCompleted(EventSystem es);
	public event EventSystemCompleted OnEventSystemCompleted;
	#endregion

	#region PROPERTIES
	public EventType EventType
	{
		get
		{
			return eventType;
		}
	}

	public bool IsActive {set; get;}
	#endregion

	#region METHODS
	public virtual void ActiveEvent() {}

	protected void RaiseOnEventSystemCompleted()
	{
		OnEventSystemCompleted?.Invoke(this);
	}
	#endregion
}

[Serializable]
public enum EventType
{
	Timer, Action, Dialogue, Trigger
}
