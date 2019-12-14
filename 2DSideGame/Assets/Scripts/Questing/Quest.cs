using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public sealed class Quest
{
    [SerializeField]
    string title;

    [SerializeField]
    string description;

    [SerializeField]
    QuestStep[] questSteps;
    int currentStep = 0;

    public bool complete { get; private set; }

    public void Init()
    {
        foreach (QuestStep step in questSteps)
            step.Init();
    }

    public void Evaluate()
    {
        if (questSteps[currentStep].complete)
            currentStep++;

        if (complete = currentStep == questSteps.Length)
            OnComplete();
        else
            questSteps[currentStep].Evaluate();
    }

    public void DisplayUI()
    {
        Debug.Log(title);
        Debug.Log(description);

        if (currentStep == questSteps.Length)
            questSteps[currentStep - 1].DisplayUI();
        else
            questSteps[currentStep].DisplayUI();
    }

    void OnComplete()
    {
        Debug.Log("Quest Complete");
    }

    public void OnPickItem(int itemType)
    {
        questSteps[currentStep].OnPickItem(itemType);
    }

    public void OnDamageDealt(float damageDealt, int objectType)
    {
        questSteps[currentStep].OnDamageDealt(damageDealt, objectType);
    }

    public void OnEnemyKilled(int enemyType)
    {
        questSteps[currentStep].OnEnemyKilled(enemyType);
    }

    public void OnAreaEntered(int areaID)
    {
        questSteps[currentStep].OnAreaEntered(areaID);
    }

    public void OnNpcConversation(string npcName)
    {
        questSteps[currentStep].OnNpcConversation(npcName);
    }
}
