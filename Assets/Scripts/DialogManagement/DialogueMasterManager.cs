using UnityEngine;

public class DialogueMasterManager : MonoBehaviour
{
    #region ATTRIBUTES
    [Header("Dialogue settings")]
    [Space]
    private DialogueLine[] dialogueLines;
    public float timeToPassDialogue;
    private int indexDialogue = 0;

    [Header("UI settings")]
    [Space]
    public GameObject UIDialoguePrefab;
    public Transform UIDialogueAnchor;
    private bool needToCreateNextDialogue;
    #endregion

    #region EVENTS
    public delegate void DialogueEnded();
    public event DialogueEnded OnDialogueEnded;
    #endregion

    #region UNITY METHODS
    private void Update()
    {
        if ((dialogueLines != null) && (dialogueLines.Length > 0))
        {
            if ((indexDialogue < dialogueLines.Length) && (!FindObjectOfType<UIDialogueSentence>()) && (needToCreateNextDialogue))
            {
                needToCreateNextDialogue = false;
                GameObject tempUIDialogueDisplay = Instantiate(UIDialoguePrefab, UIDialogueAnchor.position, Quaternion.identity);
                tempUIDialogueDisplay.transform.SetParent(UIDialogueAnchor);
                tempUIDialogueDisplay.transform.localPosition = Vector3.zero;
                tempUIDialogueDisplay.transform.localScale = new Vector3(1, 1, 1);
                tempUIDialogueDisplay.GetComponent<UIDialogueSentence>().Initialize(this, dialogueLines[indexDialogue], timeToPassDialogue);
            }
        }
    }

    internal void SetDialogueLines(DialogueLine[] dialogues)
    {
        dialogueLines = dialogues;
        indexDialogue = 0;
        StartDialogue();
    }
    #endregion

    #region METHODS
    public void StartDialogue()
    {
        needToCreateNextDialogue = true;
    }

    public void DialogueCompleted()
    {
        needToCreateNextDialogue = true;
        indexDialogue++;

        if (indexDialogue >= dialogueLines.Length)
        {
            OnDialogueEnded?.Invoke();
        }
    }
    #endregion
}
