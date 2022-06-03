using System.Collections;
using UnityEngine;

public class EventSystemManager : MonoBehaviour
{
	#region ATTRIBUTES
	public EventSystem[] events;
	public int eventIndex;
	private EventSystem actualEvent;
	#endregion

	#region EVENTS
	public delegate void AllEventsCompleted();
	public event AllEventsCompleted OnAllEventsCompleted;
	#endregion

	#region UNITY METHODS
	private void Start()
	{
		InitializeEvents();
	}
	#endregion

	#region METHODS
	private void InitializeEvents()
	{
		LoadNextEvent();
	}

	private void LoadNextEvent()
	{		
		if (eventIndex < events.Length)
		{
			actualEvent = events[eventIndex];
			Debug.Log("Event loaded :" + actualEvent.eventName);
			actualEvent.OnEventSystemCompleted += CBOnEventSystelCompleted;
			actualEvent.ActiveEvent();
		}
		else
		{
			OnAllEventsCompleted?.Invoke();
		}
	}
	#endregion

	#region CALLBACKS
	private void CBOnEventSystelCompleted(EventSystem es)
	{
		actualEvent.OnEventSystemCompleted -= CBOnEventSystelCompleted;
		Debug.Log("Event completed :" + es.eventName);
		eventIndex++;
		LoadNextEvent();
	}
	#endregion
}
