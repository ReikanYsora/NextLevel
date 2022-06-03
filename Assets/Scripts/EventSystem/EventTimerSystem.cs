using System.Collections;
using UnityEngine;

public class EventTimerSystem : EventSystem
{
	#region ATTRIBUTES
	public float delay;
	#endregion

	#region CONSTRUCTOR
	public EventTimerSystem()
	{
		eventType = EventType.Timer;
	}
	#endregion

	#region METHODS
	public override void ActiveEvent()
	{
		IsActive = true;
		StartCoroutine(WaitFor(delay));
	}

	IEnumerator WaitFor(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		RaiseOnEventSystemCompleted();
	}
	#endregion
}