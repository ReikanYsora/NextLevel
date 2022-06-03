using UnityEngine;

public class EventDialogueSystem : EventSystem
{
	#region ATTRIBUTES
	[Header("Event dialogue settings")]
	[Space]
	public DialogueLine[] dialogues;
	private DialogueMasterManager dialogueMasterManager;
	#endregion

	#region CONSTRUCTOR
	public EventDialogueSystem()
	{
		eventType = EventType.Dialogue;
	}
	#endregion

	#region UNITY METHODS
	private void Start()
	{
		dialogueMasterManager = FindObjectOfType<DialogueMasterManager>();
		dialogueMasterManager.OnDialogueEnded += CBDialogueEnded;
	}

	private void CBDialogueEnded()
	{
		RaiseOnEventSystemCompleted();
	}

	#endregion

	#region METHODS
	public override void ActiveEvent()
	{
		dialogueMasterManager.SetDialogueLines(dialogues);
		IsActive = true;
	}
	#endregion
}