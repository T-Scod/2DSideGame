using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[AddComponentMenu("Quests/QuestManager")]
public sealed class QuestManager : MonoBehaviour
{
    #region Make Singleton
    static QuestManager instance = null;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    PlayerController player;
    List<Quest> quests = new List<Quest>();

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerController>();

        quests.ForEach(q => q.Init());
    }

    // Update is called once per frame
    void Update()
    {
        quests.RemoveAll(q => q.complete);
        quests.ForEach(q => q.Evaluate());
        quests.ForEach(q => q.DisplayUI());
    }

    public static void AddQuest(Quest quest)
    {
	    instance.quests.Add(quest);
    }

    public static void DisplayQuests()
    {
        instance.quests.ForEach(q => q.DisplayUI());
    }

    public static void OnPickUpItem(int itemType)
    {
        instance.quests.ForEach(q => q.OnPickItem(itemType));
    }

    public static void OnDamageDealt(float damageDealt, int objectType)
    {
        instance.quests.ForEach(q => q.OnDamageDealt(damageDealt, objectType));
    }

    public static void OnEnemyKilled(Enemy.Type enemyType)
    {
        instance.quests.ForEach(q => q.OnEnemyKilled(enemyType));
    }

    public static void OnAreaEntered(int areaID)
    {
        instance.quests.ForEach(q => q.OnAreaEntered(areaID));
    }

    public static void OnNpcConversation(string npcName)
    {
        instance.quests.ForEach(q => q.OnNpcConversation(npcName));
    }
}
