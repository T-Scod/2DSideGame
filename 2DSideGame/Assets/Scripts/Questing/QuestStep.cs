using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public sealed class QuestStep
{
    [SerializeField]
    string title;

    [SerializeField]
    string description;

    [SerializeField]
    List<QuestGoal> goals = new List<QuestGoal>();

    public bool complete { get; private set; }

    public void Init()
    {
        goals.ForEach(goal => goal.Init());
    }

    public void Evaluate()
    {
        goals.ForEach(goal => goal.Evaluate());
        complete = goals.TrueForAll(goal => goal.complete);
        if (complete)
            OnComplete();
    }

    void OnComplete()
    {
        Debug.Log("Quest Step complete");
    }

    public void DisplayUI()
    {
        Debug.Log(title);
        Debug.Log(description);
        goals.ForEach(goal => goal.DisplayUI());
    }

    public void OnPickItem(int itemType)
    {
        goals.ForEach(goal => goal.OnPickUpItem(itemType));
    }

    public void OnDamageDealt(float damageDealt, int objectType)
    {
        goals.ForEach(goal => goal.OnDamageDealt(damageDealt, objectType));
    }

    public void OnEnemyKilled(int enemyType)
    {
        goals.ForEach(goal => goal.OnEnemyKilled(enemyType));
    }

    public void OnAreaEntered(int areaID)
    {
        goals.ForEach(goal => goal.OnAreaEntered(areaID));
    }

    public void OnNpcConversation(string npcName)
    {
        goals.ForEach(goal => goal.OnNpcConversation(npcName));
    }
}
