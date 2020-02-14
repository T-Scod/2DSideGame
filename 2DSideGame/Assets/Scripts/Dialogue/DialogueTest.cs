using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    public DialogueInformation m_dialogue;

    /// <summary>
    /// Triggers the dialogue to begin.
    /// </summary>
    public void TriggerDialogue()
    {
        DialogueManager.Instance.EnqueueDialogue(m_dialogue);
    }
}
