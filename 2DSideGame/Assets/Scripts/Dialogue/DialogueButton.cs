﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueButton : MonoBehaviour
{
    public void NextLine()
    {
        DialogueManager.Instance.DequeueDialogue();
    }
}
