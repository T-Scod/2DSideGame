﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogues")]
public class DialogueInformation : ScriptableObject
{
    [System.Serializable]
    public class Info
    {
        public CharacterProfile m_character;
        [TextArea(3, 8)]
        public string m_text;
    }

    [Header("Insert Dialogue Information Below")]
    public Info[] m_dialogueInfo;
}
