using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "new DialogueLine", menuName = "Scriptable Objects/Dialogues/DialogueLine")]
public class DialogueLine : ScriptableObject
{
    public DialogueActor Actor;
    [TextArea(3, 10)]
    public string[] Sentences;
}
