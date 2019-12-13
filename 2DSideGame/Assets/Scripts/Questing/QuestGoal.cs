using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class QuestGoal : MonoBehaviour
{
    [SerializeField]
    protected string _description;
    public string description { get => _description; }

    public bool complete { get; protected set; }

    public abstract void Init();
    public abstract void Evaluate();
    protected abstract void OnComplete();

    public abstract void DisplayUI();

    public virtual void OnPickUpItem(int itemType)
    {
    }

    public virtual void OnDamageDealt(float damageDealt, int objectType)
    {
    }

    public virtual void OnEnemyKilled(int enemyType)
    {
    }

    public virtual void OnAreaEntered(int areaID)
    {
    }

    public virtual void OnNpcConversation(string npcName)
    {
    }
}
