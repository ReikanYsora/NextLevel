using UnityEngine;

public class EventActionSystem : EventSystem
{
	#region ATTRIBUTES
	[Header("Event action settings")]
	[Space]
	public Vector3 position;
	public float range;
	public string tagRequired;
	private InputManager inputManager;
	private bool waitForValidation;
	#endregion

	#region CONSTRUCTOR
	public EventActionSystem()
	{
		eventType = EventType.Action;
		waitForValidation = false;
	}
	#endregion

	#region UNITY METHODS
	private void Start()
	{
		CircleCollider2D collider = gameObject.AddComponent(typeof(CircleCollider2D)) as CircleCollider2D;
		collider.radius = range;
		collider.isTrigger = true;
		transform.position = position;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(position, range);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if ((collision.gameObject.CompareTag(tagRequired)) && (IsActive))
		{
			waitForValidation = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if ((collision.gameObject.CompareTag(tagRequired)) && (IsActive))
		{
			waitForValidation = false;
		}
	}
	#endregion

	#region METHODS
	public override void ActiveEvent()
	{
		inputManager = FindObjectOfType<InputManager>();
		inputManager.OnValidatePressed += CBInputManagerValidatePressed;
		IsActive = true;
	}
	#endregion

	#region CALLBACKS
	private void CBInputManagerValidatePressed()
	{
		if (waitForValidation)
		{
			RaiseOnEventSystemCompleted();
			Destroy(GetComponent<CircleCollider2D>());
		}
	}
	#endregion
}