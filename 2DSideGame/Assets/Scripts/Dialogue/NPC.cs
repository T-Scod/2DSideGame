using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This npc script will allow you to have as many npc convos as possible, you just need to add this script to that game object

public class NPC : MonoBehaviour
{
    public Dialogue m_dialogue;

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(m_dialogue);
    }
}
