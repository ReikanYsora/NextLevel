using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogueSentence : MonoBehaviour
{
	#region CONSTANTS
	private const string ANIMATOR_IS_OPEN = "IsOpen";
	#endregion

	#region ATTRIBUTES
	private DialogueMasterManager DialogueMasterManager;
	private DialogueActor DialogueActor;
	private DialogueLine DialogueLine;
	private InputManager inputManager;
	private Queue<string> sentencesToDisplay;
	private bool isInitialized;
	private Animator DialogueBoxAnimator;
	private float timeToNextSentence;
	private float timeTempToNextSentence;
	private bool canDisplayNextSentence;

	[Header("UI elements")]
	[Space]
	public Image backgroundSprite;
	public TextMeshProUGUI actorText;
	public TextMeshProUGUI sentenceText;
	private bool displayNextDialogue;

	[Header("SFX elements")]
	[Space]
	public GameObject SFX_DialogNext;
	#endregion

	#region UNITY METHODS
	private void Awake()
	{
		DialogueBoxAnimator = GetComponent<Animator>();
		displayNextDialogue = false;
	}

	private void Update()
	{
		if (!canDisplayNextSentence)
		{
			timeTempToNextSentence += Time.deltaTime;

			if (timeTempToNextSentence >= timeToNextSentence)
			{
				canDisplayNextSentence = true;
				timeTempToNextSentence = 0.0f;
			}
		}

		if (displayNextDialogue)
		{
			Instantiate(SFX_DialogNext, transform.position, Quaternion.identity);
			displayNextDialogue = false;

			if (sentencesToDisplay.Count > 0)
			{
				sentenceText.text = sentencesToDisplay.Dequeue();
				timeTempToNextSentence = 0.0f;
				canDisplayNextSentence = false;
			}
			else
			{
				DialogueBoxAnimator.SetBool(ANIMATOR_IS_OPEN, false);
			}
		}
	}
	#endregion

	#region METHODS
	public void Initialize(DialogueMasterManager master, DialogueLine line, float timeToNext = 1.0f)
	{
		timeToNextSentence = timeToNext;
		DialogueMasterManager = master;
		DialogueLine = line;
		DialogueActor = line.Actor;
		sentencesToDisplay = new Queue<string>();
		DialogueLine.Sentences.ToList().ForEach(x => sentencesToDisplay.Enqueue(x));

		actorText.text = DialogueActor.Name;
		backgroundSprite.color = DialogueActor.Color;

		inputManager = FindObjectOfType<InputManager>();
		inputManager.OnValidatePressed += CBInputManagerOnValidatePressed;
		isInitialized = true;

		if (sentencesToDisplay.Count > 0)
		{
			DialogueBoxAnimator.SetBool(ANIMATOR_IS_OPEN, true);
			sentenceText.text = sentencesToDisplay.Dequeue();
			timeTempToNextSentence = 0.0f;
			canDisplayNextSentence = false;
		}
	}

	public void DestroyFromAnimation()
	{
		DialogueMasterManager.DialogueCompleted();
		Destroy(gameObject);
	}

	private void CBInputManagerOnValidatePressed()
	{
		if ((isInitialized) && (!displayNextDialogue) && (canDisplayNextSentence))
		{ 
			displayNextDialogue = true;
		}
	}
	#endregion
}
