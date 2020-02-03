using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// hosts all info about a single dialogue 

[System.Serializable]
public class Dialogue
{
    public string m_name; // npc name
    [TextArea(3, 10)]
    public string[] m_sentences; // sentences that will load into our dialogue queue
}