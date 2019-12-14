using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[AddComponentMenu("Quests/Goals/PickUpItemQuestGoal")]
public sealed class PickUpItemQuestGoal : QuestGoal
{
    [SerializeField]
    int targetItemType;

    [SerializeField]
    int requiredNumberOfItems;

    int currentNumberOfItems;

    public override void Init()
    {
        currentNumberOfItems = 0;
    }

    public override void Evaluate()
    {
        if (currentNumberOfItems >= requiredNumberOfItems)
            OnComplete();
    }

    protected override void OnComplete()
    {
        complete = true;
        Debug.Log("All Items Found!");
    }

    public override void DisplayUI()
    {
        Debug.Log(description);
        Debug.LogFormat("{0} / {1}", currentNumberOfItems, requiredNumberOfItems);
    }

    public override void OnPickUpItem(int itemType)
    {
        if (itemType == targetItemType)
            currentNumberOfItems++;
    }
}
