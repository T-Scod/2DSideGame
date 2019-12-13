using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    [SerializeField]
    string title;

    [SerializeField]
    string description;

    [SerializeField]
    public List<QuestGoal> goals = new List<QuestGoal>();

    public bool active { get; private set; }
    public bool complete { get; protected set; }
    
    public delegate void PickUpItem(int itemType);
    public delegate void DamageDealt(float damageDealt, int objectType);
    public delegate void AreaEntered(int areaID);
    public delegate void NPCConversation(string npcName);

    public void Init()
    {
        goals.ForEach(g => g.Init());
    }

    public void Evaluate()
    {
        goals.ForEach(g => g.Evaluate());

        complete = goals.TrueForAll(g => g.complete);

        if (complete)
            OnComplete();
    }

    public void Activate()
    {
        active = true;
    }

    public void DisplayUI()
    {
        Debug.Log(title);
        Debug.Log(description);
        goals.ForEach(g => g.DisplayUI());
    }

    void OnComplete()
    {
        active = false;
        Debug.Log("Quest Complete");
    }

    public void OnPickItem(int itemType)
    {
        goals.ForEach(g => g.OnPickUpItem(itemType));
    }

    public void OnDamageDealt(float damageDealt, int objectType)
    {
        goals.ForEach(g => g.OnDamageDealt(damageDealt, objectType));
    }

    public void OnEnemyKilled(int enemyType)
    {
        goals.ForEach(g => g.OnEnemyKilled(enemyType));
    }

    public void OnAreaEntered(int areaID)
    {
        goals.ForEach(g => g.OnAreaEntered(areaID));
    }

    public void OnNpcConversation(string npcName)
    {
        goals.ForEach(g => g.OnNpcConversation(npcName));
    }
}
