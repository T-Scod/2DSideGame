using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    public DialogueInformation m_dialogue;

    public void TriggerDialogue()
    {
        DialogueManager.Instance.EnqueueDialogue(m_dialogue);
    }
}
