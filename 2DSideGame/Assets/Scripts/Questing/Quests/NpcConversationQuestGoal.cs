using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[AddComponentMenu("Quests/Goals/NpcConversationQuestGoal")]
public sealed class NpcConversationQuestGoal : QuestGoal
{
    [SerializeField]
    string npcsName;

    public override void Init()
    {
    }

    public override void Evaluate()
    {
        if (complete)
            OnComplete();
    }

    protected override void OnComplete()
    {
        Debug.LogFormat("Had conversation with {0}!", npcsName);
    }

    public override void DisplayUI()
    {
        Debug.Log(description);
        Debug.LogFormat(complete ? "Had conversation with {0}" : "Haven't talked with {0}", npcsName);
    }

    public override void OnNpcConversation(string npcName)
    {
        if (npcsName == npcName)
            complete = true;
    }
}
