using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[AddComponentMenu("Quests/Goals/KillQuestGoal")]
public sealed class KillQuestGoal : QuestGoal
{
    [SerializeField]
    int requiredNumberOfKills;

    int currentNumberOfKills;

    public override void Init()
    {
        currentNumberOfKills = 0;
    }

    public override void Evaluate()
    {
        if (currentNumberOfKills >= requiredNumberOfKills)
            OnComplete();
    }

    protected override void OnComplete()
    {
        complete = true;
        Debug.Log("All Enemies Killed!");
    }

    public override void DisplayUI()
    {
        Debug.Log(description);
        Debug.LogFormat("{0} / {1}", currentNumberOfKills, requiredNumberOfKills);
    }

    public override void OnEnemyKilled(Enemy.Type enemyType)
    {
        currentNumberOfKills++;
    }
}
