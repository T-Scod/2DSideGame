using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Quests/QuestGiver")]
public class QuestGiver : MonoBehaviour
{
    [SerializeField]
    Quest quest;

    public void AcceptQuest()
    {
        QuestManager.AddQuest(quest);
    }
}
