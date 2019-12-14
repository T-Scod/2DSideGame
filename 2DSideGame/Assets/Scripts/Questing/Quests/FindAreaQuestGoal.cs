using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[AddComponentMenu("Quests/Goals/FindAreaQuestGoal")]
public sealed class FindAreaQuestGoal : QuestGoal
{
    [SerializeField]
    int areaID;

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
        Debug.Log("Area Discovered!");
    }

    public override void DisplayUI()
    {
        Debug.Log(description);
        Debug.Log(complete ? "Area is discovered" : "Area is undiscovered");
    }

    public override void OnAreaEntered(int areaID)
    {
        if (this.areaID == areaID)
            complete = true;
    }
}
