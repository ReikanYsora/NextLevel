using UnityEngine;

public class EventTriggerSystem : EventSystem
{
	#region ATTRIBUTES
	[Header("Event trigger settings")]
	[Space]
	public Vector3 position;
	public float range;
	public string tagRequired;
	#endregion

	#region CONSTRUCTOR
	public EventTriggerSystem()
	{
		eventType = EventType.Trigger;
	}
	#endregion

	#region UNITY METHODS
	private void Start()
	{
		CircleCollider2D collider = gameObject.AddComponent(typeof(CircleCollider2D)) as CircleCollider2D;
		collider.radius = range;
		collider.isTrigger = true;
		transform.position = position;
		IsActive = true;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(position, range);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if ((collision.gameObject.CompareTag(tagRequired)) && (IsActive))
		{
			RaiseOnEventSystemCompleted();
		}
	}
	#endregion
}
