using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[AddComponentMenu("Quests/Goals/DamageQuestGoal")]
public sealed class DamageQuestGoal : QuestGoal
{
    [SerializeField]
    float requiredDamage;

    [SerializeField]
    int targetObjectType;

    float currentDamage;

    public override void Init()
    {
        currentDamage = 0f;
    }

    public override void Evaluate()
    {
        if (currentDamage >= requiredDamage)
            OnComplete();
    }

    protected override void OnComplete()
    {
        complete = true;
        Debug.Log("All Damage Dealt!");
    }

    public override void DisplayUI()
    {
        Debug.Log(description);
        Debug.LogFormat("{0} / {1}", currentDamage, requiredDamage);
    }

    public override void OnDamageDealt(float damageDealt, int objectType)
    {
        if (objectType == targetObjectType)
            currentDamage += damageDealt;
    }
}
